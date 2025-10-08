using StripClub.Model;
using StripClub.Model.Data;
using Zenject;

namespace GreenT.HornyScapes.MergeStore;

public static class MergeStoreSignalExtension
{
	public static void TrySendSpendHardMergeStoreSignal(this SignalBus signalBus, Cost cost)
	{
		if (cost.Currency == CurrencyType.Hard && cost.Amount > 0)
		{
			signalBus.Fire(new SpendHardMergeStoreSignal(cost.Amount));
		}
	}

	public static void TrySendSpendHardForRechargeSignal(this SignalBus signalBus, Cost cost)
	{
		if (cost.Currency == CurrencyType.Hard && cost.Amount > 0)
		{
			signalBus.Fire(new SpendHardForRechargeSignal(cost.Amount));
		}
	}

	public static void SendBuyItemInMergeStoreSignal(this SignalBus signalBus)
	{
		signalBus.Fire(new BuyItemInMergeStoreSignal());
	}
}
