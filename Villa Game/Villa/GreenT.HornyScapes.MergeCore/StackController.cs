using System;
using System.Collections.Generic;
using Merge;
using Merge.Core.Masters;
using Merge.MotionDesign;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class StackController : Controller<StackController>, IClickInteractionController, IModuleController, ICreateItemListener
{
	private class DragManager
	{
		private ShineCircleCreator pullEffectCreator;

		private Point startPoint;

		private Point? overlapedPoint;

		private GameItem draggingItem;

		public DragManager()
		{
			pullEffectCreator = Controller<StackController>.Instance.mergeEffectCreator;
			pullEffectCreator.Init(Controller<StackController>.Instance.transform);
			MotionController.OnItemBecomesDrag += AtBecomesDrag;
		}

		public void OnDestroy()
		{
			pullEffectCreator.Pool.ClearPool();
			MotionController.OnItemBecomesDrag -= AtBecomesDrag;
		}

		private void AtBecomesDrag(GameItem gi)
		{
			SetSubscribeDrag(subscribe: true);
			overlapedPoint = gi.Coordinates;
			startPoint = gi.Coordinates;
			draggingItem = gi;
		}

		private void AtDrag(GameItem gi, Vector3 pos)
		{
			if (!TileController.TryGetPoint(pos, out var point))
			{
				if (overlapedPoint.HasValue)
				{
					AtOverlapedPointChange(null);
				}
			}
			else if (!overlapedPoint.HasValue || point != overlapedPoint.Value)
			{
				AtOverlapedPointChange(point);
			}
		}

		private void AtEndsDrag(GameItem obj)
		{
			pullEffectCreator.Pool.ActiveElements.ForEach(delegate(MergeAllownEffect x)
			{
				x.Hide();
			});
			draggingItem = null;
			SetSubscribeDrag(subscribe: false);
		}

		private void AtOverlapedPointChange(Point? newPoint)
		{
			List<MergeAllownEffect> activeElements = pullEffectCreator.Pool.ActiveElements;
			if (activeElements.Count > 0)
			{
				foreach (MergeAllownEffect item in activeElements)
				{
					if (item != null)
					{
						item.Hide();
					}
				}
			}
			overlapedPoint = newPoint;
			if (newPoint.HasValue && !(newPoint == startPoint) && !(Controller<GameItemController>.Instance.CurrentField.Field[newPoint.Value] == null) && Controller<StackController>.Instance.IsPushItem(Controller<GameItemController>.Instance.CurrentField.Field[newPoint.Value], draggingItem))
			{
				pullEffectCreator.Pool.Pop().transform.position = TileController.GetPosition(newPoint.Value);
			}
		}

		private void SetSubscribeDrag(bool subscribe)
		{
			if (subscribe)
			{
				MotionController.OnItemDragReleased += AtEndsDrag;
				MotionController.OnItemDrag += AtDrag;
			}
			else
			{
				MotionController.OnItemDragReleased -= AtEndsDrag;
				MotionController.OnItemDrag -= AtDrag;
			}
		}
	}

	[SerializeField]
	private PulsationTweenBuilder pulsationTweenBuilder;

	[SerializeField]
	private MergeCreateTweenBuilder creationTweenBuilder;

	[SerializeField]
	private ShineCircleCreator mergeEffectCreator;

	[SerializeField]
	private LightningTweenBuilder lightningPrefab;

	[Space]
	[Header("Animation")]
	[SerializeField]
	private float radius;

	[SerializeField]
	private int amountDrig;

	[SerializeField]
	private float timeDrig;

	[SerializeField]
	private float deltaPositionY;

	[SerializeField]
	private float endSize;

	private DragManager dragManager;

	private ModifyController modifyController;

	private SignalBus signalBus;

	private int tabIDNoHards;

	private GameItemController Field => Controller<GameItemController>.Instance;

	GIModuleType IModuleController.ModuleType => GIModuleType.Stack;

	int ICreateItemListener.Priority => Priority.Normal;

	public event Action<GameItem, GameItem> OnPushItem;

	public event Action<GameItem, GameItem> OnPullItem;

	[Inject]
	private void InnerInit(ModifyController modifyController)
	{
		this.modifyController = modifyController;
	}

	public override void Init()
	{
		dragManager = new DragManager();
		Controller<LockedController>.Instance.OnItemActionUnlock += AtItemUnlock;
	}

	public override void Preload()
	{
		base.Preload();
		Field.AfterItemCreated += AfterItemCreated;
	}

	private void AtItemUnlock(GameItem gi)
	{
		if (gi.TryGetBox<GIBox.Stack>(out var box))
		{
			box.ValidateEffect();
		}
	}

	void ICreateItemListener.AtItemCreated(GameItem item, MergeField mergeField)
	{
		if (item.Config.TryGetModule<ModuleConfigs.Stack>(out var result))
		{
			ModuleDatas.Stack stack = item.Data.GetModule<ModuleDatas.Stack>();
			if (stack == null)
			{
				stack = new ModuleDatas.Stack();
				item.Data.Modules.Add(stack);
			}
			GIBox.Stack box = new GIBox.Stack(stack, result);
			item.SetIconClock();
			item.AddBox(box);
		}
	}

	private void AfterItemCreated(GameItem item)
	{
		if (item.TryGetBox<GIBox.Stack>(out var box))
		{
			box.AttachTweener(UnityEngine.Object.Instantiate(lightningPrefab));
		}
	}

	public bool IsPushItem(GameItem gameStack, GameItem gameItem)
	{
		if (gameStack == null || gameItem == null)
		{
			return false;
		}
		if (gameStack.IsLocked || gameItem.IsLocked || gameStack.IsBubbled || gameItem.IsBubbled)
		{
			return false;
		}
		return gameStack.GetBox<GIBox.Stack>()?.IsPushItem(gameItem) ?? false;
	}

	public bool TryPullItem(GameItem gameStack, GameItem gameItem)
	{
		if (gameStack == null || gameItem == null)
		{
			return false;
		}
		if (gameStack.IsLocked || gameItem.IsLocked || gameStack.IsBubbled || gameItem.IsBubbled)
		{
			return false;
		}
		GIBox.Stack box = gameStack.GetBox<GIBox.Stack>();
		if (box == null)
		{
			return false;
		}
		if (box.TryPushItem(gameItem))
		{
			GIMaster.Field.RemoveItem(gameItem);
			this.OnPullItem?.Invoke(gameStack, gameItem);
			return true;
		}
		return false;
	}

	public ClickResult Interact(GameItem gameItem)
	{
		if (gameItem.IsLocked || gameItem.IsBubbled)
		{
			return ClickResult.None;
		}
		GIBox.Stack box = gameItem.GetBox<GIBox.Stack>();
		if (box == null || box.Data.Items.Count == 0 || Field.IsFull)
		{
			return ClickResult.None;
		}
		Point nearEmptyPoint = Field.GetNearEmptyPoint(gameItem.Coordinates);
		GIData data = null;
		if (box.TryPopItem(out data))
		{
			GIData giData = data.Copy().SetCoordinates(nearEmptyPoint);
			GameItem gameItem2 = Field.TakeItemFromSomethere(giData);
			gameItem2.DoCreateFrom(gameItem.Position);
			this.OnPushItem?.Invoke(gameItem, gameItem2);
			Merge.Sounds.Play("spawn");
		}
		return ClickResult.None;
	}

	protected override void OnDestroy()
	{
		dragManager?.OnDestroy();
		base.OnDestroy();
	}
}
