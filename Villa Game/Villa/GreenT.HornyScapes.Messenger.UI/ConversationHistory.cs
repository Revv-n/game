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
		conversationUpdateStream = (from _updateArgs in messenger.OnUpdate.TakeUntilDisable(this)
			where _updateArgs.Dialogue != null && !_updateArgs.Dialogue.IsComplete && base.Source.ID == _updateArgs.Dialogue.ConversationID
			select _updateArgs.Dialogue into _dialogue
			select _dialogue.LastMessage).Subscribe(DisplayMessageAndRewindToBottom).AddTo(this);
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
		(from _msg in msgView.Source.OnUpdate.TakeUntilDisable(msgView).FirstOrDefault((ChatMessage _message) => _message.State.Contains(ChatMessage.MessageState.Delivered))
			where _msg != null
			select _msg).Subscribe((Action<ChatMessage>)DisplayMessageAndRewindToBottom, (Action)delegate
		{
			msgView?.Display(display: false);
		}).AddTo(this);
	}

	public void RedirectMediaOpen(Media media)
	{
		windowsManager.Get<PhotoalbumWindow>().ShowMedia(media);
	}

	private void ChangeOptionsToPlayerMessage(PlayerChatMessage message, MonoView messageView)
	{
		(from _msg in (from _message in message.OnUpdate.TakeUntilDisable(messageView)
				where _message.State.Contains(ChatMessage.MessageState.Delivered)
				select _message).FirstOrDefault()
			where _msg != null
			select _msg).Subscribe(Display).AddTo(this);
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
		Observable.EveryLateUpdate().TakeUntilDisable(this).TakeWhile((long _) => historyRect.normalizedPosition.y > float.Epsilon)
			.Subscribe(delegate
			{
				historyRect.normalizedPosition -= Time.deltaTime * step;
			})
			.AddTo(this);
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
