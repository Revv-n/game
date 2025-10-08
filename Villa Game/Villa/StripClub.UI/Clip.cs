using System;
using UniRx;
using UnityEngine;

namespace StripClub.UI;

public abstract class Clip : MonoBehaviour
{
	private Subject<Clip> onEnd = new Subject<Clip>();

	public IObservable<Clip> OnEnd => onEnd.AsObservable();

	public virtual void Stop()
	{
		onEnd.OnNext(this);
	}

	public abstract void Play();

	protected virtual void OnDestroy()
	{
		onEnd.OnCompleted();
		onEnd.Dispose();
	}
}
