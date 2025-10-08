using DG.Tweening;
using StripClub.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI.Collections;

public class CompletionCardIndicatorView : MonoBehaviour
{
	[SerializeField]
	private Image _arrow;

	[SerializeField]
	private GameObject _rank;

	[SerializeField]
	private float _rankAnimationDuration;

	[SerializeField]
	private float _arrowAnimationDuration;

	[SerializeField]
	private float _startRankScaling;

	[SerializeField]
	private float _arrowHeight;

	private Vector3 _startScale;

	private Vector3 _startPosition;

	private float _startRankAlpha;

	private float _startArrowAlpha;

	private Sequence _animation;

	public void Animate()
	{
	}

	private void Init()
	{
		_arrow.color = TweenHelper.Alpha(_arrow, _startArrowAlpha);
		_rank.transform.localScale = _startScale;
		_arrow.transform.localPosition = _startPosition;
	}
}
