using System;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class DummyGiftSystem : IPurchaseNotifier
{
	public event Action<Lot> OnPurchaseLot;
}
