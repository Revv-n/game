using System;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Monetization;

public interface IPurchaseNotifier
{
	event Action<Lot> OnPurchaseLot;
}
