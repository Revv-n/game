using System;
using System.Collections.Generic;

namespace StripClub.Messenger;

public interface IMessengerManager
{
	IObservable<MessengerUpdateArgs> OnUpdate { get; }

	IEnumerable<ChatMessage> GetMessages();

	IEnumerable<Dialogue> GetDialogues();

	IEnumerable<Conversation> GetConversations();

	IEnumerable<DialogueLocker> GetDialogueLockers();

	bool TryGetConversation(int id, out Conversation conversation);

	void Purge();
}
