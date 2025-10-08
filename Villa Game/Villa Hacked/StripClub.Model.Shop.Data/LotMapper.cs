using System;

namespace StripClub.Model.Shop.Data;

[Serializable]
public abstract class LotMapper
{
	public int id;

	public int monetization_id;

	public int tab_id;

	public int position;

	public string source;

	public int buy_times;

	public UnlockType[] unlock_type;

	public string[] unlock_value;
}
