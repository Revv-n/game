using System.Collections.Generic;
using GreenT.Data;
using Zenject;

namespace GreenT.HornyScapes.Subscription.Push;

public class SubscriptionPushNotifierFactory : IFactory<SubscriptionPushSettings, SubscriptionTimePushNotifier>, IFactory, IFactory<SubscriptionModel, SubscriptionDailyPushNotifier>
{
	private readonly ISaver _saver;

	private readonly DiContainer _container;

	private readonly TimeInstaller.TimerCollection _timerCollection;

	private SubscriptionPushNotifierFactory(DiContainer container, [InjectOptional] TimeInstaller.TimerCollection timerCollection, ISaver saver)
	{
		_saver = saver;
		_container = container;
		_timerCollection = timerCollection;
	}

	public SubscriptionTimePushNotifier Create(SubscriptionPushSettings settings)
	{
		SubscriptionTimePushNotifier subscriptionTimePushNotifier = _container.Instantiate<SubscriptionTimePushNotifier>((IEnumerable<object>)new object[1] { settings });
		_timerCollection.Add(subscriptionTimePushNotifier.Timer);
		return subscriptionTimePushNotifier;
	}

	public SubscriptionDailyPushNotifier Create(SubscriptionModel model)
	{
		SubscriptionDailyPushNotifier subscriptionDailyPushNotifier = _container.Instantiate<SubscriptionDailyPushNotifier>((IEnumerable<object>)new object[1] { model });
		_saver.Add(subscriptionDailyPushNotifier);
		_timerCollection.Add(subscriptionDailyPushNotifier.Timer);
		return subscriptionDailyPushNotifier;
	}
}
