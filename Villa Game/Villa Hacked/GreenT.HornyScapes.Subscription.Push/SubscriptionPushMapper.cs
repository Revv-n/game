using GreenT.HornyScapes.Data;
using StripClub.Model;

namespace GreenT.HornyScapes.Subscription.Push;

[Mapper]
public class SubscriptionPushMapper
{
	public int id;

	public int[] subscription_id;

	public int push_time;

	public int? push_offset;

	public int go_to;

	public string header_localization;

	public UnlockType[] unlock_type;

	public string[] unlock_value;
}
