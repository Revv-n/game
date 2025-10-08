using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Views;

public sealed class BlockedLevelAnimator : MonoBehaviour
{
	[SerializeField]
	private GameObject _targetView;

	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private Vector3 _maxScale;

	[SerializeField]
	private float _maxScaleDuration = 0.25f;

	[SerializeField]
	private float _pingTime = 0.2f;

	[SerializeField]
	private float _delayTme = 0.5f;

	[SerializeField]
	private Ease _ease = Ease.InOutSine;

	private Sequence _blockedTween;

	public void Prepare()
	{
		base.transform.localScale = Vector3.one;
		_canvasGroup.alpha = 1f;
	}

	public void Play()
	{
		if (_targetView.activeSelf)
		{
			Transform transform = _targetView.transform;
			_blockedTween?.Kill();
			_blockedTween = DOTween.Sequence().Append(transform.DOScale(_maxScale, _maxScaleDuration).SetEase(_ease)).Append(transform.DOScale(1f, _pingTime).SetEase(_ease))
				.Append(transform.DOScale(_maxScale, _maxScaleDuration).SetEase(_ease))
				.Append(transform.DOScale(1f, _pingTime).SetEase(_ease))
				.Append(transform.DOScale(1f, _delayTme))
				.OnKill(delegate
				{
					transform.DOScale(1f, 0f);
				});
		}
	}

	public void Stop()
	{
		_blockedTween?.Kill();
		_blockedTween = null;
	}

	private void OnDisable()
	{
		Stop();
	}
}
