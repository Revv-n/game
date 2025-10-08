using System;
using UniRx;

namespace GreenT.HornyScapes.Monetization;

public class DefaultMonetizationRecorder : IMonetizationRecorder
{
	public IObservable<Unit> ApproveLast()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return Observable.Return(default(Unit));
	}
}
