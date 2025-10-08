using System;
using GreenT.HornyScapes.MergeCore;
using Merge;
using UniRx;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialMergeFinderSubSystem : IDisposable
{
	private CompositeDisposable onActivateStream = new CompositeDisposable();

	private int uniqID;

	private TutorialEntityComponent activeComponent;

	public void SubscribeOnActivate(TutorialEntityComponent component)
	{
		TutorialEntityComponent activeComponent = component;
		onActivateStream.Add(component.OnActivate.Subscribe(delegate
		{
			OnActivate(activeComponent, activeComponent.UniqID);
		}));
	}

	public void OnActivate(TutorialEntityComponent component, int uniqID)
	{
		Clear();
		this.uniqID = uniqID;
		activeComponent = component;
		Controller<GameItemController>.Instance.AfterItemCreated += Complete;
	}

	private void Complete(GameItem item)
	{
		if (item.Config.UniqId == uniqID)
		{
			activeComponent.CompleteStep();
		}
	}

	private void Clear()
	{
		activeComponent = null;
		uniqID = -1;
		Controller<GameItemController>.Instance.AfterItemCreated -= Complete;
	}

	public void Dispose()
	{
		onActivateStream.Dispose();
	}
}
