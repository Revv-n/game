using System;

namespace StripClub.Model.Shop;

public abstract class ValuableLot<T> : Lot where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
{
	protected IPurchaseProcessor purchaseProcessor;

	public abstract Price<T> Price { get; }

	public override bool IsReal => Price.Currency == CurrencyType.Real;

	protected ValuableLot(int id, int monetizationID, int tab_id, int position, int buy_times, ILocker locker, EqualityLocker<int> countLocker, IPurchaseProcessor purchaseProcessor, string shopSource)
		: base(id, monetizationID, tab_id, position, buy_times, locker, countLocker, shopSource)
	{
		this.purchaseProcessor = purchaseProcessor;
	}

	public override bool Purchase()
	{
		if (!purchaseProcessor.TryBuy(Content, Price))
		{
			return false;
		}
		int num = base.Received + 1;
		base.Received = num;
		return true;
	}
}
