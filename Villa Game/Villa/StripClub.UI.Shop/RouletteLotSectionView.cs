using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenT;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Bank.BankTabs;
using UniRx;
using Zenject;

namespace StripClub.UI.Shop;

public sealed class RouletteLotSectionView : BaseAbstractLotSectionView<BankTab, RouletteBankSummonLot>
{
	private IViewManager<RouletteBankSummonLot, BankRouletteSummonView> _viewManager;

	[Inject]
	public void Init(RouletteBankSummonLotManager lotManager, IViewManager<RouletteBankSummonLot, BankRouletteSummonView> viewManager)
	{
		_lotManager = lotManager;
		_viewManager = viewManager;
	}

	protected override void DisplayAvailableLots(IEnumerable<RouletteBankSummonLot> lots)
	{
		try
		{
			_viewManager.HideAll();
			foreach (RouletteBankSummonLot lot in lots)
			{
				_viewManager.Display(lot);
			}
		}
		catch (Exception innerException)
		{
			ExceptionReport(lots);
			throw innerException.SendException("Impossible to display section with ID: " + base.Source.ID);
		}
	}

	protected override void DisplayCloneLots(IEnumerable<RouletteBankSummonLot> lots)
	{
		try
		{
			_viewManager.HideAll();
			foreach (RouletteBankSummonLot lot in lots)
			{
				_viewManager.Display(lot);
			}
		}
		catch (Exception innerException)
		{
			ExceptionReport(lots);
			throw innerException.SendException("Impossible to display section with ID: " + base.Source.ID);
		}
	}

	protected override void ExceptionReport(IEnumerable<RouletteBankSummonLot> lots)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Error when tried to display lots:\n");
		foreach (RouletteBankSummonLot lot in lots)
		{
			stringBuilder.Append($"Roulette lot id = {lot.ID}, lot type = {lot.GetType().Name}\n");
		}
	}

	protected override IEnumerable<RouletteBankSummonLot> TargetLots(IEnumerable<RouletteBankSummonLot> collection)
	{
		return collection.Where((RouletteBankSummonLot _lot) => _lot.BankTabId == base.Source.ID).ToList();
	}

	protected override IEnumerable<RouletteBankSummonLot> VisibleLots(IEnumerable<RouletteBankSummonLot> targetLots)
	{
		return targetLots.Where((RouletteBankSummonLot lot) => lot.Locker.IsOpen.Value).ToList();
	}

	protected override void TrackLotLockers(IEnumerable<RouletteBankSummonLot> allSectionLots)
	{
		_updateStream?.Dispose();
		List<IReadOnlyReactiveProperty<bool>> list = allSectionLots.Select((RouletteBankSummonLot _lot) => _lot.Locker.IsOpen).ToList();
		if (list.Any())
		{
			_updateStream = list.Merge().Skip(list.Count).Subscribe(delegate
			{
				Set(base.Source);
			})
				.AddTo(this);
		}
	}
}
