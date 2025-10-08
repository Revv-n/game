using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Dates.Providers;
using GreenT.HornyScapes.Dates.Services;
using StripClub.Model.Cards;
using StripClub.Model.Character;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Characters.Data;

public class CharacterStructureInitializer : StructureInitializerViaArray<CharacterInfoMapper, ICharacter>
{
	private const int preloadCharacterID = 1001;

	private readonly CardsCollection cardsCollection;

	private readonly CharacterBundleLoader characterBundleDataLoader;

	private readonly CharacterStoryBundleLoader characterStoryBundleLoader;

	private readonly DateIconDataLoadService _dateIconDataLoadService;

	private readonly DateProvider _dateProvider;

	public CharacterStructureInitializer(CharacterManager manager, IFactory<CharacterInfoMapper, CharacterInfo> factory, CardsCollection cardsCollection, CharacterBundleLoader characterBundleDataLoader, CharacterStoryBundleLoader characterStoryBundleLoader, DateIconDataLoadService dateIconDataLoadService, DateProvider dateProvider, IEnumerable<IStructureInitializer> others = null)
		: base((IManager<ICharacter>)manager, (IFactory<CharacterInfoMapper, ICharacter>)(object)factory, others)
	{
		this.cardsCollection = cardsCollection;
		this.characterBundleDataLoader = characterBundleDataLoader;
		this.characterStoryBundleLoader = characterStoryBundleLoader;
		_dateIconDataLoadService = dateIconDataLoadService;
		_dateProvider = dateProvider;
	}

	public override IObservable<bool> Initialize(IEnumerable<CharacterInfoMapper> mappers)
	{
		try
		{
			ICharacter[] array = mappers.Select((CharacterInfoMapper _map) => factory.Create(_map)).ToArray();
			manager.AddRange(array);
			cardsCollection.AddRange(array);
			return Observable.Do<bool>(Observable.Select<Unit, bool>(Observable.AsSingleUnitObservable<CharacterStories>(Observable.Do<CharacterStories>(Observable.Concat<CharacterStories>(Observable.Select<DateIconData, IObservable<CharacterStories>>(Observable.Do<DateIconData>(Observable.SelectMany<CharacterData, DateIconData>(Observable.Do<CharacterData>(characterBundleDataLoader.Load(1001), (Action<CharacterData>)delegate(CharacterData _bundle)
			{
				((manager as CharacterManager).Get(1001) as CharacterInfo).Set(_bundle);
			}), (Func<CharacterData, IObservable<DateIconData>>)((CharacterData _) => _dateIconDataLoadService.LoadForCharacter((manager as CharacterManager).Get(1001)))), (Action<DateIconData>)delegate(DateIconData bundleData)
			{
				_dateProvider.Get(bundleData.ID).SetBundleData(bundleData);
			}), (Func<DateIconData, IObservable<CharacterStories>>)((DateIconData _) => characterStoryBundleLoader.Load(1001)))), (Action<CharacterStories>)delegate(CharacterStories _bundle)
			{
				((manager as CharacterManager).Get(1001) as CharacterInfo).SetStory(_bundle);
			})), (Func<Unit, bool>)((Unit _) => true)).Debug("Characters has been Loaded", LogType.Data), (Action<bool>)delegate(bool _init)
			{
				isInitialized.Value = _init;
			});
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}
}
