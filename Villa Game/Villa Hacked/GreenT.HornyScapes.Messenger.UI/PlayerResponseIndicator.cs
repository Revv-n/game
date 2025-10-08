using System;
using System.Linq;
using StripClub.Extensions;
using StripClub.Messenger;

namespace GreenT.HornyScapes.Messenger.UI;

public class PlayerResponseIndicator : ConversationLastMessageIndicator
{
	public override bool DisplayCondition()
	{
		return base.Source.Dialogues.SelectMany((Dialogue _dialogue) => _dialogue.Messages).Any(delegate(ChatMessage _message)
		{
			ChatMessage.MessageState state = _message.State;
			if (_message is CharacterChatMessage)
			{
				if (state.Contains(ChatMessage.MessageState.Delivered))
				{
					return !state.Contains(ChatMessage.MessageState.Read);
				}
				return false;
			}
			if (_message is PlayerChatMessage)
			{
				return !state.Contains(ChatMessage.MessageState.Delivered);
			}
			throw new NotImplementedException();
		});
	}
}
