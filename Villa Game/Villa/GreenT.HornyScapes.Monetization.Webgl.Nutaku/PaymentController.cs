using System;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Webgl.Nutaku;

public class PaymentController : MonoBehaviour
{
	private Connection connection;

	[Inject]
	private void InnerInit(Connection connection)
	{
		this.connection = connection;
	}

	public void Buy(int monetizationID, string itemName, int itemId, string price, string itemDescription, string itemImageURL, Action<Transaction> onTransactionComplete)
	{
		connection.Buy(monetizationID, itemName, itemId, price, itemDescription, itemImageURL, onTransactionComplete);
	}

	[EditorButton]
	public void OnPurchaseComplete(string purchaseParams)
	{
		purchaseParams = purchaseParams.Trim('\'');
		PurchaseParams purchaseParams2 = JsonConvert.DeserializeObject<PurchaseParams>(purchaseParams);
		if (purchaseParams2 != null)
		{
			connection?.OnPurchaseComplete(purchaseParams2.monetizationID);
		}
	}

	[EditorButton]
	public void OnPurchaseFailed(string purchaseParams)
	{
		purchaseParams = purchaseParams.Trim('\'');
		PurchaseParams purchaseParams2 = JsonConvert.DeserializeObject<PurchaseParams>(purchaseParams);
		if (purchaseParams2 != null)
		{
			connection?.OnPurchaseFailed(purchaseParams2.monetizationID, purchaseParams2.message);
		}
	}
}
