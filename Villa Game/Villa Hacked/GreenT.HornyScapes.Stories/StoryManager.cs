using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub;

namespace GreenT.HornyScapes.Stories;

public class StoryManager : CollectionManager<Story>
{
	private ISaver saver;

	public StoryManager(ISaver saver)
	{
		this.saver = saver;
	}

	public void AddPhrases(IEnumerable<StoryPhrase> phrases)
	{
		foreach (IGrouping<int, StoryPhrase> group in from _phrase in phrases
			group _phrase by _phrase.StoryId)
		{
			Story story = collection.Find((Story _story) => _story.ID == group.Key);
			if (story == null)
			{
				Story story2 = new Story(group.Key, group.AsEnumerable());
				Add(story2);
				saver.Add(story2);
			}
			else
			{
				story.AddPhrases(group.AsEnumerable());
			}
		}
	}

	public Story GetStoryOrDefault(int id)
	{
		return collection.FirstOrDefault((Story _story) => _story.ID == id);
	}

	public void Initialize()
	{
		foreach (Story item in base.Collection)
		{
			item.Initialize();
		}
	}
}
