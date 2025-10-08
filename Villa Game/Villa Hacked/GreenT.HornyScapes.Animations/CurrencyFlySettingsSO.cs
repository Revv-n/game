using DG.Tweening;
using Merge.MotionDesign;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

[CreateAssetMenu(fileName = "CurrencyFlyPreset", menuName = "GreenT/Animations/Presets/CurrencyFly")]
public class CurrencyFlySettingsSO : ScriptableObject
{
	[Header("Для объекта на поле")]
	public float ScaleToZeroDuration = 0.2f;

	public Ease GhostScaleEase;

	public CurrencyTypeColor CurrencySettings;

	public ParticleSystem DustCloud;

	[Header("Смещения момента увеличения барчика")]
	public float TimeShift = -0.1f;

	[Header("Величина увеличения барчика")]
	public float PunchIconScale = 0.1f;

	[Header("Время анимации увеличения")]
	public float PunchIconDuration = 0.3f;

	[Header("Отставание между монетами")]
	public float DelayBetweenCurrency = 0.08f;

	[Header("% Разброс скейла иконки и трейла в анимации")]
	[Range(0f, 1f)]
	public float AnimationScaleDispersion;

	[Header("Число монет")]
	public int MinCount = 3;

	public int MaxCount = 7;

	[Header("Альфа")]
	public float StartAlpha;

	public TweenParam EndAlpha;
}
