using GreenT.HornyScapes.Lootboxes;
using StripClub.Model;
using StripClub.Model.Shop.UI;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class SubscriptionContentView : MonoView
{
	[SerializeField]
	private Transform _boosterOptionsContainer;

	[SerializeField]
	private Transform _immediateOptionsContainer;

	[SerializeField]
	private Transform _rechargeOptionsContainer;

	private SmallCardsViewManager _boosterViewManager;

	private SmallCardsViewManager _immediateViewManager;

	private SmallCardsViewManager _rechargeViewManager;

	[Inject]
	public void Init(SubscriptionBoosterViewManager boosterViewManager, SubscriptionImmediateViewManager immediateViewManager, SubscriptionRechargeViewManager rechargeViewManager)
	{
		_boosterViewManager = boosterViewManager;
		_immediateViewManager = immediateViewManager;
		_rechargeViewManager = rechargeViewManager;
	}

	public void SetImmediate(LinkedContent immediateContent)
	{
		bool active = false;
		if (immediateContent is LootboxLinkedContent)
		{
			foreach (DropSettings item in ((LootboxLinkedContent)immediateContent).Lootbox.GuarantedDrop)
			{
				_immediateViewManager.Display(item);
				active = true;
			}
		}
		_immediateOptionsContainer.gameObject.SetActive(active);
	}

	public void SetBooster(LinkedContent boosterContent)
	{
		bool active = false;
		if (boosterContent is LootboxLinkedContent)
		{
			foreach (DropSettings item in ((LootboxLinkedContent)boosterContent).Lootbox.GuarantedDrop)
			{
				_boosterViewManager.Display(item);
				active = true;
			}
		}
		_boosterOptionsContainer.gameObject.SetActive(active);
	}

	public void SetRecharge(LinkedContent rechargeContent)
	{
		bool active = false;
		if (rechargeContent is LootboxLinkedContent)
		{
			foreach (DropSettings item in ((LootboxLinkedContent)rechargeContent).Lootbox.GuarantedDrop)
			{
				_rechargeViewManager.Display(item);
				active = true;
			}
		}
		_rechargeOptionsContainer.gameObject.SetActive(active);
	}

	public void HideAll()
	{
		_boosterViewManager.HideAll();
		_immediateViewManager.HideAll();
		_rechargeViewManager.HideAll();
	}
}
