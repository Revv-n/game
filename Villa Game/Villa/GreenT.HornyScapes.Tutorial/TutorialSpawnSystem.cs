using System.Collections.Generic;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialSpawnSystem : TutorialEntitySystem<TutorialEntityComponent, TutorialEntityStepSO>
{
	public List<TutorialEntityStepSO> Steps = new List<TutorialEntityStepSO>();

	private bool subsystemInited;

	private TutorialSpawnCalculationSubSystem spawnSystem;

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		InitSubSystem();
		foreach (TutorialEntityComponent component in components)
		{
			spawnSystem.SubscribeOnActivate(component);
		}
		return true;
	}

	protected override void InitComponents()
	{
		foreach (TutorialEntityStepSO step in Steps)
		{
			TutorialEntityComponent tutorialEntityComponent = new TutorialEntityComponent(step, groupManager);
			if (tutorialEntityComponent.IsInited.Value)
			{
				components.Add(tutorialEntityComponent);
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
		spawnSystem = new TutorialSpawnCalculationSubSystem();
		subsystemInited = true;
	}
}
