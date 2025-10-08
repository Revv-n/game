using StripClub.Model.Character;
using StripClub.Stories;

namespace GreenT.HornyScapes.Dates.Views;

public class DateSpeakerView : SpeakerView
{
	protected override CharacterStories GetStory(int characterId)
	{
		return characterManager.Get(characterId).GetDateStory();
	}
}
