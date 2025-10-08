using System;
using UniRx;

namespace GreenT.HornyScapes.Saves;

public class ManualSave : IDisposable
{
	public IObservable<Unit> SaveEvent;

	private Subject<Unit> _saveSubject;

	public ManualSave()
	{
		_saveSubject = new Subject<Unit>();
		SaveEvent = _saveSubject.AsObservable();
	}

	public void Save()
	{
		_saveSubject.OnNext(Unit.Default);
	}

	public void Dispose()
	{
		_saveSubject?.Dispose();
	}
}
