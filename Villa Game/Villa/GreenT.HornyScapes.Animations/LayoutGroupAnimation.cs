using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class LayoutGroupAnimation : Animation
{
	[SerializeField]
	private Animation animation;

	protected void OnValidate()
	{
		if (!animation)
		{
			Debug.LogWarning(GetType().Name + ": animation is empty", this);
		}
	}

	public override void Init()
	{
		animation.Init();
	}

	public override Sequence Play()
	{
		animation.ResetToAnimStart();
		return animation.Play();
	}

	public override void Stop()
	{
		animation.Stop();
	}

	public void SetDelay(float delay)
	{
		animation.Delay = delay;
	}

	public override void ResetToAnimStart()
	{
		animation.ResetToAnimStart();
	}
}
