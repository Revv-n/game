using GreenT.HornyScapes.Dates.Models;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Character;

namespace GreenT.HornyScapes.Characters;

public interface ICharacter : ICard
{
	ILocker DisplayCondition { get; }

	ILocker PreloadLocker { get; }

	int RelationsipId { get; }

	bool IsBundleDataReady { get; }

	CharacterData GetBundleData();

	CharacterStories GetStory();

	DateCharacterStories GetDateStory();

	void Set(CharacterData bundleData);

	void SetStory(CharacterStories characterStories);

	void SetDateStory(DateCharacterStories dateStories);
}
