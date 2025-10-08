using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.UI;
using Merge.Meta.RoomObjects;
using StripClub.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Views;

public sealed class StatusViewControllerChecker : MonoView<IReadOnlyList<RewardWithManyConditions>>
{
	[SerializeField]
	private ShowObjectStatable _viewState;

	private bool _wasBlocked;

	private IDisposable _stateStream;

	public override void Set(IReadOnlyList<RewardWithManyConditions> source)
	{
		base.Set(source);
		_stateStream?.Dispose();
		if (base.Source != null)
		{
			_stateStream = ObservableExtensions.Subscribe<EntityStatus>((IObservable<EntityStatus>)base.Source[0].State, (Action<EntityStatus>)UpdateState);
		}
		_wasBlocked = false;
	}

	public void ForceUpdateState()
	{
		_wasBlocked = false;
		UpdateState(base.Source[0].State.Value);
	}

	private void UpdateState(EntityStatus state)
	{
		switch (state)
		{
		case EntityStatus.Blocked:
			_wasBlocked = true;
			_viewState.Set(0);
			break;
		case EntityStatus.InProgress:
			if (!_wasBlocked)
			{
				_viewState.Set(1);
			}
			break;
		case EntityStatus.Complete:
			_viewState.Set(2);
			break;
		case EntityStatus.Rewarded:
			_viewState.Set(3);
			break;
		}
	}
}
