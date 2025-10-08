namespace StripClub.Messenger;

public class MessengerUpdateArgs
{
	public Conversation Conversation { get; }

	public Dialogue Dialogue { get; }

	public ChatMessage Message { get; }

	public MessengerUpdateArgs(Conversation conversation = null, Dialogue dialogue = null, ChatMessage message = null)
	{
		Conversation = conversation;
		Dialogue = dialogue;
		Message = message;
	}

	public override string ToString()
	{
		return "Update Args {" + ((Conversation == null) ? string.Empty : (Conversation.ID + " C;")) + ((Dialogue == null) ? string.Empty : (Dialogue.ID + " D;")) + ((Message == null) ? string.Empty : (Message.SerialNumber + " M;")) + "}";
	}
}
