using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class SpriteBlendingAnimation : Animation
{
	public SpriteRenderer SpriteRenderer;

	public Texture2D TargetSprite;

	public float Duration;

	public float Target;

	private const string LerpFloatId = "_Lerp_Fade_1";

	private const string SpriteTextureId = "_NewTex_1";

	public override Sequence Play()
	{
		return base.Play().Append(SpriteRenderer.material.DOFloat(Target, "_Lerp_Fade_1", Duration));
	}

	public override void Init()
	{
		base.Init();
		SpriteRenderer.material.SetFloat("_Lerp_Fade_1", 0f);
		SpriteRenderer.material.SetTexture("_NewTex_1", TargetSprite);
	}

	public override void ResetToAnimStart()
	{
		SpriteRenderer.material.SetFloat("_Lerp_Fade_1", 0f);
		SpriteRenderer.material.SetTexture("_NewTex_1", TargetSprite);
	}
}
