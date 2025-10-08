using System.Collections.Generic;

namespace StripClub.Messenger;

public class ChatMessageComparer : IComparer<ChatMessage>
{
	public int Compare(ChatMessage x, ChatMessage y)
	{
		if (x.SerialNumber > y.SerialNumber)
		{
			return 1;
		}
		if (x.SerialNumber == y.SerialNumber)
		{
			return 0;
		}
		return -1;
	}
}
