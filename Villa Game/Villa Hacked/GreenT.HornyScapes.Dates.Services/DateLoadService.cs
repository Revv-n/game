using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Dates.Loaders;
using GreenT.HornyScapes.Dates.Models;
using UniRx;

namespace GreenT.HornyScapes.Dates.Services;

public class DateLoadService
{
	private readonly DateBackgroundDataLoader _backgroundDataLoader;

	private readonly DateStoryLoader _dateStoryLoader;

	private readonly CharacterManager _characterManager;

	public DateLoadService(DateBackgroundDataLoader backgroundDataLoader, DateStoryLoader dateStoryLoader, CharacterManager characterManager)
	{
		_backgroundDataLoader = backgroundDataLoader;
		_dateStoryLoader = dateStoryLoader;
		_characterManager = characterManager;
	}

	public IObservable<Unit> LoadDate(Date date)
	{
		return Observable.LastOrDefault<Unit>(Observable.SelectMany<Unit, Unit>(LoadDateBackground(date), LoadDateStory(date)));
	}

	private IObservable<Unit> LoadDateStory(Date date)
	{
		return Observable.Select<DateCharacterStories, Unit>(Observable.LastOrDefault<DateCharacterStories>(Observable.SelectMany<int, DateCharacterStories>(Observable.ToObservable<int>((from x in date.Steps
			where x.CharacterID != 0
			select x.CharacterID).Distinct()), (Func<int, IObservable<DateCharacterStories>>)((int characterId) => Observable.Do<DateCharacterStories>(_dateStoryLoader.Load((characterId, date.ID)), (Action<DateCharacterStories>)delegate(DateCharacterStories story)
		{
			_characterManager.Get(characterId).SetDateStory(story);
		})))), (Func<DateCharacterStories, Unit>)((DateCharacterStories _) => Unit.Default));
	}

	private IObservable<Unit> LoadDateBackground(Date date)
	{
		string[] prefixes = (from id in date.Steps.SelectMany((DatePhrase step) => step.BackgroundIds)
			where !string.IsNullOrEmpty(id)
			select GetBackgroundId(id)).Distinct().ToArray();
		return Observable.Select<DatePhrase, Unit>(Observable.SelectMany<DateBackgroundData[], DatePhrase>(Observable.WhenAll<DateBackgroundData>(prefixes.Select((string prefix) => _backgroundDataLoader.Load(prefix)).ToArray()), (Func<DateBackgroundData[], IObservable<DatePhrase>>)delegate(DateBackgroundData[] loadedBackgrounds)
		{
			Dictionary<string, DateBackgroundData> backgroundMap = prefixes.Zip(loadedBackgrounds, (string prefix, DateBackgroundData data) => new { prefix, data }).ToDictionary(x => x.prefix, x => x.data);
			return Observable.LastOrDefault<DatePhrase>(Observable.Do<DatePhrase>(Observable.ToObservable<DatePhrase>((IEnumerable<DatePhrase>)date.Steps), (Action<DatePhrase>)delegate(DatePhrase step)
			{
				DateBackgroundData[] backgroundDatas = (from prefix in (from id in step.BackgroundIds
						where !string.IsNullOrEmpty(id)
						select GetBackgroundId(id)).Distinct()
					where backgroundMap.ContainsKey(prefix)
					select backgroundMap[prefix]).ToArray();
				step.SetBackgroundDatas(backgroundDatas);
			}));
		}), (Func<DatePhrase, Unit>)((DatePhrase _) => Unit.Default));
	}

	private string GetBackgroundId(string id)
	{
		return id.Split(':', StringSplitOptions.None)[0];
	}
}
