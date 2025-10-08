using GreenT.HornyScapes.Animations;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.RoomObjects;

[CreateAssetMenu(fileName = "NewAnimatedObjectConfig", menuName = "DL/Configs/Meta/AnimatedObjectConfig")]
public class AnimatedObjectConfig : BaseObjectConfig
{
	[field: SerializeField]
	public GreenT.HornyScapes.Animations.Animation ShowAnimation { get; set; }

	[field: SerializeField]
	public Vector3 GlowPosition { get; set; }

	[field: SerializeField]
	public int SortingOrder { get; set; }

	[field: SerializeField]
	public RoomObjectAnimationSettings AnimationSettings { get; set; }
}
