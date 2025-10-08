using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Merge;
using Merge.Core.Masters;
using Merge.MotionDesign;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class SelectionController : Controller<SelectionController>, IClickController, IMasterController, IDragController
{
	[SerializeField]
	private SelectionFrame selectionObject;

	[SerializeField]
	private SelectObjectTweenBuilder pingTweenBuilder;

	private Dictionary<GIModuleType, IClickInteractionController> interactionControllers = new Dictionary<GIModuleType, IClickInteractionController>();

	private List<Merge.IInputBlocker> inputBlockers = new List<Merge.IInputBlocker>();

	private List<GIKey> blockExceptions = new List<GIKey>();

	[Inject]
	private IInputBlockerController inputBlocker;

	public GameItem Selected { get; private set; }

	public override int PreloadOrder => -5;

	private GameItemController Field => Controller<GameItemController>.Instance;

	public event Action<GameItem> OnSelectionChange;

	public override void Init()
	{
		base.Init();
		ClearSelection();
		MotionController.OnItemDragEnd -= AtItemDragEnd;
		MotionController.OnItemDragEnd += AtItemDragEnd;
		Controller<GameItemController>.Instance.OnItemRemoved -= DeselectObject;
		Controller<GameItemController>.Instance.OnItemRemoved += DeselectObject;
	}

	protected override void OnDestroy()
	{
		MotionController.OnItemDragEnd -= AtItemDragEnd;
		base.OnDestroy();
		if (Controller<GameItemController>.Instance != null)
		{
			Controller<GameItemController>.Instance.OnItemRemoved -= DeselectObject;
		}
	}

	private void AtItemDragEnd(GameItem gi)
	{
		Select(gi);
	}

	private void DeselectObject(GameItem obj)
	{
		if (Controller<SelectionController>.Instance.Selected == obj)
		{
			Controller<SelectionController>.Instance.ClearSelection();
		}
	}

	public void ClearSelection()
	{
		Deselect();
	}

	public void AddItemToIgnoreBlockClick(params GIKey[] items)
	{
		foreach (GIKey item in items)
		{
			blockExceptions.Add(item);
		}
	}

	public void RemoveItemToIgnoreBlockClick(params GIKey[] items)
	{
		foreach (GIKey item in items)
		{
			blockExceptions.Remove(item);
		}
	}

	public void SetSelection(GameItem gi)
	{
		Select(gi);
	}

	public void AtClick(Vector3 position, Vector3 downPosition)
	{
		AtClick(position);
	}

	private ClickResult AtClick(Vector3 position)
	{
		if (!TileController.TryGetPoint(position, out var point))
		{
			return EmptyClick();
		}
		GameItem gameItem = Field.CurrentField.Field[point];
		if (gameItem == null)
		{
			return EmptyClick();
		}
		if ((inputBlockers.Any((Merge.IInputBlocker x) => x.BlocksClick) && !blockExceptions.Contains(gameItem.Key)) || inputBlocker.ClickBlock)
		{
			return EmptyClick();
		}
		if (Selected != gameItem)
		{
			return Select(gameItem);
		}
		return Interact(gameItem) switch
		{
			ClickResult.Select => Select(Field.CurrentField.Field[point]), 
			ClickResult.Deselect => Deselect(), 
			_ => ClickResult.None, 
		};
	}

	private ClickResult Deselect()
	{
		Selected = null;
		this.OnSelectionChange?.Invoke(null);
		if (selectionObject != null)
		{
			selectionObject.SetActive(active: false);
		}
		return ClickResult.Deselect;
	}

	private ClickResult EmptyClick()
	{
		return ClickResult.None;
	}

	private ClickResult Select(GameItem gameItem)
	{
		if (gameItem.IsRemovingNow)
		{
			return ClickResult.Deselect;
		}
		Selected = gameItem;
		selectionObject.SetActive(active: true);
		selectionObject.transform.position = TileController.GetPosition(gameItem.Coordinates);
		selectionObject.SetSpritePack(gameItem.Config.HasModule(GIModuleType.Merge));
		ModuleDatas.Locked result;
		Tween tween = ((gameItem.Data.TryGetModule<ModuleDatas.Locked>(out result) && result.Strength > 0 && !result.BlocksMerge) ? pingTweenBuilder.BuildLockTween(gameItem.IconRenderer.transform) : pingTweenBuilder.BuildNormalTween(gameItem.IconRenderer.transform));
		gameItem.AppendOuterTween(tween);
		this.OnSelectionChange?.Invoke(Selected);
		return ClickResult.Select;
	}

	private ClickResult Interact(GameItem gameItem)
	{
		foreach (GIModuleType key in interactionControllers.Keys)
		{
			if (gameItem.Config.HasModule(key) && gameItem.AllowInteraction(key))
			{
				return interactionControllers[key].Interact(gameItem);
			}
		}
		return ClickResult.None;
	}

	void IMasterController.LinkControllers(IList<BaseController> controllers)
	{
		foreach (BaseController controller in controllers)
		{
			if (controller is IClickInteractionController)
			{
				interactionControllers.Add((controller as IClickInteractionController).ModuleType, controller as IClickInteractionController);
			}
			if (controller is IInputBlocker)
			{
				inputBlockers.Add(controller as Merge.IInputBlocker);
			}
		}
	}

	private void AtItemDragComplete(GameItem sender)
	{
	}

	void IDragController.AtStartDrag(Vector3 position, Vector3 initDragPosition)
	{
		if (TileController.TryGetPoint(position, out var point))
		{
			GameItem gameItem = Field.CurrentField.Field[point];
			if (!(gameItem == null))
			{
				Select(gameItem);
				selectionObject.SetActive(active: false);
			}
		}
	}

	void IDragController.AtDrag(Vector3 position, Vector3 delta)
	{
	}

	void IDragController.AtEndDrag(Vector3 position)
	{
		if (!(Selected == null))
		{
			selectionObject.SetActive(active: true);
			selectionObject.transform.position = TileController.GetPosition(Selected.Coordinates);
			selectionObject.SetSpritePack(Selected.Config.HasModule(GIModuleType.Merge));
		}
	}
}
