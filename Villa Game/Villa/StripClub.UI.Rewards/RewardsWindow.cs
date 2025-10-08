using System;
using System.Collections.Generic;
using GreenT;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Lootboxes.UI;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Meta.Duplicates;
using GreenT.HornyScapes.Presents.Models;
using GreenT.HornyScapes.Resources.UI;
using GreenT.HornyScapes.Saves;
using GreenT.HornyScapes.Subscription;
using GreenT.HornyScapes.UI;
using GreenT.UI;
using StripClub.Model;
using StripClub.Model.Cards;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Rewards;

public class RewardsWindow : Window
{
	public const int MAX_IN_LINE = 5;

	public const int LINES = 2;

	[SerializeField]
	private TextMeshProUGUI cardCounter;

	[SerializeField]
	private OpenChestClip openChest;

	[SerializeField]
	private OpenCardsClip openCards;

	[SerializeField]
	private ShakeCardClip shakeCard;

	[SerializeField]
	private ActivateObjectOnTimeClip shineEffect;

	[SerializeField]
	private NewGirlClip newGirlAppearance;

	[SerializeField]
	private AfterShowNewGirlClip afterShowNewGirlClip;

	[SerializeField]
	private AddSoulsClip addSoulsClip;

	[SerializeField]
	private AddMergeItemClip addMergeItemClip;

	[SerializeField]
	private AddResourcesClip addResourcesClip;

	[SerializeField]
	private AddBattlePassLevelClip addBattlePassLevelClip;

	[SerializeField]
	private AddBoosterClip addBoosterClip;

	[SerializeField]
	private AddSkinClip addSkintemClip;

	[SerializeField]
	private AddDecorationClip addDecorationClip;

	[SerializeField]
	private AddDuplicateClip addDuplicateClip;

	[SerializeField]
	private DisplayRewardsClip showTotalRewards;

	[SerializeField]
	private AddPresentsClip addPresentsClip;

	[SerializeField]
	private GameObject[] rotationContainerChilds;

	[SerializeField]
	private GameObject BotUI;

	[SerializeField]
	private Button skipAll;

	[SerializeField]
	private Button skipOne;

	[SerializeField]
	private Button continueBtn;

	private readonly Subject<Unit> _onOpen = new Subject<Unit>();

	private readonly Subject<Lootbox> _onOpenWithLootbox = new Subject<Lootbox>();

	private readonly Subject<Lootbox> _onCloseWithLootbox = new Subject<Lootbox>();

	private readonly Subject<Unit> _onClose = new Subject<Unit>();

	private readonly Subject<Unit> _onEndCycleSubject = new Subject<Unit>();

	private Lootbox _lootbox;

	private bool _withLootbox;

	private SubscriptionModel _subscriptionModel;

	private LinkedContent _showContentForAllRewards;

	private LinkedContent _showContent;

	private LinkedContent _storedContent;

	private Dictionary<LinkedContent, LinkedContent> _duplicates = new Dictionary<LinkedContent, LinkedContent>();

	private ResourcesWindow _resourcesWindow;

	private bool _isFast;

	private int _cardCountAll;

	private int _count;

	private readonly CompositeDisposable _allDisposables = new CompositeDisposable();

	private readonly CompositeDisposable _currentDisposables = new CompositeDisposable();

	private CardsCollection _cards;

	private IFactory<ICard, IPromote> _promoteFactory;

	private ContentStorageProvider _contentStorage;

	private int resourcesWindowSortingOrder;

	private DuplicateRewardProvider _duplicateProvider;

	private PreloadContentService _preloadContentService;

	public IObservable<Unit> OnOpen => _onOpen;

	public IObservable<Lootbox> OnOpenWithLootbox => _onOpenWithLootbox;

	public IObservable<Lootbox> OnCloseWithLootbox => _onCloseWithLootbox.AsObservable();

	public IObservable<Unit> OnClose => _onClose.AsObservable();

	[Inject]
	public void Init(CardsCollection cards, IFactory<ICard, IPromote> promoteFactory, ContentStorageProvider contentStorage, DuplicateRewardProvider duplicateProvider, PreloadContentService preloadContentService)
	{
		_cards = cards;
		_promoteFactory = promoteFactory;
		_contentStorage = contentStorage;
		_duplicateProvider = duplicateProvider;
		_preloadContentService = preloadContentService;
	}

	protected override void Awake()
	{
		base.Awake();
		skipAll.onClick.AddListener(SkipAllCycle);
		continueBtn.onClick.AddListener(Close);
		skipOne.onClick.AddListener(SkipCurrent);
	}

	public void Init(Lootbox lootbox, LinkedContent lootboxContent)
	{
		SetLootbox(lootbox);
		InitShow(lootboxContent);
		_subscriptionModel = null;
	}

	public void Init(Lootbox lootbox, LinkedContent lootboxContent, SubscriptionModel subscriptionModel)
	{
		SetLootbox(lootbox);
		InitShow(lootboxContent);
		_subscriptionModel = subscriptionModel;
	}

	public void Init(LinkedContent contents)
	{
		_withLootbox = false;
		InitShow(contents);
	}

	private void SetLootbox(Lootbox lootbox)
	{
		_lootbox = lootbox;
		_withLootbox = true;
	}

	private void InitShow(LinkedContent contents)
	{
		SetContentToShow(contents, 10);
		_duplicates.Clear();
		_showContent = ReplaceDuplicates(_showContent);
		_showContentForAllRewards = _showContent;
		_contentStorage.TrySetStoreShow(_showContent);
		_contentStorage.TrySetStoredContent(_storedContent);
	}

	private void SetContentToShow(LinkedContent contents, int chunkSize)
	{
		if (contents == null)
		{
			return;
		}
		if (IsLootboxLinkedContent(contents, out var lootboxLinkedContent))
		{
			SetLootbox(lootboxLinkedContent.Lootbox);
			LinkedContent preopenedContent = lootboxLinkedContent.Lootbox.GetPreopenedContent();
			contents = contents.Next();
			preopenedContent.Insert(contents);
			SetContentToShow(preopenedContent, chunkSize);
			return;
		}
		_showContent = contents;
		LinkedContent tail = _showContent;
		for (int j = 1; HasNext(j); j++)
		{
			tail = tail.Next();
		}
		_storedContent = tail.Next();
		tail.ReleaseNext();
		bool HasNext(int i)
		{
			if (i < chunkSize && tail.Next() != null)
			{
				return !(tail.Next() is LootboxLinkedContent);
			}
			return false;
		}
	}

	private bool IsLootboxLinkedContent(LinkedContent content, out LootboxLinkedContent lootboxLinkedContent)
	{
		lootboxLinkedContent = null;
		if (content is LootboxLinkedContent lootboxLinkedContent2)
		{
			lootboxLinkedContent = lootboxLinkedContent2;
		}
		return lootboxLinkedContent != null;
	}

	private LinkedContent ReplaceDuplicates(LinkedContent contents)
	{
		LinkedContent linkedContent = null;
		for (LinkedContent linkedContent2 = contents; linkedContent2 != null; linkedContent2 = linkedContent2.Next())
		{
			if (_duplicateProvider.TryGetDuplicateReward(linkedContent2, out var reward))
			{
				if (linkedContent != null)
				{
					LinkedContentExtensions.ReplaceNode(linkedContent, linkedContent2, reward);
				}
				else
				{
					reward.Insert(linkedContent2.Next());
					contents = reward;
				}
				_duplicates.Add(reward, linkedContent2);
			}
			linkedContent = linkedContent2;
		}
		return contents;
	}

	public override void Close()
	{
		if (_storedContent != null)
		{
			Init(_storedContent);
			Open();
			return;
		}
		base.Close();
		if (_withLootbox)
		{
			_onCloseWithLootbox.OnNext(_lootbox);
		}
		else
		{
			_onClose.OnNext(default(Unit));
		}
		if ((bool)_resourcesWindow)
		{
			_resourcesWindow.SetTransferButton(interactable: true);
		}
		ReturnSortingOrderResourcesWindow();
	}

	public override void Open()
	{
		base.Open();
		Reset();
		PreInit();
		ShowResourcesWindow();
		if (_withLootbox)
		{
			OpenLootbox();
		}
		else
		{
			OnChestOpened();
		}
	}

	private void PreInit()
	{
		_isFast = false;
		BotUI.gameObject.SetActive(value: false);
		skipAll.gameObject.SetActive(value: true);
		_count = _showContent.Count();
		_cardCountAll = _count;
		cardCounter.text = _count.ToString();
	}

	private void ReturnSortingOrderResourcesWindow()
	{
		if (_resourcesWindow == null)
		{
			_resourcesWindow = windowsManager.Get<ResourcesWindow>();
		}
		windowsManager.MoveWindowToSortingOrder(_resourcesWindow, resourcesWindowSortingOrder);
	}

	private void ShowResourcesWindow()
	{
		if (_resourcesWindow == null)
		{
			_resourcesWindow = windowsManager.Get<ResourcesWindow>();
		}
		resourcesWindowSortingOrder = _resourcesWindow.Canvas.sortingOrder;
		windowsManager.Open(_resourcesWindow);
		try
		{
			_resourcesWindow.SetTransferButton(interactable: false);
		}
		catch (Exception exception)
		{
			exception.LogException();
		}
	}

	private void OpenLootbox()
	{
		openChest.Init(_lootbox.Rarity);
		openChest.Play();
		openChest.OnEnd.Take(1).Subscribe(delegate
		{
			OnChestOpened();
		}).AddTo(_allDisposables);
	}

	private void OnChestOpened()
	{
		BotUI.gameObject.SetActive(value: true);
		if (_isFast)
		{
			SkipAllCycle();
		}
		else
		{
			ShowAllCycles();
		}
	}

	private void SkipAllCycle()
	{
		_isFast = true;
		SkipCurrent();
		openCards.gameObject.SetActive(value: false);
		skipAll.gameObject.SetActive(value: false);
		skipOne.gameObject.SetActive(value: false);
		openChest.gameObject.SetActive(value: false);
	}

	private void SkipCurrent()
	{
		_currentDisposables.Clear();
		shakeCard.Stop();
		shineEffect.Stop();
		newGirlAppearance.Stop();
		addSoulsClip.Stop();
		addResourcesClip.Stop();
		addBattlePassLevelClip.Stop();
		addMergeItemClip.Stop();
		afterShowNewGirlClip.Stop();
		addPresentsClip.Stop();
	}

	private void ShowAllCycles()
	{
		skipOne.gameObject.SetActive(value: true);
		_onEndCycleSubject.DoOnSubscribe(ShowNextCycle).TakeWhile((Unit _) => _showContent != null).Subscribe(delegate
		{
			ShowNextCycle();
		}, delegate(Exception _ex)
		{
			throw _ex.SendException("Can't play lootbox animation");
		})
			.AddTo(_allDisposables);
	}

	private void ShowNextCycle()
	{
		_currentDisposables.Clear();
		_count--;
		if (_isFast)
		{
			return;
		}
		IObservable<Clip> source = OpenCard(_showContent, _count);
		if (_duplicates.ContainsKey(_showContent))
		{
			source = source.ContinueWith((Clip _) => PlayAddDuplicate(_duplicates[_showContent], _showContent));
		}
		else
		{
			LinkedContent showContent = _showContent;
			CurrencyLinkedContent currencyLinkedContent = showContent as CurrencyLinkedContent;
			if (currencyLinkedContent == null)
			{
				MergeItemLinkedContent mergeItemLinkedContent = showContent as MergeItemLinkedContent;
				if (mergeItemLinkedContent == null)
				{
					CardLinkedContent cardLinkedContent = showContent as CardLinkedContent;
					if (cardLinkedContent == null)
					{
						SkinLinkedContent skinLinkedContent = showContent as SkinLinkedContent;
						if (skinLinkedContent == null)
						{
							DecorationLinkedContent decorationLinkedContent = showContent as DecorationLinkedContent;
							if (decorationLinkedContent == null)
							{
								BattlePassLevelLinkedContent battlePassLevelLinkedContent = showContent as BattlePassLevelLinkedContent;
								if (battlePassLevelLinkedContent == null)
								{
									BoosterLinkedContent boosterLinkedContent = showContent as BoosterLinkedContent;
									if (boosterLinkedContent == null)
									{
										PresentLinkedContent presentLinkedContent = showContent as PresentLinkedContent;
										if (presentLinkedContent == null)
										{
											throw new NotImplementedException("There is no behaviour for this rew type: " + _showContent.GetType());
										}
										source = source.ContinueWith((Clip _) => PlayAddPresents(presentLinkedContent));
									}
									else
									{
										source = source.ContinueWith((Clip _) => PlayAddBooster(boosterLinkedContent));
									}
								}
								else
								{
									source = source.ContinueWith((Clip _) => PlayAddBattlePassLevel(battlePassLevelLinkedContent));
								}
							}
							else
							{
								source = source.ContinueWith((Clip _) => PlayAddDecoration(decorationLinkedContent));
							}
						}
						else
						{
							source = source.ContinueWith((Clip _) => PlayAddSkin(skinLinkedContent));
						}
					}
					else
					{
						source = source.ContinueWith((Clip _) => AddCard(cardLinkedContent));
					}
				}
				else
				{
					source = source.ContinueWith((Clip _) => PlayAddMerge(mergeItemLinkedContent));
				}
			}
			else
			{
				source = source.ContinueWith((Clip _) => PlayAddResources(currencyLinkedContent));
			}
		}
		source.Take(1).DoOnCancel(ApplyRewardAndPlayNextAnimation).DoOnCompleted(ApplyRewardAndPlayNextAnimation)
			.Catch(delegate(Exception ex)
			{
				throw ex.LogException();
			})
			.Subscribe()
			.AddTo(_currentDisposables);
		void ApplyRewardAndPlayNextAnimation()
		{
			TryApplyReward();
			ChooseNextAnimation();
		}
	}

	private void ChooseNextAnimation()
	{
		if (_showContent != null && !_isFast)
		{
			_onEndCycleSubject.OnNext(Unit.Default);
		}
		else
		{
			ShowTotal();
		}
	}

	private IObservable<Clip> PlayNewCardEffect(CardLinkedContent cardReward)
	{
		try
		{
			if (_isFast)
			{
				return Observable.Return(newGirlAppearance);
			}
			ICharacter character = cardReward.Card as ICharacter;
			newGirlAppearance.Play();
			newGirlAppearance.Init(character);
		}
		catch (Exception exception)
		{
			exception.LogException();
		}
		return newGirlAppearance.OnEnd.Take(1);
	}

	private IObservable<Clip> PlayCardShake(Rarity rarity)
	{
		shakeCard.Init(rarity);
		shakeCard.Play();
		return shakeCard.OnEnd.Take(1);
	}

	private IObservable<Clip> PlayAddDecoration(DecorationLinkedContent decoration)
	{
		if (_isFast)
		{
			return Observable.Return(addDecorationClip);
		}
		addDecorationClip.Init(decoration);
		addDecorationClip.Play();
		return addDecorationClip.OnEnd.Take(1);
	}

	private IObservable<Clip> PlayAddDuplicate(LinkedContent reward, LinkedContent alternativeReward)
	{
		if (_isFast)
		{
			return Observable.Return(addDuplicateClip);
		}
		addDuplicateClip.Init(reward, alternativeReward);
		addDuplicateClip.Play();
		return addDuplicateClip.OnEnd.Take(1);
	}

	private IObservable<Clip> PlayAddResources(CurrencyLinkedContent resourceReward)
	{
		if (!_isFast)
		{
			addResourcesClip.Init(resourceReward, _resourcesWindow);
			addResourcesClip.Play();
			return addResourcesClip.OnEnd.Take(1);
		}
		return Observable.Return(addResourcesClip);
	}

	private IObservable<Clip> PlayAddBattlePassLevel(BattlePassLevelLinkedContent content)
	{
		if (!_isFast)
		{
			addBattlePassLevelClip.Init(content);
			addBattlePassLevelClip.Play();
			return addBattlePassLevelClip.OnEnd.Take(1);
		}
		return Observable.Return(addBattlePassLevelClip);
	}

	private IObservable<Clip> PlayAddBooster(BoosterLinkedContent content)
	{
		if (!_isFast)
		{
			addBoosterClip.Init(content);
			addBoosterClip.Play();
			return addBoosterClip.OnEnd.Take(1);
		}
		return Observable.Return(addBoosterClip);
	}

	private IObservable<Clip> PlayAddCard(CardLinkedContent cardReward)
	{
		if (!_isFast)
		{
			addSoulsClip.Init(cardReward);
			addSoulsClip.Play();
			return addSoulsClip.OnEnd.Take(1);
		}
		return Observable.Return(addSoulsClip);
	}

	private IObservable<Clip> PlayAddMerge(MergeItemLinkedContent mergeItem)
	{
		if (_isFast)
		{
			return Observable.Return(addMergeItemClip);
		}
		addMergeItemClip.Init(mergeItem);
		addMergeItemClip.Play();
		return addMergeItemClip.OnEnd.Take(1);
	}

	private IObservable<Clip> PlayAddSkin(SkinLinkedContent skin)
	{
		if (_isFast)
		{
			return Observable.Return(addSkintemClip);
		}
		addSkintemClip.Init(skin);
		addSkintemClip.Play();
		return addSkintemClip.OnEnd.Debug("Add Skin").Take(1);
	}

	private IObservable<Clip> PlayAddPresents(PresentLinkedContent present)
	{
		if (!_isFast)
		{
			addPresentsClip.Init(present, _resourcesWindow);
			addPresentsClip.Play();
			return addPresentsClip.OnEnd.Take(1);
		}
		return Observable.Return(addPresentsClip);
	}

	private void ShowTotal()
	{
		if (_subscriptionModel == null)
		{
			showTotalRewards.Init(_showContentForAllRewards, _cardCountAll);
		}
		else
		{
			showTotalRewards.Init(_showContentForAllRewards, _cardCountAll, _subscriptionModel);
		}
		TryApplyAllReward();
		showTotalRewards.Play();
	}

	private IObservable<Clip> OpenCard(LinkedContent reward, int count)
	{
		openCards.Init(_showContent, count);
		openCards.Play();
		IObservable<Clip> first = from _ in openCards.OnEnd.Take(1)
			where !NeedShake(reward)
			select _;
		IObservable<Clip> observable = (from _ in openCards.OnEnd.Take(1)
			where NeedShake(reward)
			select GetRarity(reward)).ContinueWith(PlayCardShake).ContinueWith((Clip _) => ShineEffect());
		return first.Merge(observable);
	}

	private IObservable<Clip> ShineEffect()
	{
		if (!_isFast)
		{
			shineEffect.Play();
		}
		return shineEffect.OnEnd.Take(1);
	}

	private Rarity GetRarity(LinkedContent reward)
	{
		if (!(reward is CardLinkedContent cardLinkedContent))
		{
			if (reward is SkinLinkedContent skinLinkedContent)
			{
				return skinLinkedContent.Skin.Rarity;
			}
			throw new ArgumentOutOfRangeException("reward", "No behaviour for type: " + reward.GetType());
		}
		return cardLinkedContent.Card.Rarity;
	}

	private IObservable<Clip> AddCard(CardLinkedContent reward)
	{
		if (IsNewCard(reward))
		{
			CreatePromote(reward);
			return PlayNewCardEffect(reward).ContinueWith((Clip _) => PlayAddCard(reward));
		}
		return PlayAddCard(reward);
	}

	private void CreatePromote(CardLinkedContent cardContent)
	{
		IPromote promote = _promoteFactory.Create(cardContent.Card);
		_cards.Connect(promote, cardContent.Card, callUnlock: true);
	}

	private bool IsNewCard(CardLinkedContent cardReward)
	{
		return _cards.GetPromoteOrDefault(cardReward.Card) == null;
	}

	private bool NeedShake(LinkedContent reward)
	{
		if (!(reward is CardLinkedContent cardLinkedContent))
		{
			if (reward is SkinLinkedContent skinLinkedContent)
			{
				Rarity rarity = skinLinkedContent.Skin.Rarity;
				if (rarity != Rarity.Epic && rarity != Rarity.Legendary)
				{
					return rarity == Rarity.Mythic;
				}
				return true;
			}
			return false;
		}
		Rarity rarity2 = cardLinkedContent.Card.Rarity;
		if (rarity2 != Rarity.Epic && rarity2 != Rarity.Legendary)
		{
			return rarity2 == Rarity.Mythic;
		}
		return true;
	}

	private void TryApplyReward()
	{
		if (_isFast || _showContent == null)
		{
			return;
		}
		try
		{
			_showContent.AddCurrentToPlayer();
			_showContent = _showContent.Next();
			_contentStorage.TrySetStoreShow(_showContent);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.SendException($"Can't apply reward to player!\n{ex}"));
		}
	}

	private void TryApplyAllReward()
	{
		try
		{
			_showContent?.AddToPlayer();
			_showContent = null;
			_contentStorage.ClearShow();
			if (_withLootbox)
			{
				if (!_lootbox.HasPreopenedContent)
				{
					LinkedContent linkedContents = _lootbox.PrepareContent();
					_preloadContentService.PreloadRewards(linkedContents);
				}
				_onOpenWithLootbox.OnNext(_lootbox);
				_subscriptionModel = null;
			}
			else
			{
				_onOpen.OnNext(default(Unit));
			}
		}
		catch (Exception innerException)
		{
			innerException.SendException("Can't apply reward to player!");
		}
	}

	private void Reset()
	{
		_allDisposables.Clear();
		_currentDisposables.Clear();
		skipAll.gameObject.SetActive(value: false);
		skipOne.gameObject.SetActive(value: false);
		openChest.gameObject.SetActive(value: false);
		openCards.gameObject.SetActive(value: false);
		shakeCard.gameObject.SetActive(value: false);
		shineEffect.gameObject.SetActive(value: false);
		newGirlAppearance.gameObject.SetActive(value: false);
		addSoulsClip.gameObject.SetActive(value: false);
		addResourcesClip.gameObject.SetActive(value: false);
		addBattlePassLevelClip.gameObject.SetActive(value: false);
		addMergeItemClip.gameObject.SetActive(value: false);
		addPresentsClip.gameObject.SetActive(value: false);
		afterShowNewGirlClip.gameObject.SetActive(value: false);
		showTotalRewards.gameObject.SetActive(value: false);
		GameObject[] array = rotationContainerChilds;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		Reset();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_currentDisposables.Dispose();
		_allDisposables.Dispose();
		skipAll.onClick.RemoveAllListeners();
		skipOne.onClick.RemoveAllListeners();
		continueBtn.onClick.RemoveAllListeners();
		DisposeSubject(_onEndCycleSubject);
		DisposeSubject(_onOpenWithLootbox);
		DisposeSubject(_onOpen);
		DisposeSubject(_onCloseWithLootbox);
		DisposeSubject(_onClose);
	}

	private void DisposeSubject<T>(Subject<T> subject)
	{
		subject.OnCompleted();
		subject.Dispose();
	}
}
