using StripClub.Model;

namespace GreenT.HornyScapes.Stories;

public class StoryPhrase : Phrase
{
	public int StoryId;

	public CompositeLocker Lockers;

	public StoryPhrase(int story_id, int step, int[,] characters_visible, int сharacterID, string text, string name, ILocker[] lockers)
		: base(step, characters_visible, сharacterID, text, name)
	{
		StoryId = story_id;
		Lockers = new CompositeLocker(lockers);
	}
}
