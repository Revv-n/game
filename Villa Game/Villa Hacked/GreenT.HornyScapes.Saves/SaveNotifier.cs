using System;
using UniRx;

namespace GreenT.HornyScapes.Saves;

public class SaveNotifier
{
	private readonly Subject<Unit> _onLocalSave = new Subject<Unit>();

	private readonly Subject<Unit> _onServerSave = new Subject<Unit>();

	public void NotifyLocalSave()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_onLocalSave.OnNext(Unit.Default);
	}

	public void NotifyServerSave()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_onServerSave.OnNext(Unit.Default);
	}

	public IObservable<Unit> OnLocalSave()
	{
		return Observable.AsObservable<Unit>((IObservable<Unit>)_onLocalSave);
	}

	public IObservable<Unit> OnServerSave()
	{
		return Observable.AsObservable<Unit>((IObservable<Unit>)_onServerSave);
	}

	public IObservable<Unit> OnSave()
	{
		return Observable.AsObservable<Unit>(Observable.Merge<Unit>((IObservable<Unit>)_onLocalSave, new IObservable<Unit>[1] { (IObservable<Unit>)_onServerSave }).Debug("On save", LogType.Payments | LogType.SaveEvent));
	}
}
