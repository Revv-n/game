using System;
using System.Collections.Generic;
using GreenT.Bonus;
using GreenT.HornyScapes.Card.Bonus;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.UI;
using Merge;
using StripClub.Model.Cards;
using StripClub.UI;
using StripClub.UI.Collections.Promote;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventGirlCardView : CardView
{
	public class Manager : MonoViewManager<ICard, EventGirlCardView>
	{
	}

	[SerializeField]
	private MonoDisplayStrategy displayStrategy;

	[SerializeField]
	private Image girlIcon;

	[SerializeField]
	private TMP_Text girlLevel;

	[SerializeField]
	private TMP_Text bonusValue;

	[SerializeField]
	private Image bonusIcon;

	[SerializeField]
	private Image gameItemBonusIcon;

	[SerializeField]
	private StatableComponent cardPromotionState;

	[SerializeField]
	private Button openPromote;

	[SerializeField]
	private CardSetter cardSetter;

	private IPromote promote;

	private CardsCollection cards;

	private GameSettings gameSettings;

	private MergeIconService _mergeIconProvider;

	private GameItemConfigManager _gameItemConfigManager;

	private CompositeDisposable disposable = new CompositeDisposable();

	[Inject]
	private void InnerInit(GameSettings gameSettings, CardsCollection cards, MergeIconService mergeIconProvider, GameItemConfigManager gameItemConfigManager)
	{
		this.gameSettings = gameSettings;
		this.cards = cards;
		_mergeIconProvider = mergeIconProvider;
		_gameItemConfigManager = gameItemConfigManager;
	}

	protected virtual void Awake()
	{
		openPromote?.onClick.AddListener(cardSetter.PushCard);
	}

	protected virtual void OnDestroy()
	{
		openPromote?.onClick.RemoveAllListeners();
		CompositeDisposable obj = disposable;
		if (obj != null)
		{
			obj.Dispose();
		}
	}

	public override void Set(ICard source)
	{
		CompositeDisposable obj = disposable;
		if (obj != null)
		{
			obj.Clear();
		}
		base.Set(source);
		promote = cards.GetPromoteOrDefault(source);
		if (promote != null)
		{
			SetGirlInfo(source);
			SetBonus(source);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>((IObservable<int>)promote.Progress, (Action<int>)SetStatable), (ICollection<IDisposable>)disposable);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>((IObservable<int>)promote.Level, (Action<int>)SetLevel), (ICollection<IDisposable>)disposable);
		}
	}

	public override void Display(bool display)
	{
		displayStrategy.Display(display);
	}

	private void SetStatable(int progress)
	{
		cardPromotionState.Set((progress >= promote.Target) ? 1 : 0);
	}

	private void SetLevel(int level)
	{
		girlLevel.text = promote.Level.Value.ToString();
	}

	private void SetGirlInfo(ICard card)
	{
		girlLevel.text = promote.Level.Value.ToString();
		if (card is ICharacter character)
		{
			girlIcon.sprite = character.GetBundleData().SpriteForBonus;
			return;
		}
		throw new Exception().SendException($"{GetType().Name}: card must be a character. Card id tried set = {card.ID}");
	}

	private void SetBonus(ICard card)
	{
		if (gameItemBonusIcon != null)
		{
			gameItemBonusIcon.sprite = GetGameItemSprite(card.Bonus);
		}
		ObservableExtensions.Subscribe<int>((IObservable<int>)promote.Level, (Action<int>)delegate
		{
			bonusValue.text = card.Bonus.ToString();
		});
		bonusIcon.sprite = gameSettings.BonusSettings[card.Bonus.BonusType].BonusSprite;
	}

	private Sprite GetGameItemSprite(IBonus cardBonus)
	{
		CharacterMultiplierBonus bonus = (CharacterMultiplierBonus)cardBonus;
		GIKey spriteMaxOpenedSpawner = BonusTools.GetSpriteMaxOpenedSpawner(_gameItemConfigManager, bonus);
		return _mergeIconProvider.GetSprite(spriteMaxOpenedSpawner);
	}
}
