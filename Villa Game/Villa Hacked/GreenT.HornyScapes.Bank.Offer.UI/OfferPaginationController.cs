using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferPaginationController : OfferPreviewControllerBase
{
	private OfferSelector.Manager selectorManager;

	private int prevCount;

	[Inject]
	public void Init(OfferSelector.Manager selectorManager)
	{
		this.selectorManager = selectorManager;
	}

	public override void Display(OfferSettings offer)
	{
		if (base.VisibleOffers.Count > 1)
		{
			if (prevCount == 1)
			{
				selectorManager.Display(base.VisibleOffers[0]);
			}
			selectorManager.Display(offer);
		}
		prevCount = base.VisibleOffers.Count;
	}

	protected override void Replace(OfferSettings offer, OfferSettings next)
	{
		base.Replace(offer, next);
		selectorManager.GetViewOrDefault(offer)?.Set(next);
	}

	protected override void Hide(OfferSettings current)
	{
		base.Hide(current);
		OfferSelector viewOrDefault = selectorManager.GetViewOrDefault(current);
		if (viewOrDefault != null && !viewOrDefault.Button.IsInteractable())
		{
			DisplayNextOffer(current.SortingNumber);
		}
		if (base.VisibleOffers.Count > 1)
		{
			selectorManager.Hide(current);
		}
		else
		{
			selectorManager.HideAll();
		}
	}
}
