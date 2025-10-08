using System.Collections.Generic;
using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using StripClub.UI;

namespace GreenT.HornyScapes.Bank.Event;

public class OfferContentSelector : IContentSelector, ISelector<ContentType>
{
	private readonly IDictionary<ContentType, OfferSettings.Manager> managerCluster;

	private readonly List<IView<OfferSettings.Manager>> offerViews;

	public ContentType Current { get; private set; }

	public OfferContentSelector(IDictionary<ContentType, OfferSettings.Manager> managerCluster, List<IView<OfferSettings.Manager>> offerViews)
	{
		this.managerCluster = managerCluster;
		this.offerViews = offerViews;
	}

	public void Select(ContentType selector)
	{
		Current = selector;
		OfferSettings.Manager param = managerCluster[selector];
		foreach (IView<OfferSettings.Manager> offerView in offerViews)
		{
			offerView.Set(param);
		}
	}
}
