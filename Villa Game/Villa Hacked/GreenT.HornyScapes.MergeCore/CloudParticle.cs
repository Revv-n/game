using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using StripClub.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class CloudParticle : MonoView, IDisposable
{
	[SerializeField]
	private ParticleSystem system;

	[SerializeField]
	private Animation openAnimation;

	private CompositeDisposable streams = new CompositeDisposable();

	private void OnValidate()
	{
		if ((UnityEngine.Object)(object)system == null)
		{
			system = GetComponent<ParticleSystem>();
		}
	}

	public void Play()
	{
		system.Stop();
		system.Play();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Animation>(Observable.First<Animation>(openAnimation.OnAnimationEnd), (Action<Animation>)delegate
		{
			Display(display: false);
		}), (ICollection<IDisposable>)streams);
	}

	private void OnDisable()
	{
		CompositeDisposable obj = streams;
		if (obj != null)
		{
			obj.Clear();
		}
		Display(display: false);
	}

	public void Dispose()
	{
		streams.Dispose();
	}
}
