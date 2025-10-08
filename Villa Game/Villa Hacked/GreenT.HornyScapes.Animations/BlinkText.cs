using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class BlinkText : Animation
{
	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private float blinkDuration = 1f;

	[SerializeField]
	private Color textBlinkColor = Color.black;

	[SerializeField]
	private float hideDuration = 0.2f;

	private Color textColor;

	public override void Init()
	{
		base.Init();
		textColor = text.color;
	}

	public override Sequence Play()
	{
		ResetToAnimStart();
		return TextBlink();
	}

	public override void ResetToAnimStart()
	{
		text.color = textColor;
	}

	public void Stop(TweenCallback onComplete = null)
	{
		DOTweenModuleUI.DOFade(text, 0f, hideDuration).OnComplete(onComplete);
		base.Stop();
	}

	private Sequence TextBlink()
	{
		return base.Play().SetLoops(-1, LoopType.Yoyo).Append(DOTweenModuleUI.DOColor(text, textBlinkColor, blinkDuration));
	}
}
