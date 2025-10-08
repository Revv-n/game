using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Data;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Dates.Services;
using GreenT.HornyScapes.Exceptions;
using GreenT.HornyScapes.Messenger;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Sellouts.Services;
using GreenT.HornyScapes.Sellouts.Views;
using GreenT.Types;
using StripClub.Messenger;
using StripClub.Messenger.Data;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Character;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Saves;

public class PostLoadingState
{
	private readonly LotOfflineProvider _lotOfflineProvider;

	private readonly CharacterStoryLoadingController _characterStoryLoadingController;

	private readonly RoomConfigController roomConfigController;

	private readonly MessengerState messengerState;

	private readonly CharacterManager characterManager;

	private readonly CharacterManagerState charactersState;

	private readonly CharacterSettingsFactory characterFactory;

	private readonly CharacterSettingsManager characterSettingsManager;

	private readonly CardsCollection cardsCollection;

	private readonly ContentStorageProvider _contentStorageProvider;

	private readonly SkinDataLoadingController skinDataLoadingController;

	private readonly PreloadContentService preloadContentService;

	private readonly SelloutEntryPoint _selloutAutoReward;

	private readonly IMessengerDataLoader messengerDataManager;

	private readonly ICollectionSetter<ChatMessage> messageSetter;

	private readonly ICollectionSetter<Dialogue> dialogueSetter;

	private readonly ICollectionSetter<Conversation> conversationSetter;

	private readonly IExceptionHandler exceptionHandler;

	private readonly IManager<DialogueConfigMapper> _dialoguesManager;

	private readonly PlayerPaymentsStats _playerPaymentStats;

	private readonly MessengerManager _messengerManager;

	private readonly DateIconDataLoadService _dateIconDataLoadService;

	private readonly DateSaveRestoreService _dateSaveRestoreService;

	public PostLoadingState(LotOfflineProvider lotOfflineProvider, CharacterStoryLoadingController characterStoryLoadingController, RoomConfigController roomConfigController, MessengerState messengerState, CharacterManager characterManager, CharacterManagerState charactersState, CharacterSettingsFactory characterFactory, CharacterSettingsManager characterSettingsManager, CardsCollection cardsCollection, SkinDataLoadingController skinDataLoadingController, PreloadContentService preloadContentService, SelloutEntryPoint selloutAutoReward, IMessengerDataLoader messengerDataManager, ICollectionSetter<ChatMessage> messageSetter, ICollectionSetter<Dialogue> dialogueSetter, ICollectionSetter<Conversation> conversationSetter, IExceptionHandler exceptionHandler, IManager<DialogueConfigMapper> dialoguesManager, IMessengerManager messengerManager, PlayerPaymentsStats playerPaymentStats, ContentStorageProvider contentStorageProvider, DateIconDataLoadService dateIconDataLoadService, DateSaveRestoreService dateSaveRestoreService)
	{
		_lotOfflineProvider = lotOfflineProvider;
		_characterStoryLoadingController = characterStoryLoadingController;
		this.roomConfigController = roomConfigController;
		this.messengerState = messengerState;
		this.characterManager = characterManager;
		this.charactersState = charactersState;
		this.characterFactory = characterFactory;
		this.characterSettingsManager = characterSettingsManager;
		this.cardsCollection = cardsCollection;
		this.skinDataLoadingController = skinDataLoadingController;
		this.preloadContentService = preloadContentService;
		_selloutAutoReward = selloutAutoReward;
		this.messengerDataManager = messengerDataManager;
		this.messageSetter = messageSetter;
		this.dialogueSetter = dialogueSetter;
		this.conversationSetter = conversationSetter;
		this.exceptionHandler = exceptionHandler;
		_dialoguesManager = dialoguesManager;
		_playerPaymentStats = playerPaymentStats;
		_contentStorageProvider = contentStorageProvider;
		_messengerManager = messengerManager as MessengerManager;
		_dateIconDataLoadService = dateIconDataLoadService;
		_dateSaveRestoreService = dateSaveRestoreService;
	}

	public IObservable<Unit> PostLoading()
	{
		return Observable.Defer(InitCharacters).ContinueWith((Unit _) => _characterStoryLoadingController.LoadUnlockedStories()).ContinueWith((CharacterStories _) => _dateIconDataLoadService.LoadUnlocked())
			.Do(delegate
			{
				_dateSaveRestoreService.RestoreDatesSave();
			})
			.ContinueWith((Unit _) => roomConfigController.LoadUnlockedRooms())
			.Do(delegate
			{
				_playerPaymentStats.Init();
			})
			.ContinueWith(delegate
			{
				List<int> list = messengerState.SavedDialogueStates.Select((DialogueProgressMapper _state) => _state.id).ToList();
				FixBrokenMessengerSave(list);
				return LoadUnlockedMessages(list.ToArray());
			})
			.ContinueWith(delegate
			{
				IEnumerable<LinkedContent> linkedContents = _lotOfflineProvider.PrepareToBuy.Select((Lot lot) => lot.Content);
				linkedContents = LootboxContentExtensions.GetInnerContent(linkedContents);
				return preloadContentService.GetPreloadRewardsStream(linkedContents);
			})
			.ContinueWith(delegate
			{
				_contentStorageProvider.TryGetStoredContent(ContentType.Main, out var content);
				return preloadContentService.GetPreloadRewardsStream(content);
			});
	}

	private IObservable<Unit> InitCharacters()
	{
		IEnumerable<ICharacter> collection = characterManager.Collection;
		IEnumerable<int> gainedCharacteIDs = charactersState.UnlockedCharacterIDs.ToArray();
		IEnumerable<ICharacter> enumerable = collection.Where((ICharacter _character) => gainedCharacteIDs.Contains(_character.ID)).ToArray();
		IObservable<CharacterData> first = Observable.Empty<CharacterData>();
		foreach (ICharacter item in enumerable)
		{
			CharacterSettings characterSettings = characterFactory.Create(item);
			characterSettingsManager.Add(characterSettings);
			cardsCollection.Connect(characterSettings.Promote, characterSettings.Public);
			if (item is CharacterInfo { IsBundleDataReady: false } characterInfo)
			{
				first = first.Concat(preloadContentService.LoadBundleByCharacter(characterInfo));
			}
		}
		charactersState.Distinct();
		IObservable<CharacterData> observable = (from _character in characterManager.Collection.Except(enumerable).OfType<CharacterInfo>()
			where _character.LoadType == LoadType.Locker && _character.PreloadLocker.IsOpen.Value && !_character.IsBundleDataReady
			select _character).Select(preloadContentService.LoadBundleByCharacter).Concat();
		first = first.Concat(observable);
		return first.Catch(delegate(Exception ex)
		{
			throw ex.SendException("Characters data loading exception");
		}).AsSingleUnitObservable();
	}

	private IObservable<Unit> LoadUnlockedMessages(int[] unlockedDialogueIDs)
	{
		if (unlockedDialogueIDs.Length == 0)
		{
			return Observable.ReturnUnit();
		}
		IObservable<IEnumerable<Dialogue>> observable = messengerDataManager.LoadDialogues(unlockedDialogueIDs).Share();
		IObservable<IEnumerable<ChatMessage>> other = unlockedDialogueIDs.ToObservable().SelectMany(delegate(int _id)
		{
			int lastMessageNumber = messengerState.SavedDialogueStates.First((DialogueProgressMapper _dialogueState) => _dialogueState.id == _id).lastMessageNumber;
			return messengerDataManager.LoadMessagesUntil(_id, lastMessageNumber);
		}).Aggregate((IEnumerable<ChatMessage> x, IEnumerable<ChatMessage> y) => x.Union(y))
			.Debug(GetType().Name + ": Unlocked message loading", LogType.Data | LogType.Messenger)
			.Share();
		IObservable<IEnumerable<Conversation>> source = observable.Select((IEnumerable<Dialogue> _dialogues) => _dialogues.Select((Dialogue _dialogue) => _dialogue.ConversationID).ToArray()).ContinueWith(messengerDataManager.LoadConversations).Share();
		IObservable<IEnumerable<Dialogue>> source2 = source.DefaultIfEmpty().CombineLatest(observable, (IEnumerable<Conversation> x, IEnumerable<Dialogue> y) => y).Debug(GetType().Name + ": Dialogue distribution", LogType.Data | LogType.Messenger)
			.Share();
		IObservable<IEnumerable<ChatMessage>> source3 = source2.ContinueWith(other);
		source.Subscribe(delegate(IEnumerable<Conversation> _conversations)
		{
			conversationSetter.Add(_conversations.ToArray());
		}, delegate(Exception ex)
		{
			exceptionHandler.Handle(ex);
			throw ex.LogException();
		});
		source2.Subscribe(delegate(IEnumerable<Dialogue> _dialogues)
		{
			dialogueSetter.Add(_dialogues.ToArray());
		}, delegate(Exception ex)
		{
			exceptionHandler.Handle(ex);
			throw ex.LogException();
		});
		source3.Select((IEnumerable<ChatMessage> _messages) => _messages.ToArray()).Subscribe(messageSetter.Add, delegate(Exception ex)
		{
			exceptionHandler.Handle(ex);
			throw ex.LogException();
		});
		return source3.AsUnitObservable().DefaultIfEmpty().Debug(GetType().Name + ": Unlocked message loading", LogType.Data | LogType.Messenger);
	}

	private void FixBrokenMessengerSave(List<int> unlockedDialogueIDs)
	{
		List<DialogueConfigMapper> list = new List<DialogueConfigMapper>();
		foreach (int id in unlockedDialogueIDs)
		{
			DialogueConfigMapper dialogueConfigMapper = _dialoguesManager.Collection.FirstOrDefault((DialogueConfigMapper dialogue) => dialogue.ID == id);
			if (dialogueConfigMapper != null)
			{
				list.Add(dialogueConfigMapper);
			}
		}
		foreach (IGrouping<int, DialogueConfigMapper> item in from _dialogue in list
			group _dialogue by _dialogue.ConversationID)
		{
			DialogueConfigMapper[] orderedGroup = item.OrderBy((DialogueConfigMapper x) => x.ID).ToArray();
			for (int i = 0; i < orderedGroup.Length - 1; i++)
			{
				int iD = orderedGroup[i].ID;
				if (orderedGroup[i + 1].ID - iD <= 1)
				{
					continue;
				}
				int j;
				for (j = i + 1; j < orderedGroup.Count(); j++)
				{
					DialogueProgressMapper dialogueProgressMapper = messengerState.SavedDialogueStates.FirstOrDefault((DialogueProgressMapper dialogue) => dialogue.id == orderedGroup[j].ID);
					dialogueProgressMapper.lastMessageNumber = 1;
					dialogueProgressMapper.lastReplyTime = DateTime.Now;
					_messengerManager.AddBrokenDialogue(orderedGroup[j].ID);
					unlockedDialogueIDs.Remove(orderedGroup[j].ID);
				}
			}
		}
	}
}
