using DG.Tweening;
using GreenT.HornyScapes.Sounds;
using StripClub.Model;
using StripClub.Model.Cards;
using TMPro;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Rewards;

public class OpenCardsClip : Clip
{
	[SerializeField]
	private TextMeshProUGUI cardsLeftText;

	[Inject]
	private IAudioPlayer audioPlayer;

	[Header("Animation settings")]
	[SerializeField]
	private float startCardRotation = -45f;

	[SerializeField]
	private float animationDuration = 0.3f;

	[SerializeField]
	private RectTransform card;

	[SerializeField]
	private RectTransform targetCard;

	[SerializeField]
	private Ease ease;

	[SerializeField]
	private SoundSO flyCardSound;

	[SerializeField]
	private SoundSO flyLegendaryCardSound;

	[SerializeField]
	private SoundSO _flyMythicCardSound;

	private int currentDrops;

	private Vector3 startPosition;

	private Vector3 startSizeDelta;

	private LinkedContent currentReward;

	public void Init(LinkedContent currentReward, int dropsLeft)
	{
		this.currentReward = currentReward;
		DisplayDropsCountLeft(dropsLeft);
	}

	public override void Play()
	{
		DisplayDropsCountLeft(currentDrops);
		PlaySound();
		base.gameObject.SetActive(value: true);
		TakeCard();
	}

	public void TakeCard()
	{
		card.gameObject.SetActive(value: true);
		card.localPosition = startPosition;
		card.sizeDelta = startSizeDelta;
		card.localRotation = Quaternion.Euler(new Vector3(0f, 0f, startCardRotation));
		card.DOSizeDelta(targetCard.sizeDelta, animationDuration).SetEase(ease);
		card.DORotate(targetCard.localRotation.eulerAngles, animationDuration).SetEase(ease);
		card.DOMove(targetCard.position, animationDuration).SetEase(ease).OnComplete(delegate
		{
			base.gameObject.SetActive(value: false);
			Stop();
		});
	}

	private void OnValidate()
	{
		if (flyCardSound == null || flyLegendaryCardSound == null || _flyMythicCardSound == null)
		{
			Debug.LogError("Lootboxes: OpenCard has empty audio");
		}
	}

	private void Awake()
	{
		startPosition = card.localPosition;
		startSizeDelta = card.sizeDelta;
	}

	private void PlaySound()
	{
		SoundSO soundSO = flyCardSound;
		if (currentReward is CardLinkedContent cardLinkedContent)
		{
			soundSO = cardLinkedContent.Card.Rarity switch
			{
				Rarity.Legendary => flyLegendaryCardSound, 
				Rarity.Mythic => _flyMythicCardSound, 
				_ => flyCardSound, 
			};
		}
		audioPlayer?.PlayAudioClip2D(soundSO.Sound);
	}

	private void DisplayDropsCountLeft(int dropsLeft)
	{
		currentDrops = dropsLeft;
		cardsLeftText.text = dropsLeft.ToString();
	}
}
