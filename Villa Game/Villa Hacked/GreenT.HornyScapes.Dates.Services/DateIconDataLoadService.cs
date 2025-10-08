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
		return Observable.Select<DateIconData, Unit>(Observable.Do<DateIconData>(Observable.SelectMany<ICharacter, DateIconData>(Observable.Where<ICharacter>(Observable.ToObservable<ICharacter>(_characterManager.Collection), (Func<ICharacter, bool>)HasAnyDate), (Func<ICharacter, IObservable<DateIconData>>)LoadForCharacter), (Action<DateIconData>)delegate(DateIconData bundleData)
		{
			_dateProvider.Get(bundleData.ID).SetBundleData(bundleData);
		}), (Func<DateIconData, Unit>)((DateIconData _) => Unit.Default));
	}

	public IObservable<DateIconData> LoadForCharacter(ICharacter character)
	{
		return Observable.DefaultIfEmpty<DateIconData>(Observable.Concat<DateIconData>(Observable.SelectMany<Date, DateIconData>(Observable.ToObservable<Date>(GetDates(character)), (Func<Date, IObservable<DateIconData>>)Load), Array.Empty<IObservable<DateIconData>>()));
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
