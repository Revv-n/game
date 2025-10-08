using System.Collections.Generic;
using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferClusterProviderOfferPanel : MonoBehaviour
{
	[SerializeField]
	private ContentType[] contentTypes;

	private List<IView<IEnumerable<OfferSettings>>> offerViews;

	private OfferManagerCluster offerCluster;

	[Inject]
	public void Init(OfferManagerCluster offerCluster, List<IView<IEnumerable<OfferSettings>>> offerViews)
	{
		this.offerCluster = offerCluster;
		this.offerViews = offerViews;
	}

	private void OnOpen()
	{
		IEnumerable<OfferSettings> offersForTypes = offerCluster.GetOffersForTypes(contentTypes);
		foreach (IView<IEnumerable<OfferSettings>> offerView in offerViews)
		{
			offerView.Set(offersForTypes);
		}
	}

	private void OnEnable()
	{
		OnOpen();
	}
}
