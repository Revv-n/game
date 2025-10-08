using System;

namespace GreenT.HornyScapes.Subscription;

[Serializable]
public class SubscriptionResponse
{
	public int sub_id;

	public string platform;

	public int monetization_id;

	public long start_time;

	public long expire_at;
}
