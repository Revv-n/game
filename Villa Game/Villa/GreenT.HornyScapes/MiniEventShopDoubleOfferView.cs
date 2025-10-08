using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop.Offer;
using UnityEngine;

namespace GreenT.HornyScapes;

public class MiniEventShopDoubleOfferView : MonoView<IEnumerable<Lot>>
{
	[SerializeField]
	private BundleLotView _leftPart;

	[SerializeField]
	private BundleLotView _rightPart;

	private const string LEFT_KEY = "Left";

	private const string RIGHT_KEY = "Right";

	public override void Set(IEnumerable<Lot> source)
	{
		base.Set(source);
		if (source.Any())
		{
			Lot lot2 = source.FirstOrDefault((Lot lot) => (lot as BundleLot).Settings.PrefabKey.Contains("Left"));
			Lot lot3 = source.FirstOrDefault((Lot lot) => (lot as BundleLot).Settings.PrefabKey.Contains("Right"));
			if (lot2 != null && lot3 != null)
			{
				_leftPart.Set(lot2);
				_rightPart.Set(lot3);
			}
		}
	}
}
