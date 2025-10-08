using System.Collections.Generic;
using GreenT.HornyScapes.MergeCore;
using Merge;
using Merge.MotionDesign;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Inventory;

public class InventoryButton : ScaleButton
{
	[SerializeField]
	private MotionController motionController;

	[SerializeField]
	private ShineCircleCreator mergeEffectCreator;

	[SerializeField]
	private Image overlay;

	[SerializeField]
	private Sprite highlightedOverlay;

	[SerializeField]
	private Sprite defaultOverlay;

	private Point startPoint;

	private Point? overlapedPoint;

	private bool onEnterIcon;

	protected override void OnDestroy()
	{
		MotionController.OnItemBecomesDrag -= AtBecomesDrag;
	}

	private void AtBecomesDrag(GameItem gi)
	{
		SetSubscribeDrag(subscribe: true);
		overlapedPoint = gi.Coordinates;
		startPoint = gi.Coordinates;
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

	private void AtDrag(GameItem gi, Vector3 pos)
	{
		if (!TileController.TryGetPoint(pos, out var point) && point != overlapedPoint.Value)
		{
			AtOverlapedPointChange(point);
		}
	}

	private void AtOverlapedPointChange(Point? newPoint)
	{
		List<MergeAllownEffect> activeElements = mergeEffectCreator.Pool.ActiveElements;
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
		if (!(newPoint == startPoint) && onEnterIcon)
		{
			mergeEffectCreator.Pool.Pop().transform.position = TileController.GetPosition(newPoint.Value);
			startPoint = newPoint.Value;
		}
	}

	private void AtEndsDrag(GameItem obj)
	{
		mergeEffectCreator.Pool.ActiveElements.ForEach(delegate(MergeAllownEffect x)
		{
			x.Hide();
		});
		SetSubscribeDrag(subscribe: false);
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		if (motionController.IsDragging && IsInteractable() && highlightedOverlay != null)
		{
			overlay.sprite = highlightedOverlay;
		}
		onEnterIcon = true;
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (IsInteractable())
		{
			overlay.sprite = defaultOverlay;
		}
		onEnterIcon = false;
	}
}
