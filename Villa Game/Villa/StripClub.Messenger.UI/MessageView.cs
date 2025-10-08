using StripClub.UI;

namespace StripClub.Messenger.UI;

public abstract class MessageView<T> : MonoView<T>, IMessageView, IView<T>, IView where T : ChatMessage
{
	public virtual void MarkAsLast(bool isLast)
	{
	}
}
