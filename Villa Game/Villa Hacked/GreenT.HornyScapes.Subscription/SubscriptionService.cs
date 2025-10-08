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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Take<bool>(Observable.Where<bool>(Observable.SelectMany<bool, bool>(Observable.Take<bool>(Observable.Where<bool>(Observable.Select<User, bool>(_user.OnUpdate, (Func<User, bool>)((User user) => user.PlayerID != null && !LinqExtensions.IsEmpty<char>((IEnumerable<char>)user.PlayerID))), (Func<bool, bool>)((bool status) => status)), 1), (Func<bool, IObservable<bool>>)((bool _) => (IObservable<bool>)_gameStarter.IsGameActive)), (Func<bool, bool>)((bool status) => status)), 1), (Action<bool>)delegate
		{
			SetupLifecycle();
		}), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SubscriptionModel>(Observable.SelectMany<SubscriptionModel, SubscriptionModel>(_storage.OnNew, (Func<SubscriptionModel, IObservable<SubscriptionModel>>)OnShouldExpire), (Action<SubscriptionModel>)delegate(SubscriptionModel model)
		{
			_storage.Remove(model);
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	private void SetupLifecycle()
	{
	}

	private IObservable<SubscriptionModel> OnShouldExpire(SubscriptionModel subscriptionModel)
	{
		return Observable.Select<List<SubscriptionResponse>, SubscriptionModel>(Observable.Take<List<SubscriptionResponse>>(Observable.Where<List<SubscriptionResponse>>(Observable.Do<List<SubscriptionResponse>>(Observable.SelectMany<GenericTimer, List<SubscriptionResponse>>(Observable.Delay<GenericTimer>(subscriptionModel.Duration.OnTimeIsUp, TimeSpan.FromSeconds(2.0)), (Func<GenericTimer, IObservable<List<SubscriptionResponse>>>)((GenericTimer _) => _activeSubscriptionsRequest.GetRequest())), (Action<List<SubscriptionResponse>>)FetchAll), (Func<List<SubscriptionResponse>, bool>)((List<SubscriptionResponse> response) => response.All((SubscriptionResponse item) => item.sub_id != subscriptionModel.BaseID))), 1), (Func<List<SubscriptionResponse>, SubscriptionModel>)((List<SubscriptionResponse> _) => subscriptionModel));
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
		InitializeSubject.OnNext(true);
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
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Dispose();
		}
	}
}
