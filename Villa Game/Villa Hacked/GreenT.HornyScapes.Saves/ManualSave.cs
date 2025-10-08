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
		SaveEvent = Observable.AsObservable<Unit>((IObservable<Unit>)_saveSubject);
	}

	public void Save()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_saveSubject.OnNext(Unit.Default);
	}

	public void Dispose()
	{
		_saveSubject?.Dispose();
	}
}
