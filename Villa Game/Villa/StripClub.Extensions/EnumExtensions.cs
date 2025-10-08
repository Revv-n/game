using StripClub.Messenger;

namespace StripClub.Extensions;

public static class EnumExtensions
{
	public static bool Contains(this ChatMessage.MessageState state, ChatMessage.MessageState flags)
	{
		return (state & flags) == flags;
	}
}
