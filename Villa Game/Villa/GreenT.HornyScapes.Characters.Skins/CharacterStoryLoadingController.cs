using System;
using System.Linq;
using GreenT.HornyScapes.Stories;
using GreenT.Types;
using StripClub.Model.Character;
using UniRx;

namespace GreenT.HornyScapes.Characters.Skins;

public class CharacterStoryLoadingController : IDisposable
{
	private readonly CharacterManager characterManager;

	private readonly CharacterStoryBundleLoader characterStoryBundleLoader;

	private readonly StoryCluster storyCluster;

	private IDisposable trackAndLoadStream;

	public CharacterStoryLoadingController(CharacterManager characterManager, StoryCluster storyCluster, CharacterStoryBundleLoader characterStoryBundleLoader)
	{
		this.characterManager = characterManager;
		this.storyCluster = storyCluster;
		this.characterStoryBundleLoader = characterStoryBundleLoader;
	}

	public IObservable<CharacterStories> LoadUnlockedStories()
	{
		return (from _character in characterManager.Collection.OfType<CharacterInfo>().ToObservable()
			where _character.PreloadLocker.IsOpen.Value && _character.ContentType == ContentType.Main
			select _character).Where(storyCluster.IsPhraseEnable).Select(LoadStoryBundleByCharacter).Concat()
			.DefaultIfEmpty();
	}

	private IObservable<CharacterStories> LoadStoryBundleByCharacter(CharacterInfo _character)
	{
		return characterStoryBundleLoader.Load(_character.ID).Do(_character.SetStory).Catch(delegate(Exception ex)
		{
			throw ex.SendException("Exception on trying to load character: " + _character);
		});
	}

	public void Dispose()
	{
		trackAndLoadStream?.Dispose();
	}
}
