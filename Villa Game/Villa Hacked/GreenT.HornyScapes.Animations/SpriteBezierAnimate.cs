using System;
using DG.Tweening;
using GreenT.Utilities;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class SpriteBezierAnimate : MonoBehaviour
{
	[SerializeField]
	private Bezier bezier;

	[SerializeField]
	private SpriteBezierAnimateSO settings;

	private Subject<Unit> animateEnd = new Subject<Unit>();

	private Sequence sequence;

	public float Duration => settings.Duration;

	public IObservable<Unit> OnAnimationEnd => Observable.AsObservable<Unit>((IObservable<Unit>)animateEnd);

	public Sequence Launch(Vector3 from, Vector3 to, Transform flyingObj)
	{
		bezier.SetDestination(from, to);
		return PlayAnimation(from, flyingObj);
	}

	public Sequence Launch(Vector3 from, Vector3 to, SpriteRenderer flyingObj)
	{
		bezier.SetDestination(from, to);
		return PlayAnimation(from, flyingObj);
	}

	private Sequence PlayAnimation(Vector3 from, Transform flyingObj)
	{
		sequence = CreateSequence(from, flyingObj);
		return sequence;
	}

	private Sequence PlayAnimation(Vector3 from, SpriteRenderer flyingObj)
	{
		sequence = CreateSequence(from, flyingObj.transform);
		sequence.Insert(settings.Delay, DOTweenModuleSprite.DOFade(flyingObj, settings.EndAlpha.Value, settings.EndAlpha.Duration));
		sequence.SetEase(settings.Ease);
		EndAnimation(sequence, flyingObj);
		return sequence;
	}

	private Sequence CreateSequence(Vector3 from, Transform flyingObj)
	{
		sequence = DOTween.Sequence();
		Vector3[] points = bezier.Points;
		flyingObj.position = from;
		flyingObj.gameObject.SetActive(value: true);
		sequence.AppendInterval(settings.Delay);
		sequence.Insert(settings.Delay, flyingObj.DOPath(points, settings.Duration));
		return sequence;
	}

	private void EndAnimation(Sequence sequence, SpriteRenderer sprite)
	{
		sequence.onKill = (TweenCallback)Delegate.Combine(sequence.onKill, (TweenCallback)delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			animateEnd.OnNext(Unit.Default);
		});
	}
}
