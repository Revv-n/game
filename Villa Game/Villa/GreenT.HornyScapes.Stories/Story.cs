using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub.Stories;
using UniRx;

namespace GreenT.HornyScapes.Stories;

[MementoHolder]
public class Story : ISavableState, IDisposable
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

	public Queue<StoryPhrase> Phrases = new Queue<StoryPhrase>();

	private Subject<Story> onUpdate = new Subject<Story>();

	private StoryPhrase[] initPhrases;

	public int ID { get; }

	public IObservable<Story> OnUpdate => onUpdate.AsObservable();

	public bool IsComplete { get; protected set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public Story(int id, IEnumerable<StoryPhrase> phrases, bool orderBy = true)
	{
		ID = id;
		if (orderBy)
		{
			phrases = phrases.OrderBy((StoryPhrase _phrase) => _phrase.Step);
		}
		initPhrases = phrases.ToArray();
		AddPhrases(phrases);
	}

	public void AddPhrase(StoryPhrase phrase)
	{
		Phrases.Enqueue(phrase);
	}

	public void AddPhrases(IEnumerable<StoryPhrase> phrases)
	{
		foreach (StoryPhrase phrase in phrases)
		{
			AddPhrase(phrase);
		}
	}

	public void SetCompleted()
	{
		IsComplete = true;
		onUpdate.OnNext(this);
	}

	public void Initialize()
	{
		IsComplete = false;
		Phrases.Clear();
		AddPhrases(initPhrases);
		onUpdate.OnNext(this);
	}

	public void Dispose()
	{
		onUpdate?.Dispose();
	}

	public override string ToString()
	{
		return base.ToString() + " ID: " + ID + " Queue size: " + Phrases.Count;
	}

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
		try
		{
			StoryMemento storyMemento = (StoryMemento)memento;
			IsComplete = storyMemento.IsComplete;
		}
		catch (Exception)
		{
			MigrateFromVersion93to94(memento);
		}
	}

	private void MigrateFromVersion93to94(Memento memento)
	{
		StripClub.Stories.Story.StoryMemento storyMemento = (StripClub.Stories.Story.StoryMemento)memento;
		IsComplete = storyMemento.IsComplete;
	}
}
