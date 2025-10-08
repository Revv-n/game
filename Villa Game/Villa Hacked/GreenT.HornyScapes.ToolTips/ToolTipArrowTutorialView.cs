using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipArrowTutorialView : ToolTipView<ToolTipArrowTutorialSettings>
{
	[SerializeField]
	private CanvasGroup myCanvasGroup;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private ParticleSystem click;

	private Sequence showSequence;

	private Vector3 startPosition;

	private Vector3 startScale;

	private float startAlpha;

	private Vector3 endPosition;

	private Ease moveToEndEasy = Ease.InOutQuart;

	private float durationToEndPos = 1f;

	private Vector3 downStateClickScale = new Vector3(0.95f, 0.95f, 0.95f);

	private Ease downStateClickEasy = Ease.OutBack;

	private float downStateClickDuration = 0.1f;

	private Vector3 upStateClickScale = Vector3.one;

	private Ease upStateClickEasy = Ease.OutBack;

	private float upStateClickDuration = 0.2f;

	public override void Set(ToolTipArrowTutorialSettings settings)
	{
		base.Set(settings);
		base.RectTransform.anchoredPosition = settings.ToolTipPosition;
		InitIcon(settings.Rotation);
		InitAnimation(settings.ToolTipPosition, (!settings.MoveToEndPos) ? 1 : 0, base.transform.localScale, settings.EndPosition);
	}

	private void InitIcon(Vector3 rotation)
	{
		icon.gameObject.SetActive(value: true);
		base.RectTransform.localRotation = Quaternion.Euler(rotation);
	}

	public override void Display(bool display)
	{
		if (display)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void Show()
	{
		showSequence = ShowAnimation();
		base.Display(display: true);
	}

	private void Hide()
	{
		if (showSequence != null && showSequence.IsPlaying())
		{
			showSequence.Kill();
		}
		base.Display(display: false);
	}

	private void InitAnimation(Vector3 startPos, float startAlpha, Vector3 startScale, Vector3 endPosition)
	{
		startPosition = startPos;
		this.startAlpha = startAlpha;
		this.startScale = startScale;
		this.endPosition = endPosition;
	}

	private void ResetAnimation()
	{
		((Component)(object)click).gameObject.SetActive(value: false);
		base.RectTransform.anchoredPosition = startPosition;
		myCanvasGroup.alpha = startAlpha;
		base.transform.localScale = startScale;
	}

	private Sequence ShowAnimation()
	{
		ResetAnimation();
		Sequence sequence = DOTween.Sequence();
		if (base.Source.MoveToEndPos)
		{
			sequence.Append(DOTweenModuleUI.DOAnchorPos(base.RectTransform, endPosition, durationToEndPos).SetEase(moveToEndEasy)).Join(DOTweenModuleUI.DOFade(myCanvasGroup, 1f, durationToEndPos));
		}
		if (base.Source.PlayClick)
		{
			sequence.Append(base.transform.DOScale(downStateClickScale, downStateClickDuration).SetEase(downStateClickEasy)).AppendCallback(OnDownState);
			sequence.Append(base.transform.DOScale(upStateClickScale, upStateClickDuration).SetEase(upStateClickEasy)).AppendCallback(OnUpState);
		}
		return sequence.SetLoops(-1).SetDelay(1f);
		void OnDownState()
		{
			((Component)(object)click).gameObject.SetActive(value: true);
			click.Play();
		}
		void OnUpState()
		{
			((Component)(object)click).gameObject.SetActive(value: false);
			click.Stop();
		}
	}

	protected virtual void OnDisable()
	{
		showSequence.Kill();
	}
}
