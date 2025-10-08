using System;

namespace StripClub.Model.Shop;

[Flags]
public enum Stickers
{
	FirstPurchase = 1,
	Hot = 2,
	Best = 4,
	Sale = 8
}
