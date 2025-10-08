using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Subscription;

public class SubscriptionActiveLocker : Locker
{
	public enum SubscriptionStatus
	{
		Active,
		NotActive
	}

	public readonly int ID;

	private readonly SubscriptionStatus _condition;

	public SubscriptionActiveLocker(int id, SubscriptionStatus condition)
	{
		ID = id;
		_condition = condition;
	}

	public void Set(bool value)
	{
		ReactiveProperty<bool> reactiveProperty = isOpen;
		reactiveProperty.Value = _condition switch
		{
			SubscriptionStatus.Active => value, 
			SubscriptionStatus.NotActive => !value, 
			_ => isOpen.Value, 
		};
	}

	public void Set(int subscription_id, bool condition)
	{
		if (ID == subscription_id)
		{
			Set(condition);
		}
	}
}
