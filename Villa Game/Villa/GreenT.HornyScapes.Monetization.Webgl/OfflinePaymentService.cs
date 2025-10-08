using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Net;
using StripClub.Model.Shop;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Webgl;

public abstract class OfflinePaymentService : IInitializable, IOfflineServiceStarter, IPurchaseNotifier, IDisposable
{
	private readonly User user;

	private readonly LotManager lotManager;

	private readonly InvoicesFilteredRequest invoicesFilteredRequest;

	private readonly GameStarter _gameStarter;

	private readonly LotOfflineProvider _lotOfflineProvider;

	protected readonly Subject<Unit> _gameReadyToCheck = new Subject<Unit>();

	private readonly CompositeDisposable requestsStream = new CompositeDisposable();

	private IDisposable _offlinePaymentStream;

	public event Action<Lot> OnPurchaseLot = delegate
	{
	};

	public OfflinePaymentService(User user, LotManager lotManager, LotOfflineProvider lotOfflineProvider, InvoicesFilteredRequest invoicesFilteredRequest, GameStarter gameStarter)
	{
		this.user = user;
		this.lotManager = lotManager;
		_lotOfflineProvider = lotOfflineProvider;
		this.invoicesFilteredRequest = invoicesFilteredRequest;
		_gameStarter = gameStarter;
	}

	public void Start()
	{
		_gameReadyToCheck.OnNext(Unit.Default);
	}

	public virtual void Initialize()
	{
		IObservable<PaymentIntentData> emitPaymentData = OnGetPaymentsObservable();
		ProcessObservablePaymentData(emitPaymentData);
	}

	protected virtual IObservable<PaymentIntentData> OnGetPaymentsObservable()
	{
		return (from _response in user.OnAuthorizedUser().CombineLatest(_gameReadyToCheck, (User user, Unit configLoaded) => user).SelectMany((User _user) => invoicesFilteredRequest.GetRequest(_user.PlayerID))
				.Debug("OfflinePayment: Check invoice payments", LogType.Payments)
			where _response?.Data != null && _response.Data.Any()
			select _response.Data).SelectMany((List<PaymentIntentData> x) => x);
	}

	protected void ProcessObservablePaymentData(IObservable<PaymentIntentData> emitPaymentData)
	{
	}

	protected virtual void OnIssueTheProductBought(PaymentIntentData data)
	{
		Lot lot = lotManager.Collection.FirstOrDefault((Lot _lot) => _lot.MonetizationID == data.ItemID);
		lot.Purchase();
		this.OnPurchaseLot(lot);
	}

	private bool HasLot(PaymentIntentData data)
	{
		return lotManager.HasLotWithMonetizationID(data.ItemID);
	}

	private void LogAbort(PaymentIntentData data)
	{
		_ = $"{GetType().Name}: lot with id = {data.ItemID} doesn't exist";
	}

	protected abstract IObservable<PaymentIntentData> NotifyServerAboutPurchase(PaymentIntentData data);

	protected abstract IObservable<PaymentIntentData> AbortPurchaseNotification(PaymentIntentData data);

	public virtual void Dispose()
	{
		_offlinePaymentStream?.Dispose();
		requestsStream.Dispose();
	}
}
