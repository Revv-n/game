using System;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.Collections.Promote.UI;

public class PromoteNotifier
{
	private readonly Subject<Cost> _onNotify = new Subject<Cost>();

	public IObservable<Cost> OnNotify => (IObservable<Cost>)_onNotify;

	public void Notify(Cost price)
	{
		_onNotify.OnNext(price);
	}
}
