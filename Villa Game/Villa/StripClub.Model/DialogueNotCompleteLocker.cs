using StripClub.Messenger;

namespace StripClub.Model;

public class DialogueNotCompleteLocker : Locker
{
	public int DialogueID { get; }

	public DialogueNotCompleteLocker(int dialogueID)
	{
		DialogueID = dialogueID;
	}

	public void Set(Dialogue dialogue)
	{
		if (dialogue.ID == DialogueID)
		{
			isOpen.Value = !dialogue.IsComplete;
		}
	}
}
