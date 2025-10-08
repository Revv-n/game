using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.HornyScapes.Relationships.Providers;
using GreenT.HornyScapes.Relationships.Services;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Views;

public sealed class RelationshipStatusView : MonoView<ICard>
{
	[SerializeField]
	private RelationshipLevelView _relationshipLevelView;

	[SerializeField]
	private StatusViewController _statusViewController;

	private RelationshipProvider _relationshipProvider;

	private Relationship _relationship;

	private RelationshipStatusTracker _relationshipStatusTracker;

	private RelationshipMapperProvider _relationshipMapperProvider;

	private RelationshipRewardMapperProvider _relationshipRewardMapperProvider;

	private Currencies _currencies;

	private ReactiveProperty<int> _currency;

	private IDisposable _statusStream;

	[Inject]
	public void Init(RelationshipProvider relationshipProvider, RelationshipStatusTracker relationshipStatusTracker, RelationshipMapperProvider relationshipMapperProvider, RelationshipRewardMapperProvider relationshipRewardMapperProvider, Currencies currencies)
	{
		_relationshipProvider = relationshipProvider;
		_relationshipStatusTracker = relationshipStatusTracker;
		_relationshipMapperProvider = relationshipMapperProvider;
		_relationshipRewardMapperProvider = relationshipRewardMapperProvider;
		_currencies = currencies;
	}

	public override void Set(ICard source)
	{
		base.Set(source);
		_statusViewController.SetStatus(1, 1);
		_relationshipLevelView.Set(1);
		TrackRelationshipStatus();
	}

	public void ForceSetStatus(int relationshipId, int statusIndex)
	{
		_statusStream?.Dispose();
		_relationship = _relationshipProvider.Get(relationshipId);
		CompositeIdentificator identificator = new CompositeIdentificator(_relationship.ID);
		_currency = _currencies.Get(CurrencyType.LovePoints, identificator);
		SetStatus((RelationshipId: relationshipId, StatusIndex: statusIndex));
	}

	private void OnDestroy()
	{
		_statusStream?.Dispose();
	}

	private void TrackRelationshipStatus()
	{
		_statusStream?.Dispose();
		int relationsipId = (base.Source as GreenT.HornyScapes.Characters.CharacterInfo).RelationsipId;
		_relationship = _relationshipProvider.Get(relationsipId);
		if (_relationship != null)
		{
			CompositeIdentificator identificator = new CompositeIdentificator(_relationship.ID);
			_currency = _currencies.Get(CurrencyType.LovePoints, identificator);
			SetStatus((RelationshipId: _relationship.ID, StatusIndex: GetCurrentStatusRewardIndex()));
			_statusStream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(int, int)>(_relationshipStatusTracker.Changed, (Action<(int, int)>)SetStatus), (Component)this);
		}
	}

	private void SetStatus((int RelationshipId, int StatusIndex) info)
	{
		if (_relationship.ID != info.RelationshipId)
		{
			return;
		}
		int item = info.StatusIndex;
		if (item < 0)
		{
			return;
		}
		int num = 1;
		List<int> rewardStatuses = GetRewardStatuses();
		int count = rewardStatuses[item];
		int num2 = int.MaxValue;
		for (int i = 0; i < item + 1; i++)
		{
			int num3 = rewardStatuses[i];
			if (num3 < num2)
			{
				num++;
			}
			num2 = num3;
		}
		_statusViewController.SetStatus(num, count);
		_relationshipLevelView.Set(num);
	}

	private List<int> GetRewardStatuses()
	{
		int[] rewards = _relationshipMapperProvider.Get(_relationship.ID).rewards;
		List<int> list = new List<int>(rewards.Length);
		int[] array = rewards;
		foreach (int id in array)
		{
			RelationshipRewardMapper relationshipRewardMapper = _relationshipRewardMapperProvider.Get(id);
			list.Add(relationshipRewardMapper.status_number);
		}
		return list;
	}

	private int GetCurrentStatusRewardIndex()
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
				return num;
			}
			int requiredPointsForReward = _relationship.GetRequiredPointsForReward(i);
			if (value < requiredPointsForReward)
			{
				return num;
			}
			num++;
		}
		return num;
	}
}
