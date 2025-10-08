using System;
using System.Collections.Generic;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using UniRx;
using Zenject;

namespace StripClub.Model;

public class SummonUseTracker : IDisposable, IInitializable
{
	public struct Infocase
	{
		public readonly int AddCount;

		public readonly int LotId;

		public Infocase(int lotId, int addCount)
		{
			LotId = lotId;
			AddCount = addCount;
		}
	}

	private const int WholesaleCount = 10;

	private const int BaseCount = 1;

	private readonly Subject<Infocase> _onSummon = new Subject<Infocase>();

	private readonly SignalBus _signalBus;

	private readonly CompositeDisposable _trackStream = new CompositeDisposable();

	public IObservable<Infocase> OnSummon => (IObservable<Infocase>)_onSummon;

	public SummonUseTracker(SignalBus signalBus)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_signalBus = signalBus;
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SummonLot>(Observable.Select<LotBoughtSignal, SummonLot>(Observable.Where<LotBoughtSignal>(_signalBus.GetStream<LotBoughtSignal>(), (Func<LotBoughtSignal, bool>)((LotBoughtSignal x) => x.Lot is SummonLot)), (Func<LotBoughtSignal, SummonLot>)((LotBoughtSignal x) => x.Lot as SummonLot)), (Action<SummonLot>)delegate(SummonLot x)
		{
			Add((!x.CurrentWholesale) ? 1 : 10, x.ID);
		}), (ICollection<IDisposable>)_trackStream);
	}

	private void Add(int count, int id)
	{
		_onSummon.OnNext(new Infocase(id, count));
	}

	public void Dispose()
	{
		_trackStream.Dispose();
	}
}
