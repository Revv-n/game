using Zenject;

namespace GreenT.HornyScapes.Subscription;

public class SubscriptionModelFactory : IFactory<int, SubscriptionModel>, IFactory
{
	private readonly DiContainer _container;

	public SubscriptionModelFactory(DiContainer container)
	{
		_container = container;
	}

	public SubscriptionModel Create(int baseID)
	{
		return _container.Instantiate<SubscriptionModel>(new object[1] { baseID });
	}
}
