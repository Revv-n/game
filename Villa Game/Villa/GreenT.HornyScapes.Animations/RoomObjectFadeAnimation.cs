using DG.Tweening;
using GreenT.HornyScapes.Meta.Animation;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class RoomObjectFadeAnimation : RelativeTransformAnimation
{
	[SerializeField]
	private RoomObjectAnimation[] roomObjectAnimations;

	public override void Init()
	{
		base.Init();
		roomObjectAnimations = base.transform.parent.parent.parent.parent.GetComponentsInChildren<RoomObjectAnimation>();
	}

	public override Sequence Play()
	{
		RoomObjectAnimation[] array = roomObjectAnimations;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].PLayFadeParticles();
		}
		return base.Play();
	}
}
