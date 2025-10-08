using System;
using Merge.Meta.RoomObjects;

namespace GreenT.HornyScapes.Meta.Decorations;

public class DecorationController : IDisposable
{
	private readonly DecorationSubscriptions _subscriptions;

	public DecorationController(DecorationSubscriptions subscriptions)
	{
		_subscriptions = subscriptions;
	}

	public void Initialize(bool isGameActive)
	{
		_subscriptions.Activate(isGameActive);
	}

	public void SetRewarded(Decoration decoration)
	{
		decoration.SetState(EntityStatus.Rewarded);
	}

	public void Dispose()
	{
		_subscriptions.Dispose();
	}
}
