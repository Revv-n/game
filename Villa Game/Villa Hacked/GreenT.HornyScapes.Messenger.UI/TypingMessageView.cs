using GreenT.HornyScapes.UI;
using StripClub.Messenger;
using StripClub.Messenger.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Messenger.UI;

public class TypingMessageView : MessageView<CharacterChatMessage>
{
	[SerializeField]
	private MonoDisplayStrategy displayStrategy;

	public override void Display(bool display)
	{
		displayStrategy.Display(display);
	}
}
