using System.Linq;
using StripClub.Extensions;
using StripClub.Messenger;

namespace GreenT.HornyScapes.Messenger.UI;

public class UnreadMessageIndicator : ConversationLastMessageIndicator
{
	public override bool DisplayCondition()
	{
		return base.Source.Dialogues.SelectMany((Dialogue _dialogue) => _dialogue.Messages.OfType<CharacterChatMessage>()).Any((CharacterChatMessage _message) => !_message.State.Contains(ChatMessage.MessageState.Read));
	}
}
