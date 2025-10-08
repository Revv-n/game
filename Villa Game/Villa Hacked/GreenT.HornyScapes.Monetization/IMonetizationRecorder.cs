using System;
using UniRx;

namespace GreenT.HornyScapes.Monetization;

public interface IMonetizationRecorder<in TData> : IMonetizationRecorder
{
	void Record(TData data);

	IObservable<Unit> Approve(TData data);
}
public interface IMonetizationRecorder
{
	IObservable<Unit> ApproveLast();
}
