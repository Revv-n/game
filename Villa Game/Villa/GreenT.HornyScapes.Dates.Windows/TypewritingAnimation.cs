using DG.Tweening;
using GreenT.HornyScapes.Animations;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.Dates.Windows;

public class TypewritingAnimation : GreenT.HornyScapes.Animations.Animation
{
	[SerializeField]
	private int _loops;

	[SerializeField]
	private float _duration;

	[SerializeField]
	private string _targetString;

	[SerializeField]
	private TextMeshProUGUI _targetText;

	public override void ResetToAnimStart()
	{
		_targetText.text = string.Empty;
	}

	public override Sequence Play()
	{
		return base.Play().Append(Typewrite()).SetLoops(_loops);
	}

	private Tween Typewrite()
	{
		string text = string.Empty;
		return DOTween.To(() => text, delegate(string x)
		{
			text = x;
		}, _targetString, _duration).OnUpdate(delegate
		{
			_targetText.text = text;
		});
	}
}
