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
		return LoadDateBackground(date).SelectMany(LoadDateStory(date)).LastOrDefault();
	}

	private IObservable<Unit> LoadDateStory(Date date)
	{
		return from _ in (from x in date.Steps
				where x.CharacterID != 0
				select x.CharacterID).Distinct().ToObservable().SelectMany((int characterId) => _dateStoryLoader.Load((characterId, date.ID)).Do(delegate(DateCharacterStories story)
			{
				_characterManager.Get(characterId).SetDateStory(story);
			}))
				.LastOrDefault()
			select Unit.Default;
	}

	private IObservable<Unit> LoadDateBackground(Date date)
	{
		string[] prefixes = (from id in date.Steps.SelectMany((DatePhrase step) => step.BackgroundIds)
			where !string.IsNullOrEmpty(id)
			select GetBackgroundId(id)).Distinct().ToArray();
		return from _ in Observable.WhenAll(prefixes.Select((string prefix) => _backgroundDataLoader.Load(prefix)).ToArray()).SelectMany(delegate(DateBackgroundData[] loadedBackgrounds)
			{
				Dictionary<string, DateBackgroundData> backgroundMap = prefixes.Zip(loadedBackgrounds, (string prefix, DateBackgroundData data) => new { prefix, data }).ToDictionary(x => x.prefix, x => x.data);
				return date.Steps.ToObservable().Do(delegate(DatePhrase step)
				{
					DateBackgroundData[] backgroundDatas = (from prefix in (from id in step.BackgroundIds
							where !string.IsNullOrEmpty(id)
							select GetBackgroundId(id)).Distinct()
						where backgroundMap.ContainsKey(prefix)
						select backgroundMap[prefix]).ToArray();
					step.SetBackgroundDatas(backgroundDatas);
				}).LastOrDefault();
			})
			select Unit.Default;
	}

	private string GetBackgroundId(string id)
	{
		return id.Split(':')[0];
	}
}
