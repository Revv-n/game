using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Monetization;
using ModestTree;
using StripClub.Extensions;
using StripClub.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Subscription;

public class SubscriptionService : IInitializable, IDisposable
{
	public readonly Subject<bool> InitializeSubject = new Subject<bool>();

	private readonly User _user;

	private readonly IClock _clock;

	private readonly Purchaser _purchaser;

	private readonly GameStarter _gameStarter;

	private readonly SubscriptionStorage _storage;

	private readonly SubscriptionModelFactory _modelFactory;

	private readonly SubscriptionsActiveRequest _activeSubscriptionsRequest;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public SubscriptionService(SubscriptionStorage storage, GameStarter gameStarter, SubscriptionsActiveRequest activeSubscriptionsRequest, User user, SubscriptionModelFactory modelFactory, Purchaser purchaser, IClock clock)
	{
		_user = user;
		_clock = clock;
		_storage = storage;
		_purchaser = purchaser;
		_gameStarter = gameStarter;
		_modelFactory = modelFactory;
		_activeSubscriptionsRequest = activeSubscriptionsRequest;
	}

	public void Initialize()
	{
		(from status in (from user in _user.OnUpdate
				select user.PlayerID != null && !user.PlayerID.IsEmpty() into status
				where status
				select status).Take(1).SelectMany((bool _) => _gameStarter.IsGameActive)
			where status
			select status).Take(1).Subscribe(delegate
		{
			SetupLifecycle();
		}).AddTo(_compositeDisposable);
		_storage.OnNew.SelectMany((Func<SubscriptionModel, IObservable<SubscriptionModel>>)OnShouldExpire).Subscribe(delegate(SubscriptionModel model)
		{
			_storage.Remove(model);
		}).AddTo(_compositeDisposable);
	}

	private void SetupLifecycle()
	{
	}

	private IObservable<SubscriptionModel> OnShouldExpire(SubscriptionModel subscriptionModel)
	{
		return from _ in (from response in subscriptionModel.Duration.OnTimeIsUp.Delay(TimeSpan.FromSeconds(2.0)).SelectMany((GenericTimer _) => _activeSubscriptionsRequest.GetRequest()).Do(FetchAll)
				where response.All((SubscriptionResponse item) => item.sub_id != subscriptionModel.BaseID)
				select response).Take(1)
			select subscriptionModel;
	}

	private void FetchAll(List<SubscriptionResponse> responses)
	{
		foreach (SubscriptionResponse response in responses)
		{
			Activate(response);
		}
	}

	private void Setup(List<SubscriptionResponse> responses)
	{
		foreach (SubscriptionResponse response in responses)
		{
			Activate(response);
		}
		InitializeSubject.OnNext(value: true);
	}

	private void Activate(SubscriptionResponse response)
	{
		SubscriptionModel subscriptionModel = _storage.Collection.FirstOrDefault((SubscriptionModel item) => item.BaseID == response.sub_id);
		if (subscriptionModel != null)
		{
			long duration = response.expire_at - subscriptionModel.LocalStartTime;
			subscriptionModel.Update(duration);
			return;
		}
		SubscriptionModel subscriptionModel2 = _modelFactory.Create(response.sub_id);
		long num = _clock.GetTime().ConvertToUnixTimestamp();
		long num2 = num - response.start_time;
		long duration2 = response.expire_at - response.start_time - num2;
		subscriptionModel2.Activate(duration2, num);
		_storage.Add(subscriptionModel2);
	}

	public void Dispose()
	{
		_compositeDisposable?.Dispose();
	}
}
