using System;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Sounds;
using GreenT.HornyScapes.UI;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Rewards.UI;
using StripClub.UI.Rewards.Animation.Settings;
using TMPro;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Rewards;

public class AddSoulsClip : Clip
{
	[Serializable]
	public class RaritySettingsDictionary : SerializableDictionary<Rarity, CardAnimSettings>
	{
	}

	[SerializeField]
	private RewardCardView cardView;

	[SerializeField]
	private TMP_Text quantity;

	[SerializeField]
	private float waitOnTheEnd = 1.8f;

	[SerializeField]
	private CollectionSoundSO audioClips;

	[SerializeField]
	protected AnimatedProgress progressBar;

	[SerializeField]
	private GreenT.HornyScapes.Animations.CardAnimation cardAnimation;

	[Tooltip("Объекты, которые включатся после поворота карточки передней частью")]
	[SerializeField]
	private GameObject[] generalDecorationObjects;

	[Tooltip("Объекты, которые включатся пока она повёрнута рубашкой к нам")]
	[SerializeField]
	private GameObject[] generalDecorationBacksideObjects;

	[SerializeField]
	private StatableComponent frontRarityStatable;

	private IAudioPlayer audioPlayer;

	private CardsCollection cards;

	public CardLinkedContent Source { get; private set; }

	[Inject]
	public void Init(IAudioPlayer audioPlayer, CardsCollection cards)
	{
		this.cards = cards;
		this.audioPlayer = audioPlayer;
	}

	private void OnValidate()
	{
		if (audioClips == null)
		{
			Debug.LogError("Lootboxes: AddSoulsClips has empty audio");
		}
	}

	public override void Play()
	{
		float atPosition = cardAnimation.RotationDuration * 2f;
		cardAnimation.Play().InsertCallback(atPosition, ProgressBarAnimate).OnComplete(Stop);
	}

	private void ProgressBarAnimate()
	{
		IPromote promoteOrDefault = cards.GetPromoteOrDefault(Source.Card);
		if (promoteOrDefault != null)
		{
			progressBar.AnimateFromCurrent(promoteOrDefault.Progress.Value + Source.Quantity, promoteOrDefault.Target, promoteOrDefault.Progress.Value);
		}
	}

	public void Init(CardLinkedContent cardContent)
	{
		Source = cardContent;
		quantity.text = $"+{cardContent.Quantity}";
		ICard card = cardContent.Card;
		cardView.Set(card);
		frontRarityStatable.Set((int)card.Rarity);
		int index = UnityEngine.Random.Range(0, audioClips.Sounds.Count);
		audioPlayer?.PlayAudioClip2D(audioClips.Sounds[index]);
		GreenT.HornyScapes.Animations.CardAnimation.Settings settings = new GreenT.HornyScapes.Animations.CardAnimation.Settings(cardView.gameObject, generalDecorationObjects, generalDecorationBacksideObjects, waitOnTheEnd);
		cardAnimation.Init(settings);
	}

	public override void Stop()
	{
		base.gameObject.SetActive(value: false);
		cardAnimation.Stop();
		base.Stop();
	}

	private void OnDisable()
	{
		Stop();
	}
}
