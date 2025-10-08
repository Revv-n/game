using System;
using GreenT.HornyScapes;
using UniRx;
using UnityEngine;
using Zenject;

namespace Merge;

public class PCUserInputDetector : UserInputDetector
{
	private Subject<UserInputDetector> onEsc = new Subject<UserInputDetector>();

	private Subject<UserInputDetector> onSpace = new Subject<UserInputDetector>();

	private GameStarter gameStarter;

	public virtual IObservable<UserInputDetector> OnEsc()
	{
		return onEsc.AsObservable();
	}

	public virtual IObservable<UserInputDetector> OnSpace()
	{
		return onSpace.AsObservable();
	}

	[Inject]
	public void Init(GameStarter gameStarter)
	{
		this.gameStarter = gameStarter;
	}

	protected virtual void Update()
	{
		if (!gameStarter.IsGameActive.Value)
		{
			return;
		}
		if (Input.mousePresent)
		{
			base.PointerScreenPosition = Input.mousePosition;
			if (base.IsPointerDown)
			{
				base.PointerShift = base.PreviousScreenPosition - base.PointerScreenPosition;
				if (Mathf.Abs(base.PointerShift.sqrMagnitude) > float.Epsilon)
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
			if (Input.GetMouseButtonDown(0))
			{
				base.IsPointerDown = true;
				base.PointerDownScreenPosition = base.PointerScreenPosition;
				base.PreviousScreenPosition = base.PointerScreenPosition;
				PointerDown();
			}
			if (Input.GetMouseButtonUp(0))
			{
				PointerUp();
				base.IsPointerDown = false;
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
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			EscPressed();
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SpacePressed();
		}
	}

	protected void EscPressed()
	{
		onEsc.OnNext(this);
	}

	protected void SpacePressed()
	{
		onSpace.OnNext(this);
	}
}
