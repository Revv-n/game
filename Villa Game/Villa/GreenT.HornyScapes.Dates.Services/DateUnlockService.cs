using GreenT.Data;
using GreenT.HornyScapes.Dates.Models;
using Merge.Meta.RoomObjects;

namespace GreenT.HornyScapes.Dates.Services;

public class DateUnlockService
{
	private readonly ISaver _saver;

	public DateUnlockService(ISaver saver)
	{
		_saver = saver;
	}

	public void Unlock(Date date)
	{
		date.SetState(EntityStatus.InProgress);
		if (!_saver.TryGetMemento(date.UniqueKey(), out var _))
		{
			_saver.Add(date);
		}
	}
}
