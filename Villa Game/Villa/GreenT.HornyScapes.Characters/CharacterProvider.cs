using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Extensions;
using StripClub.Model.Character;
using UniRx;

namespace GreenT.HornyScapes.Characters;

public class CharacterProvider
{
	private readonly CharacterManager characterManager;

	private readonly CharacterBundleLoader characterDataLoader;

	private CharacterStoryBundleLoader storyBundleDataLoader;

	public CharacterProvider(CharacterManager characterManager, CharacterBundleLoader characterDataLoader, CharacterStoryBundleLoader storyBundleDataLoader)
	{
		this.characterManager = characterManager;
		this.characterDataLoader = characterDataLoader;
		this.storyBundleDataLoader = storyBundleDataLoader;
	}

	public IObservable<ICharacter> Get(int characterId)
	{
		ICharacter emptyCharacter = characterManager.Collection.FirstOrDefault((ICharacter _character) => _character.ID == characterId);
		return Get(emptyCharacter);
	}

	public IObservable<ICharacter> Get(IEnumerable<int> charactersId)
	{
		return characterManager.Collection.Where((ICharacter _character) => charactersId.Contains(_character.ID)).Select(Get).Concat();
	}

	public IObservable<ICharacter> Get(ICharacter emptyCharacter)
	{
		if (!emptyCharacter.IsBundleDataReady)
		{
			return LoadCharacter(emptyCharacter);
		}
		return emptyCharacter.AsEnumerable().ToObservable();
	}

	private IObservable<ICharacter> LoadCharacter(ICharacter _character)
	{
		return from _ in LoadBundleCharacter(_character).Debug("Character data loading: " + _character.ID).Do(SetBundleToCharacter(_character))
			select _character;
	}

	private Action<CharacterData> SetBundleToCharacter(ICharacter _character)
	{
		return characterManager.Get(_character.ID).Set;
	}

	private IObservable<CharacterData> LoadBundleCharacter(ICharacter _character)
	{
		return characterDataLoader.Load(_character.ID);
	}

	public IObservable<CharacterStories> GetStory(int characterId)
	{
		ICharacter character = characterManager.Collection.FirstOrDefault((ICharacter _character) => _character.ID == characterId);
		return GetStory(character);
	}

	public IObservable<CharacterStories> GetStory(int[] charactersId)
	{
		return characterManager.Collection.Where((ICharacter _character) => charactersId.Contains(_character.ID)).Select(GetStory).Concat();
	}

	public IObservable<CharacterStories> GetStory(ICharacter character)
	{
		return LoadStoryCharacter(character);
	}

	private IObservable<CharacterStories> LoadStoryCharacter(ICharacter _character)
	{
		return LoadStoryBundleCharacter(_character).Do(SetStoryBundleToCharacter(_character));
	}

	private Action<CharacterStories> SetStoryBundleToCharacter(ICharacter _character)
	{
		return characterManager.Get(_character.ID).SetStory;
	}

	private IObservable<CharacterStories> LoadStoryBundleCharacter(ICharacter _character)
	{
		return storyBundleDataLoader.Load(_character.ID);
	}
}
