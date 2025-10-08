using DG.Tweening;
using GreenT.HornyScapes.Animations;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.Animation;

public class RoomObjectAnimation : RelativeTransformAnimation
{
	[SerializeField]
	private GameObject mainParticles;

	[SerializeField]
	private GameObject fadeParticles;

	public override Sequence Play()
	{
		return base.Play().AppendCallback(PlayParticles);
	}

	public void PlayParticles()
	{
		Debug.Log("PlayParticles");
		mainParticles.SetActive(value: false);
		mainParticles.SetActive(value: true);
	}

	public void PLayFadeParticles()
	{
		fadeParticles.SetActive(value: false);
		fadeParticles.SetActive(value: true);
	}
}
