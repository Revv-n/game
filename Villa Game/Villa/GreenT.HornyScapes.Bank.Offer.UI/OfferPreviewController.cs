using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferPreviewController : OfferPreviewControllerBase
{
	private IViewManager<OfferSettings, OfferPreview> viewManager;

	[Inject]
	public void Init(IViewManager<OfferSettings, OfferPreview> viewManager)
	{
		this.viewManager = viewManager;
	}

	public override void Display(OfferSettings _offer)
	{
		viewManager.Display(_offer);
	}

	protected override void Hide(OfferSettings current)
	{
		base.Hide(current);
		OfferPreview offerPreview = viewManager.Display(current);
		if (offerPreview != null)
		{
			offerPreview.Display(display: false);
		}
	}

	protected override void Replace(OfferSettings offer, OfferSettings next)
	{
		base.Replace(offer, next);
		OfferPreview offerPreview = viewManager.Display(offer);
		if (offerPreview != null)
		{
			offerPreview.Set(next);
		}
	}
}
