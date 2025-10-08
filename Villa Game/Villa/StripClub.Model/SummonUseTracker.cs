using System;
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

	public IObservable<Infocase> OnSummon => _onSummon;

	public SummonUseTracker(SignalBus signalBus)
	{
		_signalBus = signalBus;
	}

	public void Initialize()
	{
		(from x in _signalBus.GetStream<LotBoughtSignal>()
			where x.Lot is SummonLot
			select x.Lot as SummonLot).Subscribe(delegate(SummonLot x)
		{
			Add((!x.CurrentWholesale) ? 1 : 10, x.ID);
		}).AddTo(_trackStream);
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
