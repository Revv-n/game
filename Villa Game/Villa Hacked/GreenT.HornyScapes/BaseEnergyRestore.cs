using System;
using GreenT.Data;
using StripClub.Model;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes;

[MementoHolder]
public abstract class BaseEnergyRestore : IPurchasable, ISavableState, IDisposable
{
	[Serializable]
	public class EnergyRestoreMemento : Memento
	{
		public bool IsFree;

		public bool MigrateToResetDailyPriceLogics;

		public ResetDailyPriceLogics.Memento PriceMemento;

		public EnergyRestoreMemento(BaseEnergyRestore energyRestore)
			: base(energyRestore)
		{
			MigrateToResetDailyPriceLogics = true;
			PriceMemento = energyRestore.ResetDailyPriceLogics.Save();
		}
	}

	private IPurchaseProcessor _purchaseProcessor;

	private LinkedContent _energyContent;

	protected ResetDailyPriceLogics ResetDailyPriceLogics;

	private string _energyRestoreUniqueKey;

	public IResetDailyPriceLogics PriceLogics => ResetDailyPriceLogics;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public void SetArguments(IPurchaseProcessor purchaseProcessor, LinkedContent energyContent, ResetDailyPriceLogics priceLogics, CurrencyType currencyType)
	{
		_purchaseProcessor = purchaseProcessor;
		_energyContent = energyContent;
		ResetDailyPriceLogics = priceLogics;
		_energyRestoreUniqueKey = currencyType.ToString().ToLower() + ".restore";
	}

	public void Initialization()
	{
		ResetDailyPriceLogics.ResetTimer();
	}

	public bool Purchase()
	{
		if (!_purchaseProcessor.TryBuy(_energyContent, ResetDailyPriceLogics.Price))
		{
			return false;
		}
		ResetDailyPriceLogics.OnPurchase();
		return true;
	}

	public string UniqueKey()
	{
		return _energyRestoreUniqueKey;
	}

	public Memento SaveState()
	{
		return new EnergyRestoreMemento(this);
	}

	public void LoadState(Memento memento)
	{
		EnergyRestoreMemento energyRestoreMemento = (EnergyRestoreMemento)memento;
		EnergyRestoreMemento energyRestoreMemento2 = energyRestoreMemento;
		if (energyRestoreMemento2.PriceMemento == null)
		{
			energyRestoreMemento2.PriceMemento = new ResetDailyPriceLogics.Memento();
		}
		if (!energyRestoreMemento.IsFree && !energyRestoreMemento.MigrateToResetDailyPriceLogics)
		{
			energyRestoreMemento.PriceMemento.IsFree = false;
		}
		ResetDailyPriceLogics.Load(energyRestoreMemento.PriceMemento);
	}

	public virtual void Dispose()
	{
	}
}
