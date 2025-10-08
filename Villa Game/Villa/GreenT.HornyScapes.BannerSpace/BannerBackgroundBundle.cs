using UnityEngine;

namespace GreenT.HornyScapes.BannerSpace;

[CreateAssetMenu(fileName = "Banner", menuName = "StripClub/Bank/BannerBackground")]
public class BannerBackgroundBundle : ScriptableObject
{
	public Sprite Base;

	public BannerAnimatedBackground AnimatedBackground;

	public Vector2 TextPosition;
}
