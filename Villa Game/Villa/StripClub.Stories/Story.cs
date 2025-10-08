using System;
using GreenT.Data;

namespace StripClub.Stories;

[MementoHolder]
public class Story : ISavableState
{
	[Serializable]
	public class StoryMemento : Memento
	{
		public bool IsComplete;

		public StoryMemento(Story story)
			: base(story)
		{
			IsComplete = story.IsComplete;
		}
	}

	public int ID { get; }

	public bool IsComplete { get; protected set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public string UniqueKey()
	{
		return ID.ToString();
	}

	public Memento SaveState()
	{
		return new StoryMemento(this);
	}

	public void LoadState(Memento memento)
	{
		StoryMemento storyMemento = (StoryMemento)memento;
		IsComplete = storyMemento.IsComplete;
	}
}
