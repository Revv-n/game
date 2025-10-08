using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Extensions;
using StripClub.Messenger;
using StripClub.Messenger.UI;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public sealed class MessageManager : MonoBehaviour
{
	private List<MonoView> messages = new List<MonoView>();

	private MonoViewFactory messageViewFactory;

	private MonoViewFactory playerOptionsViewFactory;

	private PlayerOptionsMessageView optionsMessageView;

	public IEnumerable<MonoView> MessageViews => messages;

	[Inject]
	public void Init([Inject(Id = "Messages")] MonoViewFactory messageViewFactory, [Inject(Id = "Player options")] MonoViewFactory playerOptionsViewFactory)
	{
		this.messageViewFactory = messageViewFactory;
		this.playerOptionsViewFactory = playerOptionsViewFactory;
	}

	public MonoView GetView(ChatMessage message)
	{
		if (!(message is CharacterChatMessage message2))
		{
			if (message is PlayerChatMessage playerChatMessage)
			{
				if (playerChatMessage.GetChoosenResponse != null)
				{
					return GetMessageView<PlayerChatMessage, PlayerMessageView>(playerChatMessage);
				}
				return GetMessageView<PlayerChatMessage, PlayerOptionsMessageView>(playerChatMessage);
			}
			throw new ArgumentOutOfRangeException("Not recognized type of content");
		}
		if (!message.State.Contains(ChatMessage.MessageState.Delivered))
		{
			return GetMessageView<CharacterChatMessage, StripClub.Messenger.UI.TypingMessageView>(message2);
		}
		return GetMessageView<CharacterChatMessage, CharacterMessageView>(message2);
	}

	private K GetMessageView<T, K>(T message) where T : ChatMessage where K : MessageView<T>, IView<T>
	{
		K val = FindMessage<T, K>();
		if (val != null)
		{
			val.Display(display: true);
			val.transform.SetAsLastSibling();
		}
		else
		{
			val = ((!typeof(PlayerOptionsMessageView).IsAssignableFrom(typeof(K))) ? messageViewFactory.Create<K>() : playerOptionsViewFactory.Create<K>());
			messages.Add(val);
		}
		val.Set(message);
		return val;
	}

	private K FindMessage<T, K>() where T : ChatMessage where K : MessageView<T>, IView<T>
	{
		return messages.OfType<K>().FirstOrDefault((K msg) => !msg.gameObject.activeSelf);
	}

	public void HideAllMessages()
	{
		if (optionsMessageView != null && optionsMessageView.IsActive())
		{
			optionsMessageView.Display(display: false);
		}
		foreach (MonoView item in messages.Where((MonoView _message) => _message.IsActive()))
		{
			item.Display(display: false);
		}
	}
}
