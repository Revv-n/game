using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Dates.Mappers;
using GreenT.HornyScapes.Dates.Models;
using Zenject;

namespace GreenT.HornyScapes.Dates.Factories;

public class DateFactory : IFactory<int, IEnumerable<DatePhraseMapper>, Date>, IFactory
{
	private readonly DatePhraseFactory _datePhraseFactory;

	public DateFactory(DatePhraseFactory datePhraseFactory)
	{
		_datePhraseFactory = datePhraseFactory;
	}

	public Date Create(int id, IEnumerable<DatePhraseMapper> stepMappers)
	{
		IEnumerable<DatePhrase> steps = stepMappers.Select((DatePhraseMapper x) => _datePhraseFactory.Create(x));
		return new Date(id, steps);
	}
}
