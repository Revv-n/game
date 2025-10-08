using System;
using DG.Tweening;
using GreenT.HornyScapes.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Level.UI;

[Obsolete("Use BattlePasLevelInfoCase")]
public class LevelGauge : MonoBehaviour
{
	[SerializeField]
	private GameObject[] particles;

	[SerializeField]
	private TextMeshProUGUI levelNumber;

	[SerializeField]
	private AnimatedProgress levelProgressBar;

	[SerializeField]
	private Transform fill;

	[SerializeField]
	private Image glow;

	[SerializeField]
	private Image counter;

	[SerializeField]
	private Sprite bgFilledSprite;

	[SerializeField]
	private float readyToSellScale = 1.1f;

	[SerializeField]
	private float glowReadyToSellUnScale = 0.6f;

	[SerializeField]
	private float glowReadyToSellScale = 0.8f;

	[SerializeField]
	private float glowReadyToSellAlpha = 0.8f;

	[SerializeField]
	private float readyToSellScaleTime = 0.4f;

	[SerializeField]
	private float readyToSellUnScaleTime = 0.2f;

	[SerializeField]
	private float fillAnimationTime = 0.2f;

	[SerializeField]
	private float UnFillAnimationTime = 0.2f;

	[SerializeField]
	private float fillScalePower = 1.2f;

	[SerializeField]
	private float fillGlowAlpha = 0.4f;

	private PlayerExperience playerExp;

	private IPlayerExpController playerExpController;

	private Sprite counterUnFilledSprite;

	private Vector3 startScale = Vector3.one;

	private Color startGlow = Color.white;

	private Vector3 fillScale = Vector3.one;

	private Vector3 glowScale = Vector3.one;

	private Sequence animationSequence;

	private Sequence progressSequence;

	[Inject]
	public void Init(PlayerExperience playerExperience, IPlayerExpController playerExpController)
	{
		playerExp = playerExperience;
		this.playerExpController = playerExpController;
	}

	private void Start()
	{
		startScale = base.transform.localScale;
		startGlow = glow.color;
		fillScale = fill.localScale;
		counterUnFilledSprite = counter.sprite;
		glowScale = glow.transform.localScale;
	}

	private void OnEnable()
	{
		TrackProgress();
		levelNumber.text = playerExp.Level.Value.ToString();
		levelProgressBar.AnimateFromCurrent(playerExp.XP.Value, playerExpController.Target.Value);
	}

	private void TrackProgress()
	{
		playerExpController.OnProgressUpdate.TakeUntilDisable(this).Subscribe(delegate(int value)
		{
			SetProgress(value, playerExpController.Target.Value);
		}).AddTo(this);
		levelProgressBar.OnProgressComplete.TakeUntilDisable(this).Subscribe(delegate
		{
			if (playerExpController.IsComplete())
			{
				PlayReadyToSellAnimation();
			}
			else
			{
				PlayDefaultAnimation();
			}
		}).AddTo(this);
	}

	private void SetProgress(int current, int target)
	{
		levelNumber.text = playerExp.Level.Value.ToString();
		levelProgressBar.AnimateFromCurrent(current, target);
		progressSequence?.Kill();
		progressSequence = DOTween.Sequence().Append(base.transform.DOScale(startScale * fillScalePower, fillAnimationTime)).Join(glow.DOFade(fillGlowAlpha, fillAnimationTime));
	}

	private void PlayDefaultAnimation()
	{
		animationSequence?.Kill();
		animationSequence = DOTween.Sequence().Append(base.transform.DOScale(startScale, UnFillAnimationTime)).Join(glow.DOColor(startGlow, UnFillAnimationTime))
			.AppendCallback(SetDefaultValues);
	}

	private void PlayReadyToSellAnimation()
	{
		SetActiveValues();
		animationSequence?.Kill();
		progressSequence?.Kill();
		animationSequence = DOTween.Sequence().Join(glow.transform.DOScale(glowReadyToSellScale, readyToSellScaleTime)).Join(glow.DOFade(1f, readyToSellScaleTime))
			.Join(base.transform.DOScale(readyToSellScale, readyToSellScaleTime))
			.Append(base.transform.DOScale(startScale, readyToSellUnScaleTime))
			.Join(glow.transform.DOScale(glowReadyToSellUnScale, readyToSellUnScaleTime))
			.Join(fill.DOScale(fillScale, readyToSellUnScaleTime))
			.AppendCallback(delegate
			{
				GameObject[] array = particles;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: true);
				}
			});
	}

	private void SetActiveValues()
	{
		counter.sprite = bgFilledSprite;
	}

	private void SetDefaultValues()
	{
		GameObject[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		fill.localScale = fillScale;
		glow.transform.localScale = glowScale;
		counter.sprite = counterUnFilledSprite;
		glow.color = startGlow;
	}
}
