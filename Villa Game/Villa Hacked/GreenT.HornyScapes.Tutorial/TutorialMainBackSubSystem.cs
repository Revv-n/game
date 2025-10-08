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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_displayService = displayService;
		_signalBus = signalBus;
	}

	public void SubscribeOnActivate(TutorialComponent component)
	{
		TutorialComponent activeComponent = component;
		_onActivateStream.Add(ObservableExtensions.Subscribe<BaseTutorialComponent<TutorialStepSO>>(component.OnActivate, (Action<BaseTutorialComponent<TutorialStepSO>>)delegate
		{
			OnActivate(activeComponent);
		}));
	}

	private void OnActivate(TutorialComponent component)
	{
		Clear();
		_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.Tutorial));
		activeComponent = component;
		waitMainStream = ObservableExtensions.Subscribe<bool>(Observable.Where<bool>(_displayService.OnIndicatorPush(FilteredIndicatorType.Tutorial), (Func<bool, bool>)((bool _value) => _value)), (Action<bool>)Complete);
	}

	private void Complete(bool value)
	{
		_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.Tutorial));
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
