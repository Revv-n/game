using System;
using System.Linq;

namespace StripClub.Messenger;

[Serializable]
public class DialogueProgressMapper
{
	public int id;

	public int lastMessageNumber;

	public DateTime lastReplyTime;

	public DialogueProgressMapper(Dialogue dialogue)
	{
		id = dialogue.ID;
		lastMessageNumber = dialogue.LastMessage?.SerialNumber ?? GetLastMessageNumber(dialogue);
		lastReplyTime = dialogue.LastMessage?.Time ?? default(DateTime);
	}

	private static int GetLastMessageNumber(Dialogue dialogue)
	{
		if (!dialogue.Messages.Any((ChatMessage x) => (x.State & ChatMessage.MessageState.Delivered) != 0))
		{
			return 1;
		}
		return dialogue.Messages.Where((ChatMessage x) => (x.State & ChatMessage.MessageState.Delivered) != 0).Max((ChatMessage x) => x.SerialNumber);
	}
}
