using System;
using Newtonsoft.Json;

namespace GreenT.HornyScapes.Monetization;

[Serializable]
public class PaymentIntentData
{
	public enum PaymentStatus
	{
		succeeded,
		failed,
		aborted
	}

	public string ID;

	public string InvoiceID;

	public string TransactionId;

	public string PlayerID;

	public string Status;

	public string AppName;

	public string Bundle;

	public int ItemID;

	public bool Received;

	[JsonProperty(PropertyName = "nutaku_payment_id")]
	private string NutakuInvoiceID
	{
		set
		{
			InvoiceID = value;
		}
	}

	[JsonProperty(PropertyName = "transaction_id")]
	private string transaction_id
	{
		set
		{
			TransactionId = value;
		}
	}

	[JsonProperty(PropertyName = "item_id")]
	private int item_id
	{
		set
		{
			ItemID = value;
		}
	}

	public bool IsSucceeded => string.CompareOrdinal(Status, PaymentStatus.succeeded.ToString()) == 0;

	public PaymentIntentData()
	{
	}

	public PaymentIntentData(string iD, string invoiceID, string playerID, string status, string appName, string bundle, int itemID, bool received)
	{
		ID = iD;
		InvoiceID = invoiceID;
		PlayerID = playerID;
		Status = status;
		AppName = appName;
		Bundle = bundle;
		ItemID = itemID;
		Received = received;
	}

	public override string ToString()
	{
		return base.ToString() + " Item ID: " + ItemID + "\n GreenT.Invoice: " + ID + "\n Outer.Invoice: " + InvoiceID;
	}
}
