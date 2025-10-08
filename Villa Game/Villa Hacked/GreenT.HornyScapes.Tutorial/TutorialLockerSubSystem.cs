using System;
using UniRx;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialLockerSubSystem : IDisposable
{
	private CompositeDisposable onActivateStream = new CompositeDisposable();

	private TutorialLockerComponent activeComponent;

	public void SubscribeOnActivate(TutorialLockerComponent component)
	{
		TutorialLockerComponent activeComponent = component;
		onActivateStream.Add(ObservableExtensions.Subscribe<BaseTutorialComponent<TutorialLockerStepSO>>(component.OnActivate, (Action<BaseTutorialComponent<TutorialLockerStepSO>>)delegate
		{
			OnActivate(activeComponent);
		}));
	}

	public void OnActivate(TutorialLockerComponent component)
	{
		Clear();
		activeComponent = component;
		ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)component.Lockers.IsOpen, (Func<bool, bool>)((bool _value) => _value)), (Action<bool>)Complete);
	}

	private void Complete(bool value)
	{
		activeComponent.CompleteStep();
	}

	private void Clear()
	{
		activeComponent = null;
	}

	public void Dispose()
	{
		onActivateStream.Dispose();
	}
}
