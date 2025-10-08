using System.Collections.Generic;
using GreenT.HornyScapes.Bank.Event;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Offer;
using GreenT.Types;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferClusterProviderMerge : MonoBehaviour
{
	[SerializeField]
	private ContentType contentRegion;

	[SerializeField]
	private ContentType[] types;

	private OfferMergePreviewController offerController;

	private OfferPaginationController paginationController;

	private OfferUnlockController unlockController;

	private BankContentSelector bankContentSelector;

	private OfferManagerCluster offerCluster;

	[Inject]
	public void Init(OfferManagerCluster offerCluster, OfferMergePreviewController offerController, OfferPaginationController paginationController, OfferUnlockController unlockController, BankContentSelector bankContentSelector)
	{
		this.offerCluster = offerCluster;
		this.offerController = offerController;
		this.paginationController = paginationController;
		this.bankContentSelector = bankContentSelector;
		this.unlockController = unlockController;
	}

	private void OnOpen()
	{
		if (contentRegion == bankContentSelector.GetActualType())
		{
			IEnumerable<OfferSettings> offersForTypes = offerCluster.GetOffersForTypes(types);
			offerController.Set(offersForTypes);
			paginationController.Set(offersForTypes);
			unlockController.Set(offersForTypes);
		}
	}

	private void OnEnable()
	{
		OnOpen();
	}
}
