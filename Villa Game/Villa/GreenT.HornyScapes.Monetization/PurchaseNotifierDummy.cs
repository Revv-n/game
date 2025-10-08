using System;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Monetization;

public class PurchaseNotifierDummy : IPurchaseNotifier
{
	public event Action<Lot> OnPurchaseLot;
}
