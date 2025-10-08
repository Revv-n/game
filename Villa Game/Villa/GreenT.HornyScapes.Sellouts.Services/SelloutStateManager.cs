using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Sellouts.Models;
using UniRx;

namespace GreenT.HornyScapes.Sellouts.Services;

public sealed class SelloutStateManager : IDisposable
{
	private readonly Subject<Sellout> _activated = new Subject<Sellout>();

	private readonly Subject<Sellout> _deactivated = new Subject<Sellout>();

	private readonly List<Sellout> _activeSellouts = new List<Sellout>();

	public IObservable<Sellout> Activated => _activated;

	public IObservable<Sellout> Deactivated => _deactivated;

	public void ActivateSellout(Sellout sellout)
	{
		_activeSellouts.Add(sellout);
		_activated.OnNext(sellout);
	}

	public void DeactivateSellout(Sellout sellout)
	{
		_activeSellouts.Remove(sellout);
		_deactivated.OnNext(sellout);
	}

	public Sellout GetActiveSellout()
	{
		return _activeSellouts.FirstOrDefault((Sellout sellout) => sellout.BundleData != null);
	}

	public void Dispose()
	{
		_activated.Dispose();
		_deactivated.Dispose();
	}
}
