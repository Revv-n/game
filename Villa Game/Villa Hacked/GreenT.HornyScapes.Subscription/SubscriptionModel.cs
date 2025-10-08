using System;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.Subscription;

public class SubscriptionModel
{
	public readonly int BaseID;

	public readonly GenericTimer Duration = new GenericTimer();

	public readonly Subject<Unit> OnActivated = new Subject<Unit>();

	public long LocalStartTime { get; private set; }

	public SubscriptionModel(int baseID)
	{
		BaseID = baseID;
	}

	public void Activate(long duration, long startUnix)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Duration.Start(TimeSpan.FromSeconds(duration));
		OnActivated.OnNext(default(Unit));
		LocalStartTime = startUnix;
	}

	public void Update(long duration)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (Duration.IsActive.Value)
		{
			Duration.TimeLeft = TimeSpan.FromSeconds(duration);
		}
		else
		{
			Duration.Start(TimeSpan.FromSeconds(duration));
		}
		OnActivated.OnNext(default(Unit));
	}
}
