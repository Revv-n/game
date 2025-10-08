using System.Collections.Generic;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.MiniEvents;

public interface IShopViewController : IViewController
{
	IEnumerable<Lot> CurrentLots { get; }
}
