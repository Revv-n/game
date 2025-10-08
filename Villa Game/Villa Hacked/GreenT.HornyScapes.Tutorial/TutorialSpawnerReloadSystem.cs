using System.Collections.Generic;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialSpawnerReloadSystem : TutorialEntitySystem<TutorialComponent, TutorialStepSO>
{
	public List<TutorialStepSO> Steps = new List<TutorialStepSO>();

	private bool subsystemInited;

	private TutorialSpawnerReloadSubSystem spawnSystem;

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		InitSubSystem();
		foreach (TutorialComponent component in components)
		{
			spawnSystem.SubscribeOnActivate(component);
		}
		return true;
	}

	protected override void InitComponents()
	{
		foreach (TutorialStepSO step in Steps)
		{
			TutorialComponent tutorialComponent = new TutorialComponent(step, groupManager);
			if (tutorialComponent.IsInited.Value)
			{
				components.Add(tutorialComponent);
			}
		}
	}

	protected override void DestroySubSystem()
	{
		if (subsystemInited)
		{
			spawnSystem.Dispose();
			spawnSystem = null;
		}
	}

	protected override void InitSubSystem()
	{
		spawnSystem = new TutorialSpawnerReloadSubSystem();
		subsystemInited = true;
	}
}
