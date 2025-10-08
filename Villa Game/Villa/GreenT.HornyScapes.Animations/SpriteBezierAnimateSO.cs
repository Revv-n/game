using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

[CreateAssetMenu(fileName = "SpriteBezierPreset", menuName = "GreenT/Animations/Presets/SpriteBezier")]
public class SpriteBezierAnimateSO : ScriptableObject
{
	[SerializeField]
	private float delay;

	[SerializeField]
	private float duration = 1f;

	[SerializeField]
	private float startAlpha = 0.7f;

	[SerializeField]
	private TweenParam endAlpha;

	public Ease Ease = Ease.InCubic;

	public float Delay => delay;

	public float Duration => duration;

	public float StartAlpha => startAlpha;

	public TweenParam EndAlpha => endAlpha;
}
