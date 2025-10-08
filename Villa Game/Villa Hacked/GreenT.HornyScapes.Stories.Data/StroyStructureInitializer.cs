using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events;
using GreenT.Types;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Stories.Data;

public class StroyStructureInitializer : StructureInitializer<IEnumerable<PhraseMapper>>
{
	private readonly StoryCluster dictionary;

	private readonly IFactory<PhraseMapper, StoryPhrase> factory;

	private readonly ISaver saver;

	private readonly StoryDataCleaner storyDataCleaner;

	public StroyStructureInitializer(StoryCluster dictionary, IFactory<PhraseMapper, StoryPhrase> factory, ISaver saver, StoryDataCleaner storyDataCleaner, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.dictionary = dictionary;
		this.factory = factory;
		this.saver = saver;
		this.storyDataCleaner = storyDataCleaner;
	}

	public override IObservable<bool> Initialize(IEnumerable<PhraseMapper> mappers)
	{
		foreach (IGrouping<ConfigContentType, PhraseMapper> item in from x in mappers
			group x by x.type)
		{
			StoryManager managerByGroup = GetManagerByGroup(item);
			foreach (IGrouping<int, PhraseMapper> item2 in from x in item
				group x by x.story_id)
			{
				List<StoryPhrase> phrases = GroupPhrases(item2);
				Story story = new Story(item2.Key, phrases);
				if (item.Key == ConfigContentType.Event)
				{
					storyDataCleaner.Add(story);
				}
				managerByGroup.Add(story);
				saver.Add(story);
			}
		}
		return Observable.Do<bool>(Observable.Return(true).Debug("Story has been Loaded", LogType.Data), (Action<bool>)delegate(bool _init)
		{
			isInitialized.Value = _init;
		});
	}

	private List<StoryPhrase> GroupPhrases(IGrouping<int, PhraseMapper> storyGroup)
	{
		List<StoryPhrase> list = new List<StoryPhrase>();
		foreach (PhraseMapper item2 in storyGroup)
		{
			StoryPhrase item = factory.Create(item2);
			list.Add(item);
		}
		return list;
	}

	private StoryManager GetManagerByGroup(IGrouping<ConfigContentType, PhraseMapper> typeGroup)
	{
		return typeGroup.Key switch
		{
			ConfigContentType.Main => dictionary[ContentType.Main], 
			ConfigContentType.Event => dictionary[ContentType.Event], 
			_ => throw new ArgumentOutOfRangeException(typeGroup.Key.ToString()).LogException(), 
		};
	}
}
