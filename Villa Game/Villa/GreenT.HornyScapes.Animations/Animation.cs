using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public abstract class Animation : MonoBehaviour, IAnimation
{
	[SerializeField]
	[Header("ID анимации используется только для виду. Назначается контроллером анимации. Иначе - тобой")]
	protected int AnimationId;

	public float Delay;

	protected Subject<Animation> onAnimationEnd = new Subject<Animation>();

	protected Sequence sequence;

	[field: SerializeField]
	public AnimationGroupID GroupID { get; private set; }

	public int ID => AnimationId;

	public IObservable<Animation> OnAnimationEnd => onAnimationEnd.AsObservable();

	public virtual Sequence Play()
	{
		Stop();
		sequence = DOTween.Sequence().SetDelay(Delay).OnStepComplete(Complete);
		return sequence;
	}

	protected virtual void Complete()
	{
		onAnimationEnd.OnNext(this);
	}

	public virtual void Init()
	{
		Stop();
	}

	public abstract void ResetToAnimStart();

	public virtual void Stop()
	{
		if (sequence.IsActive())
		{
			sequence.Kill();
			sequence = null;
			Complete();
		}
	}

	private void OnDisable()
	{
		Stop();
	}
}
