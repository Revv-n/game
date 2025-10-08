using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLerpAnimation : Animation
{
	public TextLerpAnimationSettings AnimationSettings;

	[SerializeField]
	private TextMeshProUGUI _text;

	private void OnValidate()
	{
		if (_text == null)
		{
			_text = GetComponent<TextMeshProUGUI>();
		}
	}

	public override Sequence Play()
	{
		ResetToAnimStart();
		int value = AnimationSettings.From;
		TweenerCore<int, int, NoOptions> t = DOTween.To(() => value, delegate(int x)
		{
			value = x;
		}, AnimationSettings.To, AnimationSettings.Duration).OnUpdate(delegate
		{
			_text.text = string.Format(AnimationSettings.FormatedString, value);
		});
		return base.Play().Append(t);
	}

	public override void ResetToAnimStart()
	{
		_text.text = AnimationSettings.From.ToString();
	}
}
