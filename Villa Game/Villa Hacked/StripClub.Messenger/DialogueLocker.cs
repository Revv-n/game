using System.Collections.Generic;
using StripClub.Model;

namespace StripClub.Messenger;

public class DialogueLocker : CompositeLocker
{
	public int DialogueID { get; }

	public DialogueLocker(int dialogueID, IEnumerable<ILocker> lockers)
		: base(lockers)
	{
		DialogueID = dialogueID;
	}
}
