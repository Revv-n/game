using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferMergePreviewAdapter : MonoBehaviour
{
	private OfferMergePreviewController controller;

	[Inject]
	public void Init(OfferMergePreviewController controller)
	{
		this.controller = controller;
	}

	public void SwipeHandler(Vector2Int swipeDirection)
	{
		if (swipeDirection == Vector2Int.right)
		{
			controller.SwitchOffer(forward: false);
		}
		else if (swipeDirection == Vector2Int.left)
		{
			controller.SwitchOffer(forward: true);
		}
	}
}
