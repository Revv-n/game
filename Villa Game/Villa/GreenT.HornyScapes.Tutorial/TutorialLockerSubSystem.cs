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
		onActivateStream.Add(component.OnActivate.Subscribe(delegate
		{
			OnActivate(activeComponent);
		}));
	}

	public void OnActivate(TutorialLockerComponent component)
	{
		Clear();
		activeComponent = component;
		component.Lockers.IsOpen.Where((bool _value) => _value).Subscribe(Complete);
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
