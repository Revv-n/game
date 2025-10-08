using System;
using System.Collections.Generic;
using System.Linq;
using Merge;
using Merge.Core.Masters;
using Merge.MotionDesign;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class MotionController : Controller<MotionController>, IDragController, IMasterController
{
	private const int FLY_ORDER = 100;

	private const int NORMAL_ORDER = 0;

	[SerializeField]
	private float smartMergeMaxTime = 1f;

	[SerializeField]
	private SelectObjectTweenBuilder pingTweenBuilder;

	private GameItem draggingItem;

	private Vector3 dragTarget;

	private Point dragTargetPoint;

	public bool IsBlockedByTutor;

	private List<ITileBecomesEmptyListener> emptyListeners;

	private List<IGIDropCatcher> dragCatchers;

	private List<Merge.IInputBlocker> inputBlockers;

	private float smartMergeTime;

	[Inject]
	private IInputBlockerController inputBlocker;

	public bool IsDragging => draggingItem != null;

	private PointMatrix<GameItem> GameItems => Controller<GameItemController>.Instance.CurrentField.Field;

	private bool AllowSmartMerge => smartMergeTime > 0f;

	public static event Action<GameItem, Point> OnItemMovedFrom;

	public static event Action<GameItem> OnItemDragReleased;

	public static event Action<GameItem> OnItemDragEnd;

	public static event Action<GameItem> OnItemBecomesDrag;

	public static event Action<GameItem, Vector3> OnItemDrag;

	public override void Preload()
	{
		base.Preload();
		GIMaster.OnSwap += AtItemSwapped;
		void AtItemSwapped(GameItem oldItem, GameItem newItem, MergeField field)
		{
			if (IsDragging && draggingItem == oldItem)
			{
				draggingItem = newItem;
				newItem.Position = oldItem.Position;
				((IDragController)this).AtDrag(oldItem.Position, Vector3.zero);
			}
		}
	}

	private void SendItemTo(GameItem gameItem, Point point)
	{
		gameItem.Coordinates = point;
		gameItem.DoFlyTo(TileController.GetPosition(point));
	}

	private void SwapPoints(Point p1, Point p2)
	{
		GameItem gameItem = GameItems[p1];
		GameItem gameItem2 = GameItems[p2];
		GameItems.Swap(p1, p2);
		if (gameItem != null)
		{
			SendItemTo(gameItem, p2);
		}
		if (gameItem2 != null)
		{
			SendItemTo(gameItem2, p1);
		}
	}

	void IDragController.AtStartDrag(Vector3 position, Vector3 initDragPosition)
	{
		if (!IsBlockedByTutor && !inputBlockers.Any((Merge.IInputBlocker x) => x.BlocksClick) && !inputBlocker.ClickBlock && TileController.TryGetPoint(initDragPosition, out var point) && GameItems.TryGetValue(point, out var value))
		{
			if (!value.AllowMotion)
			{
				pingTweenBuilder.BuildLockTween(value.IconRenderer.transform);
				return;
			}
			draggingItem = value;
			dragTarget = position;
			dragTargetPoint = draggingItem.Coordinates;
			draggingItem.IconRenderer.SetOrder(100);
			MotionController.OnItemBecomesDrag?.Invoke(draggingItem);
			value.SetDragged(isDragged: true);
		}
	}

	void IDragController.AtDrag(Vector3 position, Vector3 delta)
	{
		if (IsDragging)
		{
			dragTarget = position;
			if (TileController.TryGetPoint(dragTarget, out var point) && point != dragTargetPoint)
			{
				dragTargetPoint = point;
				smartMergeTime = smartMergeMaxTime;
			}
			MotionController.OnItemDrag?.Invoke(draggingItem, position);
		}
	}

	void IDragController.AtEndDrag(Vector3 position)
	{
		if (!IsDragging)
		{
			return;
		}
		GameItem tmpDragged = draggingItem;
		Point coordinates = tmpDragged.Coordinates;
		draggingItem.IconRenderer.SetOrder(0);
		MotionController.OnItemDragReleased?.Invoke(draggingItem);
		draggingItem.SetDragged(isDragged: false);
		draggingItem = null;
		IGIDropCatcher iGIDropCatcher = dragCatchers.FirstOrDefault((IGIDropCatcher x) => x.IsCatchesDrop(tmpDragged));
		if (iGIDropCatcher != null)
		{
			iGIDropCatcher.CatchDrop(tmpDragged);
			return;
		}
		if (!TileController.TryGetPoint(position, out var point))
		{
			if (!TrySmartMerge(tmpDragged))
			{
				tmpDragged.DoFlyTo(TileController.GetPosition(tmpDragged.Coordinates));
				tmpDragged.SendActionMovedFrom(tmpDragged.Coordinates);
				MotionController.OnItemDragEnd?.Invoke(tmpDragged);
			}
			return;
		}
		if (coordinates == point)
		{
			tmpDragged.DoFlyTo(TileController.GetPosition(tmpDragged.Coordinates));
			tmpDragged.SendActionMovedFrom(tmpDragged.Coordinates);
			MotionController.OnItemDragEnd?.Invoke(tmpDragged);
			return;
		}
		if (GameItems.TryGetValue(point, out var value))
		{
			if (Controller<StackController>.Instance.TryPullItem(value, tmpDragged))
			{
				return;
			}
			if (Controller<MergeController>.Instance.CanBeMerged(tmpDragged, value))
			{
				Controller<MergeController>.Instance.Merge(tmpDragged, value);
				SendTileBecomesEmpty(coordinates);
				return;
			}
			if (TrySmartMerge(tmpDragged))
			{
				return;
			}
			string message;
			bool num = value.AllowInteraction(GIModuleType.Merge, out message);
			string message2;
			bool flag = tmpDragged.AllowInteraction(GIModuleType.Merge, out message2);
			bool flag2 = Controller<CollectionController>.Instance.IsLastInCollection(tmpDragged.Key);
			bool flag3 = num && flag;
			if (tmpDragged.Key == value.Key && flag3 && flag2)
			{
				tmpDragged.DoFlyTo(TileController.GetPosition(tmpDragged.Coordinates));
				tmpDragged.SendActionMovedFrom(tmpDragged.Coordinates);
				MotionController.OnItemDragEnd?.Invoke(tmpDragged);
				return;
			}
			if (tmpDragged.Key == value.Key && !flag3)
			{
				tmpDragged.DoFlyTo(TileController.GetPosition(tmpDragged.Coordinates));
				tmpDragged.SendActionMovedFrom(tmpDragged.Coordinates);
				MotionController.OnItemDragEnd?.Invoke(tmpDragged);
				return;
			}
			if (!value.AllowMotion)
			{
				tmpDragged.DoFlyTo(TileController.GetPosition(tmpDragged.Coordinates));
				tmpDragged.SendActionMovedFrom(tmpDragged.Coordinates);
				MotionController.OnItemDragEnd?.Invoke(tmpDragged);
				return;
			}
			if (!Controller<GameItemController>.Instance.IsFull)
			{
				Point p = Controller<GameItemController>.Instance.SpiralSerch(point, (GameItem x) => x == null);
				SwapPoints(point, p);
				SwapPoints(coordinates, point);
				tmpDragged.SendActionMovedFrom(coordinates);
				value.SendActionMovedFrom(point);
				MotionController.OnItemMovedFrom?.Invoke(value, point);
				MotionController.OnItemMovedFrom?.Invoke(tmpDragged, coordinates);
				MotionController.OnItemDragEnd?.Invoke(tmpDragged);
				return;
			}
		}
		SwapPoints(coordinates, point);
		SendTileBecomesEmpty(coordinates);
		tmpDragged.SendActionMovedFrom(coordinates);
		MotionController.OnItemMovedFrom?.Invoke(tmpDragged, coordinates);
		MotionController.OnItemDragEnd?.Invoke(tmpDragged);
	}

	private bool TrySmartMerge(GameItem draggingItem)
	{
		if (!AllowSmartMerge)
		{
			return false;
		}
		Point coordinates = draggingItem.Coordinates;
		List<GameItem> list = (from x in GIMaster.Field.GetNotEmptyTilesDonut(dragTargetPoint)
			select GIMaster.GetItemAt(x)).ToList();
		if (GIMaster.TryGetItemAt(dragTargetPoint, out var result))
		{
			list.Add(result);
		}
		GameItem gameItem = list.FirstOrDefault((GameItem x) => !x.HasTaskMark && x != draggingItem && Controller<MergeController>.Instance.CanBeMerged(draggingItem, x));
		if (gameItem == null)
		{
			return false;
		}
		Controller<MergeController>.Instance.Merge(draggingItem, gameItem);
		SendTileBecomesEmpty(coordinates);
		return true;
	}

	private void Update()
	{
		if (IsDragging && !IsBlockedByTutor)
		{
			smartMergeTime -= Time.deltaTime;
			draggingItem.LerpTo(dragTarget);
		}
	}

	void IMasterController.LinkControllers(IList<BaseController> controllers)
	{
		emptyListeners = controllers.OfType<ITileBecomesEmptyListener>().ToList();
		dragCatchers = controllers.OfType<IGIDropCatcher>().ToList();
		inputBlockers = controllers.OfType<Merge.IInputBlocker>().ToList();
	}

	private void SendTileBecomesEmpty(Point p)
	{
		emptyListeners.ForEach(delegate(ITileBecomesEmptyListener x)
		{
			x.AtTileBecomesEmpty(p);
		});
	}
}
