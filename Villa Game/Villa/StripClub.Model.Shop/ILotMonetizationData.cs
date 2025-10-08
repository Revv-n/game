using System;

namespace StripClub.Model.Shop;

public interface ILotMonetizationData
{
	int MonetizationID { get; }

	IObservable<Lot> OnLotReceived();
}
