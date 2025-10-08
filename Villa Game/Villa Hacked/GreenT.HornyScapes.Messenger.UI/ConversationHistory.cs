using System;
using GreenT.HornyScapes.Gallery.UI;
using GreenT.UI;
using StripClub.Extensions;
using StripClub.Gallery;
using StripClub.Messenger;
using StripClub.Messenger.UI;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public class ConversationHistory : MonoView<Conversation>
{
	[SerializeField]
	private ScrollRect historyRect;

	private IMessengerManager messenger;

	private IWindowsManager windowsManager;

	private MessageManager messageManager;

	private RectTransform messageContainer;

	private IMessageView currentMessageView;

	private IDisposable conversationUpdateStream;

	[Inject]
	public void Init(IMessengerManager messenger, IWindowsManager windowsManager, MessageManager messageManager, [Inject(Id = "Messages")] MonoViewFactory messageViewFactory)
	{
		this.messenger = messenger;
		this.windowsManager = windowsManager;
		this.messageManager = messageManager;
		messageContainer = messageViewFactory.ViewsContainer.GetComponent<RectTransform>();
	}

	public override void Set(Conversation conversation)
	{
		base.Set(conversation);
		conversationUpdateStream?.Dispose();
		DisplayHistory(conversation);
		conversationUpdateStream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ChatMessage>(Observable.Select<Dialogue, ChatMessage>(Observable.Select<MessengerUpdateArgs, Dialogue>(Observable.Where<MessengerUpdateArgs>(Observable.TakeUntilDisable<MessengerUpdateArgs>(messenger.OnUpdate, (Component)this), (Func<MessengerUpdateArgs, bool>)((MessengerUpdateArgs _updateArgs) => _updateArgs.Dialogue != null && !_updateArgs.Dialogue.IsComplete && base.Source.ID == _updateArgs.Dialogue.ConversationID)), (Func<MessengerUpdateArgs, Dialogue>)((MessengerUpdateArgs _updateArgs) => _updateArgs.Dialogue)), (Func<Dialogue, ChatMessage>)((Dialogue _dialogue) => _dialogue.LastMessage)), (Action<ChatMessage>)DisplayMessageAndRewindToBottom), (Component)this);
	}

	private void OnEnable()
	{
		if (base.Source != null)
		{
			Set(base.Source);
		}
	}

	private void DisplayHistory(Conversation conversation)
	{
		messageManager.HideAllMessages();
		currentMessageView = null;
		foreach (Dialogue dialogue in conversation.Dialogues)
		{
			LoadDialogueMessages(dialogue);
		}
	}

	private void DisplayMessageAndRewindToBottom(ChatMessage message)
	{
		Display(message);
		RewindToBottom();
	}

	private void ChangeTypingMessageToCharacterMessage(StripClub.Messenger.UI.TypingMessageView msgView)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ChatMessage>(Observable.Where<ChatMessage>(Observable.FirstOrDefault<ChatMessage>(Observable.TakeUntilDisable<ChatMessage>(msgView.Source.OnUpdate, (Component)msgView), (Func<ChatMessage, bool>)((ChatMessage _message) => _message.State.Contains(ChatMessage.MessageState.Delivered))), (Func<ChatMessage, bool>)((ChatMessage _msg) => _msg != null)), (Action<ChatMessage>)DisplayMessageAndRewindToBottom, (Action)delegate
		{
			msgView?.Display(display: false);
		}), (Component)this);
	}

	public void RedirectMediaOpen(Media media)
	{
		windowsManager.Get<PhotoalbumWindow>().ShowMedia(media);
	}

	private void ChangeOptionsToPlayerMessage(PlayerChatMessage message, MonoView messageView)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ChatMessage>(Observable.Where<ChatMessage>(Observable.FirstOrDefault<ChatMessage>(Observable.Where<ChatMessage>(Observable.TakeUntilDisable<ChatMessage>(message.OnUpdate, (Component)messageView), (Func<ChatMessage, bool>)((ChatMessage _message) => _message.State.Contains(ChatMessage.MessageState.Delivered)))), (Func<ChatMessage, bool>)((ChatMessage _msg) => _msg != null)), (Action<ChatMessage>)Display), (Component)this);
	}

	private void LoadDialogueMessages(Dialogue dialogue)
	{
		foreach (ChatMessage message in dialogue.Messages)
		{
			Display(message);
		}
	}

	private void Display(ChatMessage message)
	{
		MonoView view = messageManager.GetView(message);
		if (view is StripClub.Messenger.UI.TypingMessageView msgView)
		{
			ChangeTypingMessageToCharacterMessage(msgView);
		}
		else if (view is CharacterMessageView characterMessageView)
		{
			characterMessageView.OnMediaRequest += RedirectMediaOpen;
			SetupPreviousMessage(characterMessageView);
		}
		else if (view is PlayerOptionsMessageView playerOptionsMessageView)
		{
			ChangeOptionsToPlayerMessage(playerOptionsMessageView.Source, view);
		}
		else if (view is PlayerMessageView messageView)
		{
			SetupPreviousMessage(messageView);
		}
	}

	private void SetupPreviousMessage<T>(T messageView) where T : IMessageView
	{
		if (currentMessageView != null && currentMessageView.GetType().Equals(messageView.GetType()))
		{
			currentMessageView.MarkAsLast(isLast: false);
		}
		currentMessageView = messageView;
	}

	private void RewindToBottom()
	{
		_ = messageContainer;
		_ = Vector3.zero;
		Vector2 step = new Vector2(0f, 1f);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.TakeWhile<long>(Observable.TakeUntilDisable<long>(Observable.EveryLateUpdate(), (Component)this), (Func<long, bool>)((long _) => historyRect.normalizedPosition.y > float.Epsilon)), (Action<long>)delegate
		{
			historyRect.normalizedPosition -= Time.deltaTime * step;
		}), (Component)this);
	}

	public override void Display(bool isOn)
	{
		if (base.Source != null && isOn)
		{
			Set(base.Source);
		}
		if (!isOn)
		{
			messageManager.HideAllMessages();
		}
	}
}
