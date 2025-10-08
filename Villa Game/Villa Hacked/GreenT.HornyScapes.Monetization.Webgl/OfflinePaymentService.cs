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
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		this.user = user;
		this.lotManager = lotManager;
		_lotOfflineProvider = lotOfflineProvider;
		this.invoicesFilteredRequest = invoicesFilteredRequest;
		_gameStarter = gameStarter;
	}

	public void Start()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_gameReadyToCheck.OnNext(Unit.Default);
	}

	public virtual void Initialize()
	{
		IObservable<PaymentIntentData> emitPaymentData = OnGetPaymentsObservable();
		ProcessObservablePaymentData(emitPaymentData);
	}

	protected virtual IObservable<PaymentIntentData> OnGetPaymentsObservable()
	{
		return Observable.SelectMany<List<PaymentIntentData>, PaymentIntentData>(Observable.Select<Response<List<PaymentIntentData>>, List<PaymentIntentData>>(Observable.Where<Response<List<PaymentIntentData>>>(Observable.SelectMany<User, Response<List<PaymentIntentData>>>(Observable.CombineLatest<User, Unit, User>(user.OnAuthorizedUser(), (IObservable<Unit>)_gameReadyToCheck, (Func<User, Unit, User>)((User user, Unit configLoaded) => user)), (Func<User, IObservable<Response<List<PaymentIntentData>>>>)((User _user) => invoicesFilteredRequest.GetRequest(_user.PlayerID))).Debug("OfflinePayment: Check invoice payments", LogType.Payments), (Func<Response<List<PaymentIntentData>>, bool>)((Response<List<PaymentIntentData>> _response) => _response?.Data != null && _response.Data.Any())), (Func<Response<List<PaymentIntentData>>, List<PaymentIntentData>>)((Response<List<PaymentIntentData>> _response) => _response.Data)), (Func<List<PaymentIntentData>, IEnumerable<PaymentIntentData>>)((List<PaymentIntentData> x) => x));
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
