using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Gallery;
using StripClub.Extensions;
using StripClub.Messenger;
using StripClub.Messenger.Data;
using StripClub.Model;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Messenger;

public class MessengerTracker : MonoBehaviour
{
	private IMessengerManager messenger;

	private IMessengerDataLoader messengerDataManager;

	private GalleryController galleryController;

	private ICollectionSetter<ChatMessage> messageSetter;

	private ICollectionSetter<Dialogue> dialogueSetter;

	private ICollectionSetter<Conversation> conversationSetter;

	private IConstants<float> constants;

	private CompositeDisposable messagePassedStream = new CompositeDisposable();

	private CompositeDisposable dialogueLoadingStream = new CompositeDisposable();

	private IDisposable dialogueUnlockStream;

	private IDisposable playerDeliveredMessageStream;

	private TimeSpan characterDeliveryDelay;

	private TimeSpan playerDeliveryDelay;

	[Inject]
	public void Init(IMessengerManager messenger, IMessengerDataLoader messengerDataManager, ICollectionSetter<ChatMessage> messageSetter, ICollectionSetter<Dialogue> dialogueSetter, ICollectionSetter<Conversation> conversationSetter, GalleryController gallery, IConstants<float> constants)
	{
		this.messenger = messenger;
		this.messengerDataManager = messengerDataManager;
		galleryController = gallery;
		this.messageSetter = messageSetter;
		this.dialogueSetter = dialogueSetter;
		this.conversationSetter = conversationSetter;
		this.constants = constants;
	}

	public void Start()
	{
		characterDeliveryDelay = TimeSpan.FromSeconds(constants["character_delivery_delay"]);
		playerDeliveryDelay = TimeSpan.FromSeconds(constants["player_delivery_delay"]);
		ObservePlayerMessageDelivered();
		ObserveCharacterMessagePassEvent();
		ValidateCharacterMessagesState();
		ValidatePlayerMessagesState();
		LoadUnlockedDialogue();
	}

	private void ValidateCharacterMessagesState()
	{
		foreach (CharacterChatMessage item in from _messages in messenger.GetMessages().OfType<CharacterChatMessage>()
			where !_messages.State.Contains(ChatMessage.MessageState.Delivered)
			select _messages)
		{
			item.AddFlag(ChatMessage.MessageState.Delivered);
		}
	}

	private void ValidatePlayerMessagesState()
	{
		foreach (PlayerChatMessage item in from _messages in messenger.GetMessages().OfType<PlayerChatMessage>()
			where _messages.State.Contains(ChatMessage.MessageState.Delivered) && !_messages.State.Contains(ChatMessage.MessageState.Passed)
			select _messages)
		{
			item.AddFlag(ChatMessage.MessageState.Passed);
		}
	}

	public void SetFakeDeliveryDelay(CharacterChatMessage message)
	{
		if (!message.State.Contains(ChatMessage.MessageState.Delivered))
		{
			Observable.Timer(characterDeliveryDelay, Scheduler.MainThread).Subscribe(delegate
			{
				message.AddFlag(ChatMessage.MessageState.Delivered);
			}).AddTo(messagePassedStream);
		}
	}

	private void ObservePlayerMessageDelivered()
	{
		playerDeliveredMessageStream?.Dispose();
		playerDeliveredMessageStream = messenger.OnUpdate.Where((MessengerUpdateArgs _args) => _args.Message != null && _args.Message is PlayerChatMessage playerChatMessage && playerChatMessage.State.Contains(ChatMessage.MessageState.Delivered) && !playerChatMessage.State.Contains(ChatMessage.MessageState.Passed)).Delay(playerDeliveryDelay, Scheduler.MainThread).Subscribe(delegate(MessengerUpdateArgs _args)
		{
			_args.Message.AddFlag(ChatMessage.MessageState.Passed);
		})
			.AddTo(this);
	}

	private void ObserveCharacterMessagePassEvent()
	{
		messagePassedStream.Clear();
		IObservable<ChatMessage> source = ObservableNextMessageLoading();
		source.OfType<ChatMessage, CharacterChatMessage>().Subscribe(SetFakeDeliveryDelay).AddTo(messagePassedStream);
		source.Subscribe(delegate(ChatMessage _message)
		{
			messageSetter.Add(_message);
		}).AddTo(messagePassedStream);
		(from _message in source.OfType<ChatMessage, CharacterChatMessage>()
			where _message.Attachements.Any()
			select _message.Attachements).Subscribe((Action<IEnumerable<int>>)galleryController.LoadMedia, (Action<Exception>)delegate
		{
		}).AddTo(messagePassedStream);
	}

	private IObservable<ChatMessage> ObservableNextMessageLoading()
	{
		IObservable<Dialogue> observable = (from _dialogue in messenger.GetDialogues()
			where !_dialogue.IsComplete && _dialogue.LastMessage != null
			select _dialogue).ToObservable();
		return (from _args in messenger.OnUpdate
			where _args.Dialogue != null && !_args.Dialogue.IsComplete
			select _args.Dialogue).Merge(observable).SelectMany((Dialogue _dialogue) => EmitOnceOnPass(_dialogue.LastMessage).TakeWhile((ChatMessage _) => !_dialogue.IsComplete)).SelectMany((ChatMessage _message) => messengerDataManager.LoadNextMessage(_message.DialogueID, _message.SerialNumber))
			.DelayFrameSubscription(1)
			.Share();
	}

	public IObservable<ChatMessage> EmitOnceOnPass(ChatMessage message)
	{
		if (message.State.Contains(ChatMessage.MessageState.Passed))
		{
			return Observable.Return(message);
		}
		return message.OnUpdate.Where((ChatMessage _message) => _message.State.Contains(ChatMessage.MessageState.Passed)).Take(1);
	}

	private void LoadUnlockedDialogue()
	{
		dialogueUnlockStream?.Dispose();
		dialogueUnlockStream = ObservableUnlockedDialogueID().Subscribe(LoadDialogue);
	}

	private IObservable<int> ObservableUnlockedDialogueID()
	{
		IEnumerable<Dialogue> dialogues = messenger.GetDialogues();
		return (from _locker in messenger.GetDialogueLockers()
			where dialogues.All((Dialogue _dialogue) => _dialogue.ID != _locker.DialogueID)
			select _locker).ToObservable().SelectMany((DialogueLocker _locker) => from _ in _locker.IsOpen.First((bool x) => x)
			select _locker.DialogueID).Merge();
	}

	public void LoadDialogue(int dialogueID)
	{
		IConnectableObservable<Dialogue> connectableObservable = messengerDataManager.LoadDialogues(dialogueID).SelectMany((IEnumerable<Dialogue> x) => x).DelayFrame(1)
			.Publish();
		(from _dialogue in connectableObservable
			where !messenger.TryGetConversation(_dialogue.ConversationID, out var _)
			select _dialogue.ConversationID).SelectMany((int _conversationID) => messengerDataManager.LoadConversations(_conversationID)).Share().Subscribe(delegate(IEnumerable<Conversation> _conversations)
		{
			conversationSetter.Add(_conversations.ToArray());
		}, delegate(Exception ex)
		{
			throw ex.LogException();
		})
			.AddTo(dialogueLoadingStream);
		connectableObservable.Subscribe(delegate(Dialogue _dialogue)
		{
			dialogueSetter.Add(_dialogue);
		}, delegate(Exception ex)
		{
			throw ex.LogException();
		}).AddTo(dialogueLoadingStream);
		connectableObservable.SelectMany((Dialogue _dialogue) => messengerDataManager.LoadNextMessage(_dialogue.ID, int.MinValue)).Subscribe(delegate(ChatMessage _message)
		{
			messageSetter.Add(_message);
			if (_message is CharacterChatMessage fakeDeliveryDelay)
			{
				SetFakeDeliveryDelay(fakeDeliveryDelay);
			}
		}, delegate(Exception ex)
		{
			throw ex.LogException();
		}).AddTo(dialogueLoadingStream);
		connectableObservable.Connect().AddTo(dialogueLoadingStream);
	}

	private void OnDestroy()
	{
		messagePassedStream.Dispose();
		dialogueLoadingStream.Dispose();
		dialogueUnlockStream?.Dispose();
		playerDeliveredMessageStream?.Dispose();
	}
}
