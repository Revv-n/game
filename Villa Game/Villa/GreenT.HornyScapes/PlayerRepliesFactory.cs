using System;
using GreenT.Data;
using GreenT.HornyScapes.Constants;
using StripClub.Extensions;
using Zenject;

namespace GreenT.HornyScapes;

public class PlayerRepliesFactory : PlayerRestorableVariablesFactory, IFactory<RestorableValue<int>>, IFactory
{
	private readonly IConstants<int> intConstants;

	private readonly ISaver saver;

	private RestorableValue<int> replies;

	public PlayerRepliesFactory(IClock clock, IConstants<int> intConstants, ISaver saver, EnergyLoadContainer loadContainer)
		: base(clock, loadContainer)
	{
		this.intConstants = intConstants;
		this.saver = saver;
	}

	public RestorableValue<int> Create()
	{
		int num = intConstants["start_messages"];
		int num2 = intConstants["time_message_restore"];
		int num3 = 1;
		int maxValue = intConstants["max_messages"];
		if (replies == null)
		{
			replies = Create(num, num2, num3, maxValue, "Messages");
			saver.Add(replies);
		}
		else
		{
			Initialize(num, num2, num3, maxValue);
		}
		return replies;
	}

	private void Initialize(int value, int restoreTime, int amountPerTick, int maxValue)
	{
		replies.SetForce(value);
		replies.UpdateBounds(maxValue, 0);
		replies.AmountPerTick = amountPerTick;
		replies.RestorePeriod = TimeSpan.FromSeconds(restoreTime);
	}
}
