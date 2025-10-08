using System;
using GreenT.Types;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class AnySummonObjective : GainObjective, IDisposable
{
	protected readonly SignalBus signalBus;

	protected readonly ContentType contentType;

	protected IDisposable signalStream;

	public AnySummonObjective(Func<Sprite> iconProvider, SavableObjectiveData data, SignalBus signalBus, ContentType contentType)
		: base(iconProvider, data)
	{
		this.signalBus = signalBus;
		this.contentType = contentType;
	}

	public override void Track()
	{
		base.Track();
		signalStream?.Dispose();
		signalStream = (from _lot in signalBus.GetStream<LotBoughtSignal>()
			where _lot.Lot is SummonLot summonLot && summonLot.TaskType == contentType
			select _lot.Lot as SummonLot).Subscribe(delegate(SummonLot _lot)
		{
			AddProgress(_lot.CurrentWholesale);
		});
	}

	public override void OnRewardTask()
	{
		base.OnRewardTask();
		signalStream?.Dispose();
	}

	public void Dispose()
	{
		signalStream?.Dispose();
	}

	protected void AddProgress(bool wholesale)
	{
		Data.Progress += ((!wholesale) ? 1 : 10);
		onUpdate.OnNext(this);
	}
}
