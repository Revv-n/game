using GreenT.Data;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Dates.Providers;

namespace GreenT.HornyScapes.Dates.Services;

public class DateSaveRestoreService
{
	private readonly DateProvider _dateProvider;

	private readonly ISaver _saver;

	public DateSaveRestoreService(DateProvider dateProvider, ISaver saver)
	{
		_dateProvider = dateProvider;
		_saver = saver;
	}

	public void RestoreDatesSave()
	{
		foreach (Date item in _dateProvider.Collection)
		{
			if (_saver.TryGetMemento(item.UniqueKey(), out var _))
			{
				_saver.Add(item);
			}
		}
	}
}
