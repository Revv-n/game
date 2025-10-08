using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Merge.MotionDesign;

public class LightningTweenBuilder : MonoBehaviour
{
	[Serializable]
	public struct TargetTweenInfo
	{
		[SerializeField]
		private float size;

		[SerializeField]
		private float time;

		[SerializeField]
		private Ease ease;

		public Tween BuildTween(Transform target)
		{
			Sequence sequence = DOTween.Sequence();
			target.transform.localScale = Vector3.one;
			sequence.Join(target.DOScale(size, time));
			sequence.SetLoops(-1, LoopType.Yoyo);
			sequence.SetEase(ease);
			return sequence;
		}
	}

	[Serializable]
	public struct SizeTweenInfo
	{
		public float StartAlpha;

		public Vector3 StartSize;

		[SerializeField]
		private SpriteRenderer sr;

		[SerializeField]
		private float size;

		[SerializeField]
		private float alpha;

		[SerializeField]
		private float time;

		[SerializeField]
		private Ease ease;

		public SpriteRenderer TweenObject => sr;

		public Tween BuildTween()
		{
			sr.SetAlpha(StartAlpha);
			sr.transform.localScale = StartSize;
			Sequence sequence = DOTween.Sequence();
			sequence.Join(sr.transform.DOScale(size, time));
			sequence.Join(sr.DOFade(alpha, time));
			sequence.SetLoops(-1, LoopType.Yoyo);
			sequence.SetEase(ease);
			return sequence;
		}
	}

	[Serializable]
	public struct RoamTweenInfo
	{
		[SerializeField]
		private Transform roamer;

		[SerializeField]
		private Vector3 position;

		[SerializeField]
		private float time;

		[SerializeField]
		private Ease ease;

		public Vector3 StartPosition => position;

		public Tween BuildTween()
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Join(roamer.DOLocalMove(position, time));
			sequence.SetLoops(-1, LoopType.Yoyo);
			sequence.SetEase(ease);
			return sequence;
		}
	}

	[SerializeField]
	private SizeTweenInfo energyInfo;

	[SerializeField]
	private SizeTweenInfo auraInfo;

	[SerializeField]
	private TargetTweenInfo targetInfo;

	[SerializeField]
	private SizeTweenInfo[] dotsSizeInfo;

	[SerializeField]
	private RoamTweenInfo[] dotsRoamInfo;

	private List<Tween> tweens = new List<Tween>();

	private Transform cacedTarget;

	private bool cacedFlag;

	private void OnValidate()
	{
		energyInfo.StartAlpha = energyInfo.TweenObject.GetAlpha();
		energyInfo.StartSize = energyInfo.TweenObject.transform.localScale;
	}

	public void SetVisible(bool visible)
	{
		if (!visible)
		{
			Vector3 localScale = cacedTarget.localScale;
			tweens.ForEach(delegate(Tween x)
			{
				x.Kill(complete: true);
			});
			tweens.Clear();
			base.gameObject.SetActive(value: false);
			cacedTarget.localScale = localScale;
			cacedTarget.DOScale(1f, 0.5f);
		}
		else
		{
			base.gameObject.SetActive(value: true);
			BuildTween(cacedTarget, cacedFlag);
		}
	}

	public void Kill()
	{
		tweens.ForEach(delegate(Tween x)
		{
			x.Kill(complete: true);
		});
	}

	public void BuildTween(Transform target, bool withEnergy)
	{
		cacedTarget = target;
		cacedFlag = withEnergy;
		tweens.ForEach(delegate(Tween x)
		{
			x.Kill(complete: true);
		});
		tweens.Clear();
		SetDefault();
		if (withEnergy)
		{
			tweens.Add(energyInfo.BuildTween());
		}
		energyInfo.TweenObject.gameObject.SetActive(withEnergy);
		tweens.Add(auraInfo.BuildTween());
		tweens.Add(targetInfo.BuildTween(target));
		SizeTweenInfo[] array = dotsSizeInfo;
		foreach (SizeTweenInfo sizeTweenInfo in array)
		{
			tweens.Add(sizeTweenInfo.BuildTween());
		}
		RoamTweenInfo[] array2 = dotsRoamInfo;
		foreach (RoamTweenInfo roamTweenInfo in array2)
		{
			tweens.Add(roamTweenInfo.BuildTween());
		}
	}

	private void SetDefault()
	{
		Vector3 one = Vector3.one;
		cacedTarget.transform.localScale = one;
		energyInfo.TweenObject.transform.localScale = one;
		auraInfo.TweenObject.transform.localScale = one;
		for (int i = 0; i < dotsSizeInfo.Length; i++)
		{
			dotsSizeInfo[i].TweenObject.SetAlpha(1f);
			dotsSizeInfo[i].TweenObject.transform.localScale = one;
			dotsSizeInfo[i].TweenObject.transform.localPosition = dotsRoamInfo[i].StartPosition;
		}
	}
}
