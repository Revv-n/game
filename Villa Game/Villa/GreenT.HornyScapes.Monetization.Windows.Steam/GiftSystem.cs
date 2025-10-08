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
		_unreceivedRequest = unreceivedRequest;
		_lotManager = lotManager;
		_gameStarter = gameStarter;
		_monetizationRecorder = monetizationRecorder;
	}

	public void Initialize()
	{
		IObservable<SteamPaymentData> emitPaymentDataInGame = (from data in _gameStarter.IsGameActive.Where((bool x) => x).SelectMany((bool _) => _unreceivedRequest.Post())
			select (data)).SelectMany((List<SteamPaymentData> x) => x);
		ProcessObservablePaymentData(emitPaymentDataInGame);
	}

	private void ProcessObservablePaymentData(IObservable<SteamPaymentData> emitPaymentDataInGame)
	{
		emitPaymentDataInGame = emitPaymentDataInGame.Share();
		emitPaymentDataInGame.Where((SteamPaymentData _data) => !TryGetLotByData(_data, out var _)).Subscribe(AbortValidate).AddTo(_stream);
		emitPaymentDataInGame.Where((SteamPaymentData _data) => TryGetLotByData(_data, out var _)).Do(AddLotToPlayer, delegate(Exception ex)
		{
			throw ex.LogException();
		}).SelectMany((SteamPaymentData data) => _monetizationRecorder.Approve(data))
			.Subscribe(delegate
			{
			})
			.AddTo(_stream);
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
		_stream?.Dispose();
	}
}
