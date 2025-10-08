using System;
using GreenT.HornyScapes.Data;
using GreenT.Types;

namespace StripClub.Model.Shop.Data;

[Serializable]
[Mapper]
public class SummonMapper : LotMapper
{
	public bool hot;

	public bool best;

	public string price_resource;

	public int price;

	public int? price_x10;

	public int reward;

	public int? reward_10;

	public int? first_reward;

	public int? first_reward_10;

	public int? free_unlock_time;

	public string view_type;

	public int[] drop_chances;

	public ContentSource content_source;

	public ContentType task_type;
}
