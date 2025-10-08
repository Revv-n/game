using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Animations;

public class ToggleAnimController : AnimationController
{
	[SerializeField]
	private Toggle toggle;

	[SerializeField]
	private Animation selectAnimation;

	[SerializeField]
	private Animation unSelectAnimation;

	public override void Init()
	{
		base.Init();
		selectAnimation.Init();
		unSelectAnimation.Init();
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}

	public override void PlayAllAnimations()
	{
		toggle.OnValueChangedAsObservable().TakeUntilDisable(this).Subscribe(OnSelect)
			.AddTo(this);
	}

	public override void ResetAnimation()
	{
	}

	private void OnSelect(bool value)
	{
		selectAnimation.Stop();
		unSelectAnimation.Stop();
		Animation obj = (value ? selectAnimation : unSelectAnimation);
		obj.ResetToAnimStart();
		obj.Play();
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (toggle == null && TryGetComponent<Toggle>(out var component))
		{
			toggle = component;
		}
	}

	public override bool IsPlaying()
	{
		throw new NotImplementedException();
	}
}
