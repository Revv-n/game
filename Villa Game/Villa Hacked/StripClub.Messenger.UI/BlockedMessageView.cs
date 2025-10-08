using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Messenger.UI;

public class BlockedMessageView : MessageView<ChatMessage>
{
	[SerializeField]
	private Text text;

	public void SetContent(string message)
	{
		text.text = message;
	}
}
