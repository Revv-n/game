using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using UniRx;

namespace GreenT.HornyScapes;

public class RestoreEventEnergy : BaseEnergyRestore
{
	private readonly CompositeDisposable _disposable = new CompositeDisposable();

	public void SetDataCleaner(EventDataCleaner eventDataCleaner)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Event>(eventDataCleaner.OnResetRequest, (Action<Event>)CleanData), (ICollection<IDisposable>)_disposable);
	}

	private void CleanData(Event target)
	{
		ResetDailyPriceLogics.ForceDailyInfo();
	}

	public override void Dispose()
	{
		_disposable.Dispose();
	}
}
