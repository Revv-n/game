using DG.Tweening;
using UnityEngine;

namespace StripClub.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class AnimatedUIComponent : MonoBehaviour
{
	[SerializeField]
	private float _duration = 1f;

	[SerializeField]
	private Ease _ease = Ease.OutBack;

	[SerializeField]
	private Vector3 _startPositionFarness;

	[SerializeField]
	private float _startDelay;

	[SerializeField]
	private float _startAnimationAlpha;

	private CanvasGroup _canvasGroup;

	private Sequence _animation;

	private Vector3 _startPosition;

	private float _startAlpha;

	private RectTransform _transform;

	private void Awake()
	{
		Init();
	}

	private void OnDisable()
	{
		_animation?.Kill();
	}

	private void OnEnable()
	{
		_animation = DOTween.Sequence().OnStart(ResetAnimationProps).SetDelay(_startDelay)
			.Append(DOTweenModuleUI.DOAnchorPos(_transform, _startPosition, _duration).SetEase(_ease))
			.Join(DOTweenModuleUI.DOFade(_canvasGroup, _startAlpha, _duration).SetEase(_ease));
	}

	public void Init()
	{
		_transform = GetComponent<RectTransform>();
		_canvasGroup = GetComponent<CanvasGroup>();
		_startAlpha = _canvasGroup.alpha;
		_startPosition = _transform.anchoredPosition;
	}

	private void ResetAnimationProps()
	{
		base.transform.localPosition += _startPositionFarness;
		_canvasGroup.alpha = _startAnimationAlpha;
	}
}
