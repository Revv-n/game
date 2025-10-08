using System;
using GreenT.HornyScapes.MergeCore;
using Merge;
using UniRx;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialSpawnCalculationSubSystem : IDisposable
{
	private CompositeDisposable onActivateStream = new CompositeDisposable();

	private int needCount;

	private int createdItemsCount;

	private TutorialEntityComponent activeComponent;

	public void SubscribeOnActivate(TutorialEntityComponent component)
	{
		TutorialEntityComponent activeComponent = component;
		onActivateStream.Add(component.OnActivate.Subscribe(delegate
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
		needCount = count;
		createdItemsCount = 0;
		activeComponent = component;
		Controller<ClickSpawnController>.Instance.OnClickSpawn += Complete;
	}

	private void Complete(GameItem source, GameItem created)
	{
		createdItemsCount++;
		if (createdItemsCount == needCount)
		{
			Controller<MotionController>.Instance.IsBlockedByTutor = false;
			activeComponent.CompleteStep();
			Clear();
		}
	}

	private void Clear()
	{
		needCount = -1;
		createdItemsCount = 0;
		activeComponent = null;
		Controller<ClickSpawnController>.Instance.OnClickSpawn -= Complete;
	}

	public void Dispose()
	{
		onActivateStream.Dispose();
	}
}
