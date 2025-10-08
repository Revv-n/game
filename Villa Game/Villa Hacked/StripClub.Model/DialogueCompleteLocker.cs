using StripClub.Messenger;

namespace StripClub.Model;

public class DialogueCompleteLocker : Locker
{
	public int DialogueID { get; }

	public DialogueCompleteLocker(int dialogueID)
	{
		DialogueID = dialogueID;
	}

	public void Set(Dialogue dialogue)
	{
		if (dialogue.ID == DialogueID)
		{
			isOpen.Value = dialogue.IsComplete;
		}
	}
}
