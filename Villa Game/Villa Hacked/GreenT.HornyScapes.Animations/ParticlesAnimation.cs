using System;
using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class ParticlesAnimation : Animation
{
	public GameObject ParticlesContainer;

	public ParticleSystem[] ParticleSystems;

	public override Sequence Play()
	{
		return base.Play().AppendCallback(PlayParticles);
	}

	public override void ResetToAnimStart()
	{
		throw new NotImplementedException();
	}

	private void PlayParticles()
	{
		ParticleSystem[] particleSystems = ParticleSystems;
		for (int i = 0; i < particleSystems.Length; i++)
		{
			particleSystems[i].Play();
		}
		if (!(ParticlesContainer == null))
		{
			ParticlesContainer.SetActive(value: false);
			ParticlesContainer.SetActive(value: true);
		}
	}
}
