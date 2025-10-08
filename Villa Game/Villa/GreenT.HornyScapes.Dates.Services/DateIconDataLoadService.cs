using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Dates.Providers;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.Dates.Services;

public class DateIconDataLoadService
{
	private readonly DateProvider _dateProvider;

	private readonly ILoader<int, DateIconData> _dateBundleDataLoader;

	private readonly CharacterManager _characterManager;

	public DateIconDataLoadService(CharacterManager characterManager, DateProvider dateProvider, ILoader<int, DateIconData> dateBundleDataLoader)
	{
		_dateProvider = dateProvider;
		_dateBundleDataLoader = dateBundleDataLoader;
		_characterManager = characterManager;
	}

	public IObservable<Unit> LoadUnlocked()
	{
		return from _ in _characterManager.Collection.ToObservable().Where(HasAnyDate).SelectMany((Func<ICharacter, IObservable<DateIconData>>)LoadForCharacter)
				.Do(delegate(DateIconData bundleData)
				{
					_dateProvider.Get(bundleData.ID).SetBundleData(bundleData);
				})
			select Unit.Default;
	}

	public IObservable<DateIconData> LoadForCharacter(ICharacter character)
	{
		return GetDates(character).ToObservable().SelectMany((Func<Date, IObservable<DateIconData>>)Load).Concat()
			.DefaultIfEmpty();
	}

	public bool HasAnyDate(ICharacter character)
	{
		return _dateProvider.Collection.Any((Date date) => date.Steps.Any((DatePhrase step) => step.CharacterID == character.ID));
	}

	private IObservable<DateIconData> Load(Date date)
	{
		return _dateBundleDataLoader.Load(date.ID);
	}

	private IEnumerable<Date> GetDates(ICharacter character)
	{
		return _dateProvider.Collection.Where((Date date) => date.Steps.Any((DatePhrase step) => step.CharacterID == character.ID));
	}
}
