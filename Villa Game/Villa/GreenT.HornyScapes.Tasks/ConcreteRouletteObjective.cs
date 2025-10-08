using System;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public sealed class ConcreteRouletteObjective : GainObjective, IDisposable
{
	public readonly int RouletteId;

	private const int SINGLE_PURCHASE_POINTS = 1;

	private const int WHOLESALE_PURCHASE_POINTS = 10;

	private readonly SignalBus _signalBus;

	private IDisposable _signalStream;

	public ConcreteRouletteObjective(Func<Sprite> iconProvider, SavableObjectiveData data, SignalBus signalBus, int rouletteId)
		: base(iconProvider, data)
	{
		RouletteId = rouletteId;
		_signalBus = signalBus;
	}

	public void Dispose()
	{
		_signalStream?.Dispose();
	}

	public override void Track()
	{
		base.Track();
		_signalStream?.Dispose();
		_signalStream = (from _lot in _signalBus.GetStream<RouletteLotBoughtSignal>().Where(delegate(RouletteLotBoughtSignal _lot)
			{
				RouletteLot lot = _lot.Lot;
				return lot != null && lot.ID == RouletteId;
			})
			select _lot.Lot).Subscribe(delegate(RouletteLot _lot)
		{
			AddProgress(_lot.Wholesale);
		});
	}

	public override void OnRewardTask()
	{
		base.OnRewardTask();
		_signalStream?.Dispose();
	}

	private void AddProgress(bool wholesale)
	{
		Data.Progress += ((!wholesale) ? 1 : 10);
		onUpdate.OnNext(this);
	}
}
