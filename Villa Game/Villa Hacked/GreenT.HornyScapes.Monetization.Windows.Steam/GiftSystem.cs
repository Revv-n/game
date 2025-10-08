using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Shop;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class GiftSystem : IInitializable, IDisposable, IPurchaseNotifier
{
	private readonly SteamUnreceivedRequest _unreceivedRequest;

	private readonly LotManager _lotManager;

	private readonly GameStarter _gameStarter;

	private readonly IMonetizationRecorder<SteamPaymentData> _monetizationRecorder;

	private IDisposable offlinePaymentStream;

	private readonly CompositeDisposable _stream = new CompositeDisposable();

	public event Action<Lot> OnPurchaseLot;

	public GiftSystem(SteamUnreceivedRequest unreceivedRequest, LotManager lotManager, GameStarter gameStarter, IMonetizationRecorder<SteamPaymentData> monetizationRecorder)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_unreceivedRequest = unreceivedRequest;
		_lotManager = lotManager;
		_gameStarter = gameStarter;
		_monetizationRecorder = monetizationRecorder;
	}

	public void Initialize()
	{
		IObservable<SteamPaymentData> emitPaymentDataInGame = Observable.SelectMany<List<SteamPaymentData>, SteamPaymentData>(Observable.Select<List<SteamPaymentData>, List<SteamPaymentData>>(Observable.SelectMany<bool, List<SteamPaymentData>>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Func<bool, IObservable<List<SteamPaymentData>>>)((bool _) => _unreceivedRequest.Post())), (Func<List<SteamPaymentData>, List<SteamPaymentData>>)((List<SteamPaymentData> data) => data)), (Func<List<SteamPaymentData>, IEnumerable<SteamPaymentData>>)((List<SteamPaymentData> x) => x));
		ProcessObservablePaymentData(emitPaymentDataInGame);
	}

	private void ProcessObservablePaymentData(IObservable<SteamPaymentData> emitPaymentDataInGame)
	{
		emitPaymentDataInGame = Observable.Share<SteamPaymentData>(emitPaymentDataInGame);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SteamPaymentData>(Observable.Where<SteamPaymentData>(emitPaymentDataInGame, (Func<SteamPaymentData, bool>)((SteamPaymentData _data) => !TryGetLotByData(_data, out var _))), (Action<SteamPaymentData>)AbortValidate), (ICollection<IDisposable>)_stream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.SelectMany<SteamPaymentData, Unit>(Observable.Do<SteamPaymentData>(Observable.Where<SteamPaymentData>(emitPaymentDataInGame, (Func<SteamPaymentData, bool>)((SteamPaymentData _data) => TryGetLotByData(_data, out var _))), (Action<SteamPaymentData>)AddLotToPlayer, (Action<Exception>)delegate(Exception ex)
		{
			throw ex.LogException();
		}), (Func<SteamPaymentData, IObservable<Unit>>)((SteamPaymentData data) => _monetizationRecorder.Approve(data))), (Action<Unit>)delegate
		{
		}), (ICollection<IDisposable>)_stream);
	}

	private bool TryGetLotByData(SteamPaymentData data, out Lot lot)
	{
		lot = GetLot(data);
		return lot != null;
	}

	private void AbortValidate(SteamPaymentData data)
	{
		_ = GetType().Name + ": lot with id = " + data.item_id + " doesn't exist";
	}

	private void AddLotToPlayer(SteamPaymentData data)
	{
		Lot lot = GetLot(data);
		lot.Purchase();
		this.OnPurchaseLot?.Invoke(lot);
	}

	private Lot GetLot(SteamPaymentData data)
	{
		return _lotManager.Collection.FirstOrDefault((Lot _lot) => _lot.MonetizationID == int.Parse(data.item_id));
	}

	public void Dispose()
	{
		CompositeDisposable stream = _stream;
		if (stream != null)
		{
			stream.Dispose();
		}
	}
}
