using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Merge.MotionDesign;

public class PulsationTweenBuilder : MonoBehaviour
{
	public float delayTme = 0.5f;

	public float max_scale = 1.05f;

	public float pingTime = 0.2f;

	public float attractionDistance = 0.2f;

	public Ease ease = Ease.InOutSine;

	public Tween BuildTween(Transform tr, Transform target)
	{
		_ = (target.position - tr.position).normalized * attractionDistance;
		TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = tr.DOScale(1f, 1f);
		Sequence seq = DOTween.Sequence();
		seq.Append(tr.DOScale(max_scale, pingTime).SetEase(ease));
		seq.Append(tr.DOScale(1f, pingTime).SetEase(ease));
		seq.Append(tr.DOScale(max_scale, pingTime).SetEase(ease));
		seq.Append(tr.DOScale(1f, pingTime).SetEase(ease));
		seq.Append(tr.DOScale(1f, delayTme));
		seq.SetLoops(-1, LoopType.Restart);
		seq.Pause();
		tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, (TweenCallback)delegate
		{
			seq.Play();
		});
		return seq;
	}
}
