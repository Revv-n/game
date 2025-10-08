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
		return Observable.DefaultIfEmpty<CharacterStories>(Observable.Concat<CharacterStories>(Observable.Select<CharacterInfo, IObservable<CharacterStories>>(Observable.Where<CharacterInfo>(Observable.Where<CharacterInfo>(Observable.ToObservable<CharacterInfo>(characterManager.Collection.OfType<CharacterInfo>()), (Func<CharacterInfo, bool>)((CharacterInfo _character) => _character.PreloadLocker.IsOpen.Value && _character.ContentType == ContentType.Main)), (Func<CharacterInfo, bool>)storyCluster.IsPhraseEnable), (Func<CharacterInfo, IObservable<CharacterStories>>)LoadStoryBundleByCharacter)));
	}

	private IObservable<CharacterStories> LoadStoryBundleByCharacter(CharacterInfo _character)
	{
		return Observable.Catch<CharacterStories, Exception>(Observable.Do<CharacterStories>(characterStoryBundleLoader.Load(_character.ID), (Action<CharacterStories>)_character.SetStory), (Func<Exception, IObservable<CharacterStories>>)delegate(Exception ex)
		{
			throw ex.SendException("Exception on trying to load character: " + _character);
		});
	}

	public void Dispose()
	{
		trackAndLoadStream?.Dispose();
	}
}
