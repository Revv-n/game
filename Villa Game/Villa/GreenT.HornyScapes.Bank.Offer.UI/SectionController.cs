using GreenT.HornyScapes.Bank.Data;
using StripClub.UI;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class SectionController : AbstractSectionController<LayoutType, OfferSettings, OfferSectionView>
{
	public override void LoadSection(OfferSettings model)
	{
		base.LoadSection(model);
		SetupTimer(model.DisplayTimeLocker.Timer);
	}

	protected override OfferSectionView GetSection()
	{
		return sectionManager.GetView(base.Source.Layout);
	}

	protected virtual void SetupTimer(GenericTimer offerTimer)
	{
		foreach (LotView displayedLot in base.CurrentSection.DisplayedLots)
		{
			MonoView<GenericTimer> component = displayedLot.gameObject.GetComponent<MonoView<GenericTimer>>();
			if (component != null)
			{
				component.Set(offerTimer);
			}
		}
	}
}
