using DG.Tweening;
using GreenT.HornyScapes.ToolTips;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class ArrowAnimationSystem : Animation
{
	[Header("Объекты с сцены")]
	[SerializeField]
	private CanvasGroup myCanvasGroup;

	[SerializeField]
	private RectTransform toolTipTransform;

	[SerializeField]
	private ParticleSystem click;

	[SerializeField]
	private ToolTipArrowTutorialView settingsKeeper;

	[Header("Настройка анимации")]
	[SerializeField]
	private Ease moveToEndEasy = Ease.InOutQuart;

	[SerializeField]
	private float durationToEndPos = 1f;

	[SerializeField]
	private float clickDelay = 1f;

	[SerializeField]
	private float downStateClickScale = 0.8f;

	[SerializeField]
	private Ease downStateClickEasy = Ease.OutBack;

	[SerializeField]
	private float downStateClickDuration = 0.2f;

	[SerializeField]
	private float upStateClickScale = 1f;

	[SerializeField]
	private Ease upStateClickEasy = Ease.OutBack;

	[SerializeField]
	private float upStateClickDuration = 0.4f;

	private Vector3 startPosition;

	private Vector3 startScale;

	private float startAlpha;

	private Vector3 endPosition;

	private ToolTipArrowTutorialSettings settings;

	private Sequence endlessClick;

	public override void Init()
	{
		base.Init();
		settings = settingsKeeper.Source;
		startPosition = settings.ToolTipPosition;
		startAlpha = ((!settings.MoveToEndPos) ? 1 : 0);
		startScale = base.transform.localScale;
		endPosition = settings.EndPosition;
	}

	public override Sequence Play()
	{
		ResetToAnimStart();
		base.Play();
		if (settings.OneMoveEndlessClick)
		{
			PlaySpecialAnimation();
		}
		else
		{
			sequence = PlayDefaultAnimation().SetLoops(-1);
		}
		return sequence;
	}

	private Sequence PlayDefaultAnimation()
	{
		Sequence sequence = DOTween.Sequence();
		if (settings.MoveToEndPos)
		{
			DoLinearMotion(sequence);
		}
		if (settings.PlayClick)
		{
			DoClick(sequence);
		}
		return sequence;
	}

	private Sequence DoLinearMotion(Sequence seq)
	{
		seq.Append(toolTipTransform.DOAnchorPos(endPosition, durationToEndPos).SetEase(moveToEndEasy)).Join(myCanvasGroup.DOFade(1f, durationToEndPos));
		return seq;
	}

	private Sequence DoClick(Sequence seq, bool endless = false)
	{
		seq.Append(base.transform.DOScale(downStateClickScale, downStateClickDuration).SetEase(downStateClickEasy)).AppendCallback(OnDownState);
		seq.Append(base.transform.DOScale(upStateClickScale, upStateClickDuration).SetEase(upStateClickEasy)).AppendCallback(OnUpState);
		return seq.SetDelay(clickDelay).SetLoops(endless ? (-1) : 0);
		void OnDownState()
		{
			click.gameObject.SetActive(value: true);
			click.Play();
		}
		void OnUpState()
		{
			click.gameObject.SetActive(value: false);
			click.Stop();
		}
	}

	private Sequence PlaySpecialAnimation()
	{
		sequence = DoLinearMotion(sequence);
		sequence.AppendCallback(delegate
		{
			DoClick(endlessClick = DOTween.Sequence(), endless: true);
		});
		return sequence;
	}

	public override void Stop()
	{
		base.Stop();
		endlessClick?.Kill();
	}

	public override void ResetToAnimStart()
	{
		click.gameObject.SetActive(value: false);
		toolTipTransform.anchoredPosition = startPosition;
		myCanvasGroup.alpha = startAlpha;
		base.transform.localScale = startScale;
	}
}
