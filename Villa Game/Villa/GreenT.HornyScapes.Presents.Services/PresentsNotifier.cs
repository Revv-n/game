using System;
using GreenT.HornyScapes.Presents.Models;
using UniRx;

namespace GreenT.HornyScapes.Presents.Services;

public class PresentsNotifier
{
	private readonly Subject<Present> _onNotify = new Subject<Present>();

	public IObservable<Present> OnNotify => _onNotify;

	public void Notify(Present present)
	{
		_onNotify.OnNext(present);
	}
}
