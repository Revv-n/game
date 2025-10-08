using System;
using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes.BannerSpace;

[Serializable]
public class ItemBackgroundInfo
{
	public Sprite Coins;

	public Sprite Diamonds;

	public Sprite Energy;

	public Sprite Items;

	public Sprite Get(LinkedContent linkedContent)
	{
		if (!(linkedContent is CurrencyLinkedContent currencyLinkedContent))
		{
			return Items;
		}
		return currencyLinkedContent.Currency switch
		{
			CurrencyType.Soft => Coins, 
			CurrencyType.Hard => Diamonds, 
			CurrencyType.Energy => Energy, 
			_ => Items, 
		};
	}
}
