using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

public class EventReward : NotifyReward
{
	public readonly Sprite BundleTarget;

	public EventReward(LinkedContent content, int target, Sprite bundleTarget, EventReward prevReward)
		: base(content, target, prevReward)
	{
		BundleTarget = bundleTarget;
	}
}
