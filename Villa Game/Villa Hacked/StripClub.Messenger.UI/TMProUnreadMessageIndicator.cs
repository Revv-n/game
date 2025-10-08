using TMPro;
using UnityEngine;

namespace StripClub.Messenger.UI;

public class TMProUnreadMessageIndicator : UnreadMessagesIndicator
{
	[SerializeField]
	private TMP_Text unreadMessagesCount;

	protected override void Set(int unreadMessages)
	{
		base.Set(unreadMessages);
		if (unreadMessages > 0)
		{
			unreadMessagesCount.text = unreadMessages.ToString();
		}
	}
}
