using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Merge;

public class MergeAllownEffect : MonoBehaviour, IPoolReturner, IPoolPopListener
{
	[SerializeField]
	private float showTime = 0.4f;

	[SerializeField]
	private float targetScale = 3f;

	[SerializeField]
	private List<Transform> raysObjects;

	[SerializeField]
	private List<Transform> cirecleObjects;

	private bool isHiding;

	private Tween showTween;

	public Action ReturnInPool { get; set; }

	void IPoolPopListener.OnPopFromPool()
	{
		base.transform.localScale = Vector3.zero;
		showTween = base.transform.DOScale(targetScale, showTime);
	}

	public void SetCircleSize(float val)
	{
		cirecleObjects.ForEach(delegate(Transform x)
		{
			x.SetScale(val);
		});
	}

	public void SetRaySize(float val)
	{
		raysObjects.ForEach(delegate(Transform x)
		{
			x.SetScale(val);
		});
	}

	public void Hide()
	{
		if (!isHiding)
		{
			isHiding = true;
			showTween?.Kill();
			base.transform.localScale = Vector3.one;
			showTween = base.transform.DOScale(Vector3.zero, showTime);
			Tween tween = showTween;
			tween.onComplete = (TweenCallback)Delegate.Combine(tween.onComplete, (TweenCallback)delegate
			{
				ReturnInPool();
				isHiding = false;
			});
		}
	}
}
