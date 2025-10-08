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
		return Observable.ContinueWith<Unit, Unit>(Observable.ContinueWith<Unit, Unit>(Observable.ContinueWith<Unit, Unit>(Observable.Do<Unit>(Observable.ContinueWith<Unit, Unit>(Observable.Do<Unit>(Observable.ContinueWith<CharacterStories, Unit>(Observable.ContinueWith<Unit, CharacterStories>(Observable.Defer<Unit>((Func<IObservable<Unit>>)InitCharacters), (Func<Unit, IObservable<CharacterStories>>)((Unit _) => _characterStoryLoadingController.LoadUnlockedStories())), (Func<CharacterStories, IObservable<Unit>>)((CharacterStories _) => _dateIconDataLoadService.LoadUnlocked())), (Action<Unit>)delegate
		{
			_dateSaveRestoreService.RestoreDatesSave();
		}), (Func<Unit, IObservable<Unit>>)((Unit _) => roomConfigController.LoadUnlockedRooms())), (Action<Unit>)delegate
		{
			_playerPaymentStats.Init();
		}), (Func<Unit, IObservable<Unit>>)delegate
		{
			List<int> list = messengerState.SavedDialogueStates.Select((DialogueProgressMapper _state) => _state.id).ToList();
			FixBrokenMessengerSave(list);
			return LoadUnlockedMessages(list.ToArray());
		}), (Func<Unit, IObservable<Unit>>)delegate
		{
			IEnumerable<LinkedContent> linkedContents = _lotOfflineProvider.PrepareToBuy.Select((Lot lot) => lot.Content);
			linkedContents = LootboxContentExtensions.GetInnerContent(linkedContents);
			return preloadContentService.GetPreloadRewardsStream(linkedContents);
		}), (Func<Unit, IObservable<Unit>>)delegate
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
		IObservable<CharacterData> observable = Observable.Empty<CharacterData>();
		foreach (ICharacter item in enumerable)
		{
			CharacterSettings characterSettings = characterFactory.Create(item);
			characterSettingsManager.Add(characterSettings);
			cardsCollection.Connect(characterSettings.Promote, characterSettings.Public);
			if (item is CharacterInfo { IsBundleDataReady: false } characterInfo)
			{
				observable = Observable.Concat<CharacterData>(observable, new IObservable<CharacterData>[1] { preloadContentService.LoadBundleByCharacter(characterInfo) });
			}
		}
		charactersState.Distinct();
		IObservable<CharacterData> observable2 = Observable.Concat<CharacterData>((from _character in characterManager.Collection.Except(enumerable).OfType<CharacterInfo>()
			where _character.LoadType == LoadType.Locker && _character.PreloadLocker.IsOpen.Value && !_character.IsBundleDataReady
			select _character).Select(preloadContentService.LoadBundleByCharacter));
		observable = Observable.Concat<CharacterData>(observable, new IObservable<CharacterData>[1] { observable2 });
		return Observable.AsSingleUnitObservable<CharacterData>(Observable.Catch<CharacterData, Exception>(observable, (Func<Exception, IObservable<CharacterData>>)delegate(Exception ex)
		{
			throw ex.SendException("Characters data loading exception");
		}));
	}

	private IObservable<Unit> LoadUnlockedMessages(int[] unlockedDialogueIDs)
	{
		if (unlockedDialogueIDs.Length == 0)
		{
			return Observable.ReturnUnit();
		}
		IObservable<IEnumerable<Dialogue>> observable = Observable.Share<IEnumerable<Dialogue>>(messengerDataManager.LoadDialogues(unlockedDialogueIDs));
		IObservable<IEnumerable<ChatMessage>> observable2 = Observable.Share<IEnumerable<ChatMessage>>(Observable.Aggregate<IEnumerable<ChatMessage>>(Observable.SelectMany<int, IEnumerable<ChatMessage>>(Observable.ToObservable<int>((IEnumerable<int>)unlockedDialogueIDs), (Func<int, IObservable<IEnumerable<ChatMessage>>>)delegate(int _id)
		{
			int lastMessageNumber = messengerState.SavedDialogueStates.First((DialogueProgressMapper _dialogueState) => _dialogueState.id == _id).lastMessageNumber;
			return messengerDataManager.LoadMessagesUntil(_id, lastMessageNumber);
		}), (Func<IEnumerable<ChatMessage>, IEnumerable<ChatMessage>, IEnumerable<ChatMessage>>)((IEnumerable<ChatMessage> x, IEnumerable<ChatMessage> y) => x.Union(y))).Debug(GetType().Name + ": Unlocked message loading", LogType.Data | LogType.Messenger));
		IObservable<IEnumerable<Conversation>> observable3 = Observable.Share<IEnumerable<Conversation>>(Observable.ContinueWith<int[], IEnumerable<Conversation>>(Observable.Select<IEnumerable<Dialogue>, int[]>(observable, (Func<IEnumerable<Dialogue>, int[]>)((IEnumerable<Dialogue> _dialogues) => _dialogues.Select((Dialogue _dialogue) => _dialogue.ConversationID).ToArray())), (Func<int[], IObservable<IEnumerable<Conversation>>>)messengerDataManager.LoadConversations));
		IObservable<IEnumerable<Dialogue>> observable4 = Observable.Share<IEnumerable<Dialogue>>(Observable.CombineLatest<IEnumerable<Conversation>, IEnumerable<Dialogue>, IEnumerable<Dialogue>>(Observable.DefaultIfEmpty<IEnumerable<Conversation>>(observable3), observable, (Func<IEnumerable<Conversation>, IEnumerable<Dialogue>, IEnumerable<Dialogue>>)((IEnumerable<Conversation> x, IEnumerable<Dialogue> y) => y)).Debug(GetType().Name + ": Dialogue distribution", LogType.Data | LogType.Messenger));
		IObservable<IEnumerable<ChatMessage>> observable5 = Observable.ContinueWith<IEnumerable<Dialogue>, IEnumerable<ChatMessage>>(observable4, observable2);
		ObservableExtensions.Subscribe<IEnumerable<Conversation>>(observable3, (Action<IEnumerable<Conversation>>)delegate(IEnumerable<Conversation> _conversations)
		{
			conversationSetter.Add(_conversations.ToArray());
		}, (Action<Exception>)delegate(Exception ex)
		{
			exceptionHandler.Handle(ex);
			throw ex.LogException();
		});
		ObservableExtensions.Subscribe<IEnumerable<Dialogue>>(observable4, (Action<IEnumerable<Dialogue>>)delegate(IEnumerable<Dialogue> _dialogues)
		{
			dialogueSetter.Add(_dialogues.ToArray());
		}, (Action<Exception>)delegate(Exception ex)
		{
			exceptionHandler.Handle(ex);
			throw ex.LogException();
		});
		ObservableExtensions.Subscribe<ChatMessage[]>(Observable.Select<IEnumerable<ChatMessage>, ChatMessage[]>(observable5, (Func<IEnumerable<ChatMessage>, ChatMessage[]>)((IEnumerable<ChatMessage> _messages) => _messages.ToArray())), (Action<ChatMessage[]>)messageSetter.Add, (Action<Exception>)delegate(Exception ex)
		{
			exceptionHandler.Handle(ex);
			throw ex.LogException();
		});
		return Observable.DefaultIfEmpty<Unit>(Observable.AsUnitObservable<IEnumerable<ChatMessage>>(observable5)).Debug(GetType().Name + ": Unlocked message loading", LogType.Data | LogType.Messenger);
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
