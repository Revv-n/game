using System;
using UniRx;

namespace GreenT.HornyScapes.Monetization;

public class DefaultMonetizationRecorder : IMonetizationRecorder
{
	public IObservable<Unit> ApproveLast()
	{
		return Observable.Return(default(Unit));
	}
}
