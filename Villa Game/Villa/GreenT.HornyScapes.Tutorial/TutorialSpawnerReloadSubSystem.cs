using System;
using GreenT.HornyScapes.MergeCore;
using Merge;
using UniRx;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialSpawnerReloadSubSystem : IDisposable
{
	private CompositeDisposable onActivateStream = new CompositeDisposable();

	private TutorialComponent activeComponent;

	public void SubscribeOnActivate(TutorialComponent component)
	{
		TutorialComponent activeComponent = component;
		onActivateStream.Add(component.OnActivate.Subscribe(delegate
		{
			OnActivate(activeComponent);
		}));
	}

	public void OnActivate(TutorialComponent component)
	{
		if (activeComponent != null)
		{
			Clear();
		}
		activeComponent = component;
		Controller<ClickSpawnController>.Instance.OnClickSpawn += Complete;
	}

	private void Complete(GameItem source, GameItem created)
	{
		if (source.GetBox<GIBox.ClickSpawn>().Data.Amount == 0)
		{
			activeComponent.CompleteStep();
			Clear();
		}
	}

	private void Clear()
	{
		activeComponent = null;
		Controller<ClickSpawnController>.Instance.OnClickSpawn -= Complete;
	}

	public void Dispose()
	{
		onActivateStream.Dispose();
	}
}
