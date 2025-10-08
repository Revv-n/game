using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.Types;
using StripClub.Model;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Services;

public class RelationshipStatusTracker : IDisposable
{
	private Currencies _currencies;

	private IDisposable _trackStream;

	private Relationship _relationship;

	private ReactiveProperty<int> _currency;

	private readonly Subject<(int RelationshipId, int StatusIndex)> _changed = new Subject<(int, int)>();

	public IObservable<(int RelationshipId, int StatusIndex)> Changed => _changed.AsObservable();

	[Inject]
	public void Init(Currencies currencies)
	{
		_currencies = currencies;
	}

	public void Set(Relationship relationship)
	{
		_relationship = relationship;
		_trackStream?.Dispose();
		CompositeIdentificator identificator = new CompositeIdentificator(_relationship.ID);
		_currency = _currencies.Get(CurrencyType.LovePoints, identificator);
		EmitProgressRewardStatus();
		_trackStream = _relationship.Rewards.ToObservable().SelectMany((IReadOnlyList<RewardWithManyConditions> x) => x.FirstOrDefault().State).Merge()
			.Subscribe(delegate
			{
				EmitProgressRewardStatus();
			});
	}

	public void Dispose()
	{
		_trackStream?.Dispose();
		_changed.Dispose();
	}

	private void EmitProgressRewardStatus()
	{
		IReadOnlyList<IReadOnlyList<RewardWithManyConditions>> rewards = _relationship.Rewards;
		int value = _currency.Value;
		int num = -1;
		for (int i = 0; i < rewards.Count; i++)
		{
			if (i == rewards.Count - 1)
			{
				if (_relationship.GetRequiredPointsForReward(i) <= value)
				{
					num++;
				}
				_changed.OnNext((_relationship.ID, num));
				break;
			}
			int requiredPointsForReward = _relationship.GetRequiredPointsForReward(i);
			if (value < requiredPointsForReward)
			{
				_changed.OnNext((_relationship.ID, num));
				break;
			}
			num++;
		}
	}
}
