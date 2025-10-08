using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Characters;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Stories.Data;

public class PhraseFactory : IFactory<PhraseMapper, StoryPhrase>, IFactory
{
	private readonly CharacterManager characterManager;

	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	public PhraseFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, CharacterManager characterManager)
	{
		this.lockerFactory = lockerFactory;
		this.characterManager = characterManager;
	}

	public StoryPhrase Create(PhraseMapper mapper)
	{
		ICharacter character;
		try
		{
			character = characterManager.Get(mapper.character_data);
		}
		catch (KeyNotFoundException innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Character {mapper.character_data} doesn't exist");
		}
		string text = "content.story." + mapper.story_id + "." + mapper.step + ".text";
		string nameKey = character.NameKey;
		ILocker[] array = new ILocker[mapper.unlock_type.Length];
		try
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = lockerFactory.Create(mapper.unlock_type[i], mapper.unlock_value[i], LockerSourceType.Phrase);
			}
		}
		catch (Exception innerException2)
		{
			throw innerException2.SendException(GetType().Name + ": Can't create locker for storyID " + mapper.story_id + " step: " + mapper.step);
		}
		return new StoryPhrase(mapper.story_id, mapper.step, mapper.characters_visible, mapper.character_data, text, nameKey, array);
	}
}
