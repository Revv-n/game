using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;

namespace StripClub.Messenger;

[MementoHolder]
public class MessengerState : ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public DialogueProgressMapper[] dialogueProgresses;

		public Memento(MessengerState state)
			: base(state)
		{
			dialogueProgresses = (from _dialogue in state.Source.GetDialogues()
				select new DialogueProgressMapper(_dialogue)).ToArray();
		}
	}

	private DialogueProgressMapper[] loadedStates;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public IMessengerManager Source { get; private set; }

	public IEnumerable<DialogueProgressMapper> SavedDialogueStates => loadedStates.AsEnumerable();

	public string UniqueKey()
	{
		return "Messenger";
	}

	public MessengerState(IMessengerManager messenger)
	{
		Source = messenger;
		Initialize();
	}

	public void Initialize()
	{
		loadedStates = new DialogueProgressMapper[0];
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		FixBrokeSave(memento2);
		loadedStates = memento2.dialogueProgresses;
	}

	private static void FixBrokeSave(Memento messengerMemento)
	{
		foreach (DialogueProgressMapper item in messengerMemento.dialogueProgresses.Where((DialogueProgressMapper x) => x.lastMessageNumber < 0))
		{
			item.lastMessageNumber = Math.Max(1, item.lastMessageNumber);
		}
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}
}
