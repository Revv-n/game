using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenT;
using GreenT.HornyScapes.Bank;
using StripClub.Model.Shop;
using UniRx;
using Zenject;

namespace StripClub.UI.Shop;

public abstract class AbstractLotSectionView<T> : BaseAbstractLotSectionView<T, Lot> where T : IBankSection
{
	protected IViewManager<LotContainer, ContainerView> viewManager;

	public IEnumerable<LotView> DisplayedLots => viewManager.VisibleViews.SelectMany((ContainerView _view) => _view.LotViews);

	[Inject]
	public void Init(IViewManager<LotContainer, ContainerView> viewManager)
	{
		this.viewManager = viewManager;
	}

	protected override void DisplayAvailableLots(IEnumerable<Lot> lots)
	{
		try
		{
			viewManager.HideAll();
			foreach (Lot lot in lots)
			{
				LotContainer source = new LotContainer(new Lot[1] { lot });
				viewManager.Display(source);
			}
		}
		catch (Exception innerException)
		{
			ExceptionReport(lots);
			throw innerException.SendException("Impossible to display section with ID: " + base.Source.ID);
		}
	}

	protected override void DisplayCloneLots(IEnumerable<Lot> lots)
	{
		try
		{
			viewManager.HideAll();
			foreach (Lot lot in lots)
			{
				LotContainer source = ((!(lot is BundleLot)) ? new LotContainer(new Lot[1] { lot }) : new LotContainer(new Lot[1] { (lot as BundleLot).CloneWithAccessibleLocker() }));
				viewManager.Display(source);
			}
		}
		catch (Exception innerException)
		{
			ExceptionReport(lots);
			throw innerException.SendException("Impossible to display section with ID: " + base.Source.ID);
		}
	}

	protected override void ExceptionReport(IEnumerable<Lot> lots)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Error when tried to display lots:\n");
		foreach (Lot lot in lots)
		{
			stringBuilder.Append($"lot id = {lot.ID}, lot type = {lot.GetType().Name}\n");
		}
	}

	protected override IEnumerable<Lot> TargetLots(IEnumerable<Lot> collection)
	{
		return collection.Where((Lot _lot) => _lot.TabID == base.Source.ID).ToList();
	}

	protected override IEnumerable<Lot> VisibleLots(IEnumerable<Lot> targetLots)
	{
		return (from lot in targetLots
			where lot.IsAvailable()
			select lot into _lot
			orderby _lot.SerialNumber
			select _lot).ToList();
	}

	protected override void TrackLotLockers(IEnumerable<Lot> allSectionLots)
	{
		_updateStream?.Dispose();
		List<IReadOnlyReactiveProperty<bool>> list = allSectionLots.Select((Lot _lot) => _lot.Locker.IsOpen).ToList();
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
