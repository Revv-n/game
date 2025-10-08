using Zenject;

namespace GreenT.HornyScapes;

public class RestoreEnergyViewControllerBase<TRestore, TView, TRestoreEnergyPopup> where TRestore : BaseEnergyRestore where TView : BaseRestoreEnergyView<TRestore, TRestoreEnergyPopup> where TRestoreEnergyPopup : BaseRestoreEnergyPopup
{
	private TView _restoreEnergyView;

	private TRestore _mainEnergyRestore;

	[Inject]
	private void Init(TRestore mainEnergyRestore)
	{
		_mainEnergyRestore = mainEnergyRestore;
	}

	public void SetView(TView restoreEnergyView)
	{
		_restoreEnergyView = restoreEnergyView;
		restoreEnergyView.Set(_mainEnergyRestore);
	}

	public void Set(bool isSingle)
	{
		_restoreEnergyView.SetBackground(isSingle);
	}
}
