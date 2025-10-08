using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Merge.MotionDesign;

public class Room0TrashEffect : RoomObjectEffect
{
	[Serializable]
	public class FogInfo
	{
		public SpriteRenderer fog;

		public float delay;

		public float scale;

		public Tween BuildTween(float time, Ease fogSceleEase, Ease fogfadeEase)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Join(fog.transform.DOScale(scale, time).SetEase(fogSceleEase));
			sequence.Join(fog.DOFade(0f, time).SetEase(fogfadeEase));
			return sequence;
		}
	}

	[SerializeField]
	private float fogInTime;

	[SerializeField]
	private float itemsBecomesFade;

	[SerializeField]
	private float itemsFadeTime;

	[SerializeField]
	private Ease fogSceleEase;

	[SerializeField]
	private Ease fogfadeEase;

	[SerializeField]
	private List<FogInfo> fogInfos;

	[SerializeField]
	private List<SpriteRenderer> objects;

	private Tween tween;

	public override void Play(params SpriteRenderer[] roView)
	{
		foreach (SpriteRenderer item in roView)
		{
			SpriteRenderer spriteRenderer = objects.First((SpriteRenderer x) => x.sprite.name == item.sprite.name);
			spriteRenderer.transform.position = item.transform.position;
			spriteRenderer.SetOrder(item);
		}
		Sequence sequence = DOTween.Sequence();
		foreach (SpriteRenderer @object in objects)
		{
			sequence.Join(@object.DOFade(0f, itemsFadeTime));
		}
		Sequence sequence2 = DOTween.Sequence();
		foreach (FogInfo fogInfo in fogInfos)
		{
			sequence2.Insert(fogInfo.delay, fogInfo.BuildTween(fogInTime, fogSceleEase, fogfadeEase));
		}
		sequence2.Insert(itemsBecomesFade, sequence);
		sequence2.onComplete = (TweenCallback)Delegate.Combine(sequence2.onComplete, (TweenCallback)delegate
		{
			UnityEngine.Object.Destroy(base.gameObject);
		});
		tween = sequence2;
	}
}
