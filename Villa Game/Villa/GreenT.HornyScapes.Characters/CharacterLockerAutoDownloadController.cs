using System;
using System.Linq;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Dates.Providers;
using GreenT.HornyScapes.Dates.Services;
using GreenT.HornyScapes.Stories;
using GreenT.Types;
using StripClub.Model.Character;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Characters;

public class CharacterLockerAutoDownloadController : IInitializable, IDisposable
{
	private GameStarter gameStarter;

	private CharacterManager characterManager;

	private CharacterBundleLoader characterBundleDataLoader;

	private CharacterStoryBundleLoader storyBundleDataLoader;

	private CompositeDisposable disposable = new CompositeDisposable();

	private DateIconDataLoadService _dateIconDataLoadService;

	private DateProvider _dateProvider;

	private StoryCluster storyManagerCluster;

	public CharacterLockerAutoDownloadController(GameStarter gameStarter, CharacterManager characterManager, CharacterBundleLoader characterBundleDataLoader, CharacterStoryBundleLoader storyBundleDataLoader, DateIconDataLoadService dateIconDataLoadService, DateProvider dateProvider, StoryCluster storyManagerCluster)
	{
		this.gameStarter = gameStarter;
		this.characterManager = characterManager;
		this.characterBundleDataLoader = characterBundleDataLoader;
		this.storyBundleDataLoader = storyBundleDataLoader;
		this.storyManagerCluster = storyManagerCluster;
		_dateIconDataLoadService = dateIconDataLoadService;
		_dateProvider = dateProvider;
	}

	public void Initialize()
	{
		disposable.Clear();
		gameStarter.IsGameActive.Where((bool isActive) => isActive).Subscribe((Action<bool>)delegate
		{
			foreach (CharacterInfo item in from _character in characterManager.Collection.OfType<CharacterInfo>()
				where _character.ContentType == ContentType.Main && _character.LoadType == LoadType.Locker && !_character.IsBundleDataReady
				select _character)
			{
				TrackLocker(item);
				TrackStory(item);
				TrackDates(item);
			}
		}, (Action<Exception>)delegate
		{
		}).AddTo(disposable);
	}

	private void TrackLocker(CharacterInfo character)
	{
		(from state in character.PreloadLocker.IsOpen
			where state
			select state into _
			select character).SelectMany((Func<CharacterInfo, IObservable<CharacterData>>)LoadCharacterBundle).SubscribeAutoDetach((Action<CharacterData>)character.Set, (Action<Exception>)delegate
		{
		}).AddTo(disposable);
	}

	private void TrackStory(CharacterInfo character)
	{
		(from state in character.PreloadLocker.IsOpen
			where state
			select state into _
			select character).Where(storyManagerCluster.IsPhraseEnable).SelectMany((Func<CharacterInfo, IObservable<CharacterStories>>)LoadStoryBundle).SubscribeAutoDetach((Action<CharacterStories>)character.SetStory, (Action<Exception>)delegate
		{
		})
			.AddTo(disposable);
	}

	private void TrackDates(CharacterInfo character)
	{
		(from x in character.PreloadLocker.IsOpen
			where x
			select x into _
			select character).Where(_dateIconDataLoadService.HasAnyDate).SelectMany((Func<CharacterInfo, IObservable<DateIconData>>)_dateIconDataLoadService.LoadForCharacter).SubscribeAutoDetach((Action<DateIconData>)delegate(DateIconData bundleData)
		{
			_dateProvider.Get(bundleData.ID).SetBundleData(bundleData);
		}, (Action<Exception>)delegate
		{
		})
			.AddTo(disposable);
	}

	public IObservable<CharacterData> LoadCharacterBundle(CharacterInfo _character)
	{
		return characterBundleDataLoader.Load(_character.ID);
	}

	public IObservable<CharacterStories> LoadStoryBundle(CharacterInfo _character)
	{
		return storyBundleDataLoader.Load(_character.ID);
	}

	public void Dispose()
	{
		disposable?.Dispose();
	}
}
