using System;
using GreenT.HornyScapes.MergeCore;
using Merge;
using UniRx;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialSelectionSubsystem : IDisposable
{
	private CompositeDisposable onActivateStream = new CompositeDisposable();

	private TutorialEntityComponent activeComponent;

	public void SubscribeOnActivate(TutorialEntityComponent component)
	{
		TutorialEntityComponent activeComponent = component;
		onActivateStream.Add(ObservableExtensions.Subscribe<BaseTutorialComponent<TutorialEntityStepSO>>(component.OnActivate, (Action<BaseTutorialComponent<TutorialEntityStepSO>>)delegate
		{
			OnActivate(activeComponent, activeComponent.UniqID);
		}));
	}

	public void OnActivate(TutorialEntityComponent component, int count)
	{
		if (activeComponent != null)
		{
			Clear();
		}
		Controller<MotionController>.Instance.IsBlockedByTutor = true;
		activeComponent = component;
		Controller<SelectionController>.Instance.OnSelectionChange += Complete;
	}

	private void Complete(GameItem obj)
	{
		if (activeComponent.UniqID == obj.Config.UniqId)
		{
			Controller<MotionController>.Instance.IsBlockedByTutor = false;
			activeComponent.CompleteStep();
			Clear();
		}
	}

	private void Clear()
	{
		activeComponent = null;
		Controller<SelectionController>.Instance.OnSelectionChange -= Complete;
	}

	public void Dispose()
	{
		onActivateStream.Dispose();
	}
}
