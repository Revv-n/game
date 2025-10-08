using System;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialMainBackSubSystem : IDisposable
{
	private TutorialComponent activeComponent;

	private IDisposable waitMainStream;

	private readonly SignalBus _signalBus;

	private readonly IndicatorDisplayService _displayService;

	private readonly CompositeDisposable _onActivateStream = new CompositeDisposable();

	public TutorialMainBackSubSystem(IndicatorDisplayService displayService, SignalBus signalBus)
	{
		_displayService = displayService;
		_signalBus = signalBus;
	}

	public void SubscribeOnActivate(TutorialComponent component)
	{
		TutorialComponent activeComponent = component;
		_onActivateStream.Add(component.OnActivate.Subscribe(delegate
		{
			OnActivate(activeComponent);
		}));
	}

	private void OnActivate(TutorialComponent component)
	{
		Clear();
		_signalBus.TryFire(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.Tutorial));
		activeComponent = component;
		waitMainStream = (from _value in _displayService.OnIndicatorPush(FilteredIndicatorType.Tutorial)
			where _value
			select _value).Subscribe(Complete);
	}

	private void Complete(bool value)
	{
		_signalBus.TryFire(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.Tutorial));
		activeComponent.CompleteStep();
		waitMainStream.Dispose();
	}

	private void Clear()
	{
		waitMainStream?.Dispose();
		activeComponent = null;
	}

	public void Dispose()
	{
		_onActivateStream.Dispose();
	}
}
