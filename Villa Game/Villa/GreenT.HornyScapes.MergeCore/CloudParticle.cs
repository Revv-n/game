using System;
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
	private GreenT.HornyScapes.Animations.Animation openAnimation;

	private CompositeDisposable streams = new CompositeDisposable();

	private void OnValidate()
	{
		if (system == null)
		{
			system = GetComponent<ParticleSystem>();
		}
	}

	public void Play()
	{
		system.Stop();
		system.Play();
		openAnimation.OnAnimationEnd.First().Subscribe(delegate
		{
			Display(display: false);
		}).AddTo(streams);
	}

	private void OnDisable()
	{
		streams?.Clear();
		Display(display: false);
	}

	public void Dispose()
	{
		streams.Dispose();
	}
}
