using System;
using System.Collections.Generic;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool isActive) => isActive)), (Action<bool>)delegate
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
		}), (ICollection<IDisposable>)disposable);
	}

	private void TrackLocker(CharacterInfo character)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.SubscribeAutoDetach<CharacterData>(Observable.SelectMany<CharacterInfo, CharacterData>(Observable.Select<bool, CharacterInfo>(Observable.Where<bool>((IObservable<bool>)character.PreloadLocker.IsOpen, (Func<bool, bool>)((bool state) => state)), (Func<bool, CharacterInfo>)((bool _) => character)), (Func<CharacterInfo, IObservable<CharacterData>>)LoadCharacterBundle), (Action<CharacterData>)character.Set, (Action<Exception>)delegate
		{
		}), (ICollection<IDisposable>)disposable);
	}

	private void TrackStory(CharacterInfo character)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.SubscribeAutoDetach<CharacterStories>(Observable.SelectMany<CharacterInfo, CharacterStories>(Observable.Where<CharacterInfo>(Observable.Select<bool, CharacterInfo>(Observable.Where<bool>((IObservable<bool>)character.PreloadLocker.IsOpen, (Func<bool, bool>)((bool state) => state)), (Func<bool, CharacterInfo>)((bool _) => character)), (Func<CharacterInfo, bool>)storyManagerCluster.IsPhraseEnable), (Func<CharacterInfo, IObservable<CharacterStories>>)LoadStoryBundle), (Action<CharacterStories>)character.SetStory, (Action<Exception>)delegate
		{
		}), (ICollection<IDisposable>)disposable);
	}

	private void TrackDates(CharacterInfo character)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.SubscribeAutoDetach<DateIconData>(Observable.SelectMany<CharacterInfo, DateIconData>(Observable.Where<CharacterInfo>(Observable.Select<bool, CharacterInfo>(Observable.Where<bool>((IObservable<bool>)character.PreloadLocker.IsOpen, (Func<bool, bool>)((bool x) => x)), (Func<bool, CharacterInfo>)((bool _) => character)), (Func<CharacterInfo, bool>)_dateIconDataLoadService.HasAnyDate), (Func<CharacterInfo, IObservable<DateIconData>>)_dateIconDataLoadService.LoadForCharacter), (Action<DateIconData>)delegate(DateIconData bundleData)
		{
			_dateProvider.Get(bundleData.ID).SetBundleData(bundleData);
		}, (Action<Exception>)delegate
		{
		}), (ICollection<IDisposable>)disposable);
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
		CompositeDisposable obj = disposable;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
