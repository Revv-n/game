using System;
using UniRx;

namespace GreenT.HornyScapes.Saves;

public class SaveNotifier
{
	private readonly Subject<Unit> _onLocalSave = new Subject<Unit>();

	private readonly Subject<Unit> _onServerSave = new Subject<Unit>();

	public void NotifyLocalSave()
	{
		_onLocalSave.OnNext(Unit.Default);
	}

	public void NotifyServerSave()
	{
		_onServerSave.OnNext(Unit.Default);
	}

	public IObservable<Unit> OnLocalSave()
	{
		return _onLocalSave.AsObservable();
	}

	public IObservable<Unit> OnServerSave()
	{
		return _onServerSave.AsObservable();
	}

	public IObservable<Unit> OnSave()
	{
		return _onLocalSave.Merge(_onServerSave).Debug("On save", LogType.Payments | LogType.SaveEvent).AsObservable();
	}
}
