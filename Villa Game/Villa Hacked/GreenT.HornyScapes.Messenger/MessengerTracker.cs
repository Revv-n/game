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
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.Timer(characterDeliveryDelay, Scheduler.MainThread), (Action<long>)delegate
			{
				message.AddFlag(ChatMessage.MessageState.Delivered);
			}), (ICollection<IDisposable>)messagePassedStream);
		}
	}

	private void ObservePlayerMessageDelivered()
	{
		playerDeliveredMessageStream?.Dispose();
		playerDeliveredMessageStream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<MessengerUpdateArgs>(Observable.Delay<MessengerUpdateArgs>(Observable.Where<MessengerUpdateArgs>(messenger.OnUpdate, (Func<MessengerUpdateArgs, bool>)((MessengerUpdateArgs _args) => _args.Message != null && _args.Message is PlayerChatMessage playerChatMessage && playerChatMessage.State.Contains(ChatMessage.MessageState.Delivered) && !playerChatMessage.State.Contains(ChatMessage.MessageState.Passed))), playerDeliveryDelay, Scheduler.MainThread), (Action<MessengerUpdateArgs>)delegate(MessengerUpdateArgs _args)
		{
			_args.Message.AddFlag(ChatMessage.MessageState.Passed);
		}), (Component)this);
	}

	private void ObserveCharacterMessagePassEvent()
	{
		messagePassedStream.Clear();
		IObservable<ChatMessage> observable = ObservableNextMessageLoading();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CharacterChatMessage>(Observable.OfType<ChatMessage, CharacterChatMessage>(observable), (Action<CharacterChatMessage>)SetFakeDeliveryDelay), (ICollection<IDisposable>)messagePassedStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ChatMessage>(observable, (Action<ChatMessage>)delegate(ChatMessage _message)
		{
			messageSetter.Add(_message);
		}), (ICollection<IDisposable>)messagePassedStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IEnumerable<int>>(Observable.Select<CharacterChatMessage, IEnumerable<int>>(Observable.Where<CharacterChatMessage>(Observable.OfType<ChatMessage, CharacterChatMessage>(observable), (Func<CharacterChatMessage, bool>)((CharacterChatMessage _message) => _message.Attachements.Any())), (Func<CharacterChatMessage, IEnumerable<int>>)((CharacterChatMessage _message) => _message.Attachements)), (Action<IEnumerable<int>>)galleryController.LoadMedia, (Action<Exception>)delegate
		{
		}), (ICollection<IDisposable>)messagePassedStream);
	}

	private IObservable<ChatMessage> ObservableNextMessageLoading()
	{
		IObservable<Dialogue> observable = Observable.ToObservable<Dialogue>(from _dialogue in messenger.GetDialogues()
			where !_dialogue.IsComplete && _dialogue.LastMessage != null
			select _dialogue);
		return Observable.Share<ChatMessage>(Observable.DelayFrameSubscription<ChatMessage>(Observable.SelectMany<ChatMessage, ChatMessage>(Observable.SelectMany<Dialogue, ChatMessage>(Observable.Merge<Dialogue>(Observable.Select<MessengerUpdateArgs, Dialogue>(Observable.Where<MessengerUpdateArgs>(messenger.OnUpdate, (Func<MessengerUpdateArgs, bool>)((MessengerUpdateArgs _args) => _args.Dialogue != null && !_args.Dialogue.IsComplete)), (Func<MessengerUpdateArgs, Dialogue>)((MessengerUpdateArgs _args) => _args.Dialogue)), new IObservable<Dialogue>[1] { observable }), (Func<Dialogue, IObservable<ChatMessage>>)((Dialogue _dialogue) => Observable.TakeWhile<ChatMessage>(EmitOnceOnPass(_dialogue.LastMessage), (Func<ChatMessage, bool>)((ChatMessage _) => !_dialogue.IsComplete)))), (Func<ChatMessage, IObservable<ChatMessage>>)((ChatMessage _message) => messengerDataManager.LoadNextMessage(_message.DialogueID, _message.SerialNumber))), 1, (FrameCountType)0));
	}

	public IObservable<ChatMessage> EmitOnceOnPass(ChatMessage message)
	{
		if (message.State.Contains(ChatMessage.MessageState.Passed))
		{
			return Observable.Return<ChatMessage>(message);
		}
		return Observable.Take<ChatMessage>(Observable.Where<ChatMessage>(message.OnUpdate, (Func<ChatMessage, bool>)((ChatMessage _message) => _message.State.Contains(ChatMessage.MessageState.Passed))), 1);
	}

	private void LoadUnlockedDialogue()
	{
		dialogueUnlockStream?.Dispose();
		dialogueUnlockStream = ObservableExtensions.Subscribe<int>(ObservableUnlockedDialogueID(), (Action<int>)LoadDialogue);
	}

	private IObservable<int> ObservableUnlockedDialogueID()
	{
		IEnumerable<Dialogue> dialogues = messenger.GetDialogues();
		return Observable.Merge<int>(Observable.SelectMany<DialogueLocker, int>(Observable.ToObservable<DialogueLocker>(from _locker in messenger.GetDialogueLockers()
			where dialogues.All((Dialogue _dialogue) => _dialogue.ID != _locker.DialogueID)
			select _locker), (Func<DialogueLocker, IObservable<int>>)((DialogueLocker _locker) => Observable.Select<bool, int>(Observable.First<bool>((IObservable<bool>)_locker.IsOpen, (Func<bool, bool>)((bool x) => x)), (Func<bool, int>)((bool _) => _locker.DialogueID)))), Array.Empty<IObservable<int>>());
	}

	public void LoadDialogue(int dialogueID)
	{
		IConnectableObservable<Dialogue> obj = Observable.Publish<Dialogue>(Observable.DelayFrame<Dialogue>(Observable.SelectMany<IEnumerable<Dialogue>, Dialogue>(messengerDataManager.LoadDialogues(dialogueID), (Func<IEnumerable<Dialogue>, IEnumerable<Dialogue>>)((IEnumerable<Dialogue> x) => x)), 1, (FrameCountType)0));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IEnumerable<Conversation>>(Observable.Share<IEnumerable<Conversation>>(Observable.SelectMany<int, IEnumerable<Conversation>>(Observable.Select<Dialogue, int>(Observable.Where<Dialogue>((IObservable<Dialogue>)obj, (Func<Dialogue, bool>)((Dialogue _dialogue) => !messenger.TryGetConversation(_dialogue.ConversationID, out var _))), (Func<Dialogue, int>)((Dialogue _dialogue) => _dialogue.ConversationID)), (Func<int, IObservable<IEnumerable<Conversation>>>)((int _conversationID) => messengerDataManager.LoadConversations(_conversationID)))), (Action<IEnumerable<Conversation>>)delegate(IEnumerable<Conversation> _conversations)
		{
			conversationSetter.Add(_conversations.ToArray());
		}, (Action<Exception>)delegate(Exception ex)
		{
			throw ex.LogException();
		}), (ICollection<IDisposable>)dialogueLoadingStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Dialogue>((IObservable<Dialogue>)obj, (Action<Dialogue>)delegate(Dialogue _dialogue)
		{
			dialogueSetter.Add(_dialogue);
		}, (Action<Exception>)delegate(Exception ex)
		{
			throw ex.LogException();
		}), (ICollection<IDisposable>)dialogueLoadingStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ChatMessage>(Observable.SelectMany<Dialogue, ChatMessage>((IObservable<Dialogue>)obj, (Func<Dialogue, IObservable<ChatMessage>>)((Dialogue _dialogue) => messengerDataManager.LoadNextMessage(_dialogue.ID, int.MinValue))), (Action<ChatMessage>)delegate(ChatMessage _message)
		{
			messageSetter.Add(_message);
			if (_message is CharacterChatMessage fakeDeliveryDelay)
			{
				SetFakeDeliveryDelay(fakeDeliveryDelay);
			}
		}, (Action<Exception>)delegate(Exception ex)
		{
			throw ex.LogException();
		}), (ICollection<IDisposable>)dialogueLoadingStream);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)dialogueLoadingStream);
	}

	private void OnDestroy()
	{
		messagePassedStream.Dispose();
		dialogueLoadingStream.Dispose();
		dialogueUnlockStream?.Dispose();
		playerDeliveredMessageStream?.Dispose();
	}
}
