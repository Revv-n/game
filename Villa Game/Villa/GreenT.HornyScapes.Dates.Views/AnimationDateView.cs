using Spine;
using Spine.Unity;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Dates.Views;

public sealed class AnimationDateView : MonoView
{
	[SerializeField]
	private SkeletonGraphic _skeleton;

	public global::Spine.AnimationState AnimationState => _skeleton.AnimationState;

	public TrackEntry SetAnimation(string animationId, bool isLooped)
	{
		return _skeleton.AnimationState.SetAnimation(0, animationId, isLooped);
	}

	public TrackEntry AddAnimation(string animationId, bool isLooped)
	{
		return _skeleton.AnimationState.AddAnimation(0, animationId, isLooped, 0f);
	}

	public float GetAnimationDuration(string animationId)
	{
		if (string.IsNullOrEmpty(animationId))
		{
			return 0f;
		}
		return _skeleton.SkeletonData.FindAnimation(animationId)?.Duration ?? 0f;
	}

	public float GetRemainingAnimationTime()
	{
		if (_skeleton == null || _skeleton.AnimationState == null)
		{
			return 0f;
		}
		TrackEntry current = _skeleton.AnimationState.GetCurrent(0);
		if (current == null || current.Animation == null)
		{
			return 0f;
		}
		float timeScale = current.TimeScale;
		if (timeScale <= 0f)
		{
			return 0f;
		}
		if (current.AnimationTime < 0f)
		{
			return (0f - current.AnimationTime + current.Animation.Duration) / timeScale;
		}
		if (current.Loop)
		{
			float num = current.AnimationTime % current.Animation.Duration;
			return (current.Animation.Duration - num) / timeScale;
		}
		float b = current.Animation.Duration - current.AnimationTime;
		return Mathf.Max(0f, b) / timeScale;
	}
}
