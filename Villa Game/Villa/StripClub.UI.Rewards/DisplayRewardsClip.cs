using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Presents.Models;
using GreenT.HornyScapes.Sounds;
using GreenT.HornyScapes.Subscription;
using StripClub.Extensions;
using StripClub.Model;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Rewards;

public class DisplayRewardsClip : Clip
{
	private string SubscriptionTitleKey = "ui.subscription.daily.";

	[Inject]
	private IAudioPlayer audioPlayer;

	[SerializeField]
	private SoundSO cardSlapSound;

	[SerializeField]
	private List<RewardLineView> rewardLineView;

	[SerializeField]
	private List<Button> hideObjs;

	[SerializeField]
	private GameObject hideBottom;

	[SerializeField]
	private Transform _subscriptionPanel;

	[SerializeField]
	private MonoTimer _subscriptionTime;

	[SerializeField]
	private LocalizedTextMeshPro _subscriptionTitle;

	[SerializeField]
	private float alphaDuration = 0.5f;

	[Header("Три варианта под 3-4-5 карточек")]
	[SerializeField]
	private float scaleCoefOnlyOneLine3 = 1f;

	[SerializeField]
	private float scaleCoefOnlyOneLine4 = 1f;

	[SerializeField]
	private float scaleCoefOnlyOneLine5 = 1f;

	[SerializeField]
	private float scaleCoefLines = 0.57f;

	[SerializeField]
	private float scaleDuration = 0.6f;

	[SerializeField]
	private float scalePower = 8f;

	[SerializeField]
	private float moveDuration = 0.6f;

	[SerializeField]
	private float positionRandomPower = 5f;

	[SerializeField]
	private float timeBetweenCards = 0.05f;

	[SerializeField]
	private float randomTimeBetweenCards = 0.05f;

	private TimeHelper _timeHelper;

	private LinkedContent reward;

	private List<GameObject> cards = new List<GameObject>();

	private int cardCount;

	[Inject]
	private void Construct(TimeHelper timeHelper)
	{
		_timeHelper = timeHelper;
	}

	private void OnValidate()
	{
		if (cardSlapSound == null)
		{
			Debug.LogError("Lootboxes: AllReward has empty audio");
		}
	}

	public void Init(LinkedContent reward, int cardCount)
	{
		this.reward = reward;
		this.cardCount = cardCount;
		_subscriptionPanel.gameObject.SetActive(value: false);
		SetObjsState(isActive: true);
	}

	public void Init(LinkedContent reward, int cardCount, SubscriptionModel subscriptionModel)
	{
		this.reward = reward;
		this.cardCount = cardCount;
		_subscriptionPanel.gameObject.SetActive(value: true);
		_subscriptionTime.Init(subscriptionModel.Duration, _timeHelper.UseCombineFormat);
		_subscriptionTitle.Init($"{SubscriptionTitleKey}{subscriptionModel.BaseID}");
		SetObjsState(isActive: true);
	}

	public void LvlCompletedInit(LinkedContent reward, int cardCount)
	{
		this.reward = reward;
		this.cardCount = cardCount;
		SetObjsState(isActive: false);
	}

	private void SetObjsState(bool isActive)
	{
		foreach (Button hideObj in hideObjs)
		{
			hideObj.gameObject.SetActive(isActive);
		}
	}

	public override void Play()
	{
		hideBottom.SetActive(value: false);
		base.gameObject.SetActive(value: true);
		cards.Clear();
		ResetLines();
		int currentCard = 0;
		Display<CardLinkedContent>(ref currentCard);
		Display<CurrencyLinkedContent>(ref currentCard);
		Display<MergeItemLinkedContent>(ref currentCard);
		Display<SkinLinkedContent>(ref currentCard);
		Display<DecorationLinkedContent>(ref currentCard);
		Display<BattlePassLevelLinkedContent>(ref currentCard);
		Display<BoosterLinkedContent>(ref currentCard);
		Display<PresentLinkedContent>(ref currentCard);
		PlayAnim();
		audioPlayer.PlayOneShotAudioClip2D(cardSlapSound.Sound);
	}

	public void Display<T>(ref int currentCard) where T : LinkedContent
	{
		for (T next = reward.GetNext<T>(checkThis: true); next != null; next = next.GetNext<T>())
		{
			int lineNumber = GetLineNumber(currentCard);
			currentCard++;
			if (lineNumber == -1)
			{
				break;
			}
			rewardLineView[lineNumber].gameObject.SetActive(value: true);
			MonoBehaviour monoBehaviour = rewardLineView[lineNumber].DisplayContent(next);
			SetupView(monoBehaviour.gameObject);
		}
	}

	private void ResetLines()
	{
		foreach (RewardLineView item in rewardLineView)
		{
			item.gameObject.SetActive(value: false);
		}
	}

	private int GetLineNumber(int current)
	{
		if (cardCount <= rewardLineView[0].MaxInLine)
		{
			return 0;
		}
		int num = (current - cardCount % 2) / (cardCount / rewardLineView.Count);
		if (rewardLineView[num].IsFree)
		{
			return num;
		}
		return -1;
	}

	private void SetupView(GameObject card)
	{
		card.transform.localScale = SetScale();
		card.GetComponent<CanvasGroup>().alpha = 0f;
		cards.Add(card);
	}

	private Vector3 SetScale()
	{
		Vector3 result = Vector3.one * scaleCoefOnlyOneLine3;
		if (cardCount <= 3)
		{
			return Vector3.one * scaleCoefOnlyOneLine3;
		}
		if (cardCount <= 4)
		{
			return Vector3.one * scaleCoefOnlyOneLine4;
		}
		if (cardCount <= 5)
		{
			return Vector3.one * scaleCoefOnlyOneLine5;
		}
		if (cardCount > rewardLineView[0].MaxInLine)
		{
			result = Vector3.one * scaleCoefLines;
		}
		return result;
	}

	public void PlayAnim()
	{
		Queue<GameObject> randomCards = new Queue<GameObject>(cards.OrderBy((GameObject a) => Guid.NewGuid()));
		Observable.Interval(TimeSpan.FromSeconds(timeBetweenCards + UnityEngine.Random.Range(0f, randomTimeBetweenCards))).TakeUntilDisable(this).Subscribe(delegate
		{
			if (randomCards.Count > 0)
			{
				GameObject gameObject = randomCards.Dequeue();
				AnimateAlpha(gameObject, alphaDuration);
				AnimateScale(gameObject, scaleDuration, scalePower);
				AnimatePosition(gameObject, moveDuration, positionRandomPower);
				gameObject.SetActive(value: true);
			}
		})
			.AddTo(this);
	}

	private void AnimateAlpha(GameObject card, float duration)
	{
		CanvasGroup component = card.GetComponent<CanvasGroup>();
		component.alpha = 0f;
		component.DOFade(1f, duration);
	}

	private void AnimateScale(GameObject card, float duration, float power)
	{
		Vector3 localScale = card.transform.localScale;
		card.transform.localScale *= power;
		card.transform.DOScale(localScale, duration);
	}

	private void AnimatePosition(GameObject card, float duration, float power)
	{
		Vector3 localPosition = card.transform.localPosition;
		Vector2 vector = localPosition * power;
		card.transform.localPosition += new Vector3(vector.x, vector.y, 0f);
		card.transform.DOLocalMove(localPosition, duration);
	}

	private void OnDisable()
	{
		foreach (RewardLineView item in rewardLineView)
		{
			item.Clear();
		}
		hideBottom.SetActive(value: true);
		SetObjsState(isActive: true);
	}
}
