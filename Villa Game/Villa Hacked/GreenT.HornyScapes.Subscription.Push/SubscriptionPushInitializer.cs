using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Subscription.Push;

public class SubscriptionPushInitializer : StructureInitializerViaArray<SubscriptionPushMapper, SubscriptionPushSettings>
{
	public SubscriptionPushInitializer(IManager<SubscriptionPushSettings> manager, IFactory<SubscriptionPushMapper, SubscriptionPushSettings> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
