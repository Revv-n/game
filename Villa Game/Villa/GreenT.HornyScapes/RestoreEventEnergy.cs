using GreenT.HornyScapes.Events;
using UniRx;

namespace GreenT.HornyScapes;

public class RestoreEventEnergy : BaseEnergyRestore
{
	private readonly CompositeDisposable _disposable = new CompositeDisposable();

	public void SetDataCleaner(EventDataCleaner eventDataCleaner)
	{
		eventDataCleaner.OnResetRequest.Subscribe(CleanData).AddTo(_disposable);
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
