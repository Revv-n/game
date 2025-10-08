using System.Linq;
using GreenT.HornyScapes.StarShop;
using Merge.Meta.RoomObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatStarShop : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private Button completeBtn;

	private StarShopManager starShopManager;

	private int targetIDs;

	[Inject]
	private void InnerInit(StarShopManager starShopManager)
	{
		this.starShopManager = starShopManager;
	}

	private void Awake()
	{
		inputField.onValueChanged.AddListener(OnEnterValue);
		completeBtn.onClick.AddListener(CompleteStarShops);
	}

	protected void OnEnterValue(string value)
	{
		int.TryParse(value, out targetIDs);
	}

	private void CompleteStarShops()
	{
		foreach (StarShopItem item in from _item in starShopManager.Collection.Take(targetIDs)
			where _item.State != EntityStatus.Rewarded
			select _item)
		{
			item.SetState(EntityStatus.Rewarded);
		}
		inputField.text = string.Empty;
	}
}
