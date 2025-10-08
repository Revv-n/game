using System.Collections.Generic;
using StripClub.UI;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferPreviewManager : MonoViewManager<OfferSettings, OfferPreview>
{
	private Dictionary<int, OfferPreview> existingViews = new Dictionary<int, OfferPreview>();

	public override OfferPreview Display(OfferSettings source)
	{
		if (existingViews.TryGetValue(source.SortingNumber, out var value))
		{
			if (value.IsActive())
			{
				if (value.Source != source)
				{
					return null;
				}
				return value;
			}
		}
		else
		{
			value = Create();
			views.Add(value);
			existingViews[source.SortingNumber] = value;
			value.transform.SetSiblingIndex(source.SortingNumber - 1);
		}
		value.Set(source);
		if (!value.IsActive())
		{
			value.Display(display: true);
		}
		return value;
	}
}
