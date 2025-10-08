using System;
using System.Collections.Generic;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public abstract class EventRewardCard : MonoView<EventReward>
{
	[SerializeField]
	private EventRewardArrow arrow;

	[SerializeField]
	private LocalizedTextMeshPro title;

	[SerializeField]
	private LocalizedTextMeshPro description;

	[SerializeField]
	private EventProgressSlider slider;

	[SerializeField]
	private List<StatableComponent> statableComponents;

	[SerializeField]
	private List<StatableComponent> windowDependenciesStatableComponents;

	private IReadOnlyReactiveProperty<int> _currencyTarget;

	protected GameSettings _settings;

	private IDisposable _rewardedStream;

	private IDisposable _targetStream;

	private readonly CompositeDisposable _inProgressStream = new CompositeDisposable();

	[Inject]
	private void Construct(GameSettings settings, ICurrencyProcessor currencyProcessor)
	{
		_settings = settings;
		_currencyTarget = currencyProcessor.GetCountReactiveProperty(CurrencyType.EventXP);
	}

	private void OnDestroy()
	{
		_rewardedStream?.Dispose();
	}

	public override void Set(EventReward source)
	{
		base.Set(source);
		title.Init(source.Content.GetName());
		description.Init(source.Content.GetDescription());
		_rewardedStream?.Dispose();
		_rewardedStream = ObservableExtensions.Subscribe<EntityStatus>((IObservable<EntityStatus>)source.State, (Action<EntityStatus>)SetState);
		SetState(source.State.Value);
		arrow.Set(source, source.BundleTarget);
		Subscribe();
	}

	public void SetWindowDependenciesState(int state)
	{
		foreach (StatableComponent windowDependenciesStatableComponent in windowDependenciesStatableComponents)
		{
			windowDependenciesStatableComponent.Set(state);
		}
	}

	private void SetProgress()
	{
		if (base.Source.State.Value == EntityStatus.Blocked)
		{
			slider.Init(0f, base.Source.Target, 0f);
		}
		else
		{
			slider.SetProgress(base.Source, _currencyTarget.Value);
		}
	}

	private void SetState(EntityStatus state)
	{
		int stateNumber = GetStateNumber(state);
		SetProgress();
		arrow.Set(stateNumber);
		SetViewState(stateNumber);
	}

	public void SetViewState(int stateNumber)
	{
		statableComponents.ForEach(delegate(StatableComponent _item)
		{
			_item.Set(stateNumber);
		});
	}

	public int GetStateNumber(EntityStatus state)
	{
		switch (state)
		{
		case EntityStatus.Blocked:
			return 0;
		case EntityStatus.InProgress:
		case EntityStatus.Complete:
			return 1;
		case EntityStatus.Rewarded:
			return 2;
		default:
			throw new Exception();
		}
	}

	private void Subscribe()
	{
		_inProgressStream.Clear();
		OnProgressSetSlider();
		OnSetRewarded();
		_targetStream?.Dispose();
		OnTargetChangeSetSlider();
	}

	private void OnTargetChangeSetSlider()
	{
		_targetStream = ObservableExtensions.Subscribe<int>(Observable.Where<int>((IObservable<int>)_currencyTarget, (Func<int, bool>)((int _) => base.Source.State.Value == EntityStatus.InProgress)), (Action<int>)delegate
		{
			SetProgress();
		});
	}

	private void OnSetRewarded()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<EntityStatus>(Observable.Take<EntityStatus>(Observable.Where<EntityStatus>((IObservable<EntityStatus>)base.Source.State, (Func<EntityStatus, bool>)((EntityStatus _state) => _state == EntityStatus.Rewarded)), 1), (Action<EntityStatus>)delegate
		{
			SetProgress();
		}), (ICollection<IDisposable>)_inProgressStream);
	}

	private void OnProgressSetSlider()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<EntityStatus>(Observable.Where<EntityStatus>((IObservable<EntityStatus>)base.Source.State, (Func<EntityStatus, bool>)((EntityStatus _state) => _state == EntityStatus.InProgress)), (Action<EntityStatus>)delegate
		{
			SetProgress();
		}), (ICollection<IDisposable>)_inProgressStream);
	}
}
