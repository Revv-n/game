using GreenT.HornyScapes;
using UnityEngine;
using Zenject;

namespace Merge;

public class MobileUserInputDetector : UserInputDetector
{
	private const float DRAG_TRASH_HOLD = 16f;

	private GameStarter gameStarter;

	[Inject]
	public void Init(GameStarter gameStarter)
	{
		this.gameStarter = gameStarter;
	}

	protected virtual void Update()
	{
		if (!gameStarter.IsGameActive.Value || Input.touchCount == 0)
		{
			return;
		}
		Touch touch = Input.GetTouch(0);
		base.PointerScreenPosition = touch.position;
		if (base.IsPointerDown)
		{
			base.PointerShift = touch.deltaPosition;
			if (base.PointerShift.sqrMagnitude > 16f)
			{
				if (!base.IsDragging)
				{
					base.IsDragging = true;
					DragBegin();
				}
				else
				{
					Drag();
				}
			}
		}
		if (touch.phase == TouchPhase.Began)
		{
			base.IsPointerDown = true;
			base.PointerDownScreenPosition = base.PointerScreenPosition;
			base.PreviousScreenPosition = base.PointerScreenPosition;
			PointerDown();
		}
		if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
		{
			base.IsPointerDown = false;
			PointerUp();
			if (base.IsDragging)
			{
				base.IsDragging = false;
				DragEnd();
			}
			else
			{
				Click();
			}
		}
		base.PreviousScreenPosition = base.PointerScreenPosition;
	}
}
