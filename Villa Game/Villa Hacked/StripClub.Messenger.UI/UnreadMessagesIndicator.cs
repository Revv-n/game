using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Extensions;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.Messenger.UI;

public class UnreadMessagesIndicator : MonoBehaviour
{
	[Inject]
	private IMessengerManager messenger;

	private IEnumerable<ChatMessage> unreadMessages;

	private void Start()
	{
		unreadMessages = from _message in messenger.GetMessages().OfType<CharacterChatMessage>()
			where _message.State.Contains(ChatMessage.MessageState.Delivered) && !_message.State.Contains(ChatMessage.MessageState.Read)
			select _message;
		Set(unreadMessages.Count());
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<MessengerUpdateArgs>(Observable.Where<MessengerUpdateArgs>(Observable.TakeUntilDestroy<MessengerUpdateArgs>(messenger.OnUpdate, (Component)this), (Func<MessengerUpdateArgs, bool>)((MessengerUpdateArgs _args) => _args.Dialogue != null || _args.Message != null)), (Action<MessengerUpdateArgs>)delegate
		{
			Set(unreadMessages.Count());
		}), (Component)this);
	}

	protected virtual void Set(int unreadMessages)
	{
		base.gameObject.SetActive(unreadMessages > 0);
	}
}
