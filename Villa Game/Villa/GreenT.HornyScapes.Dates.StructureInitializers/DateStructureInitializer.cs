using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Dates.Extensions;
using GreenT.HornyScapes.Dates.Factories;
using GreenT.HornyScapes.Dates.Mappers;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Dates.Providers;
using UniRx;

namespace GreenT.HornyScapes.Dates.StructureInitializers;

public class DateStructureInitializer : StructureInitializer<IEnumerable<DatePhraseMapper>>
{
	private readonly DateFactory _dateFactory;

	private readonly DateProvider _dateProvider;

	public DateStructureInitializer(DateFactory dateFactory, DateProvider dateProvider, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		_dateFactory = dateFactory;
		_dateProvider = dateProvider;
	}

	public override IObservable<bool> Initialize(IEnumerable<DatePhraseMapper> mappers)
	{
		foreach (IGrouping<int, DatePhraseMapper> item in GroupStepsByDateID(mappers))
		{
			Date entity = _dateFactory.Create(item.Key, item);
			_dateProvider.Add(entity);
		}
		foreach (IGrouping<int, Date> item2 in from x in _dateProvider.Collection
			group x by x.Steps.First().CharacterID)
		{
			foreach (Date item3 in item2)
			{
				item3.SetDateNumber(item2.IndexOf(item3) + 1);
			}
		}
		return Observable.Return(value: true).Debug("Dates has been Loaded", LogType.Data).Do(delegate(bool _init)
		{
			isInitialized.Value = _init;
		});
	}

	private IEnumerable<IGrouping<int, DatePhraseMapper>> GroupStepsByDateID(IEnumerable<DatePhraseMapper> mappers)
	{
		return from x in mappers
			group x by x.date_id;
	}
}
