using System;

namespace GreenT.HornyScapes.Monetization;

[Serializable]
public class SteamPaymentData
{
	public string id;

	public long order_id;

	public string steam_id;

	public string player_id;

	public string transaction_id;

	public string app_id;

	public string item_id;

	public int qty;

	public int amount;

	public bool dev;

	public string status;

	public bool received;

	public int time_int;
}
