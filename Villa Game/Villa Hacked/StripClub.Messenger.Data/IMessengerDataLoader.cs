using System;
using System.Collections.Generic;

namespace StripClub.Messenger.Data;

public interface IMessengerDataLoader
{
	IObservable<IEnumerable<Dialogue>> LoadDialogues(params int[] dialogueIDs);

	IObservable<IEnumerable<Conversation>> LoadConversations(params int[] conversationIDs);

	IObservable<IEnumerable<ChatMessage>> LoadMessages(params int[] dialogueIDs);

	IObservable<ChatMessage> LoadNextMessage(int dialogueID, int currentMessageNumber);

	IObservable<IEnumerable<ChatMessage>> LoadMessagesUntil(int dialogueID, int lastMessageNumber);

	IObservable<IEnumerable<DialogueLocker>> LoadDialogueLockers();
}
