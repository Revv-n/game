using System.Collections.Generic;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialCollectSystem : TutorialEntitySystem<TutorialEntityComponent, TutorialEntityStepSO>
{
	public List<TutorialEntityStepSO> Steps = new List<TutorialEntityStepSO>();

	private bool subsystemInited;

	private TutorialCollectFinderSubSystem finderSystem;

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		InitSubSystem();
		foreach (TutorialEntityComponent component in components)
		{
			finderSystem.SubscribeOnActivate(component);
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
			finderSystem.Dispose();
			finderSystem = null;
		}
	}

	protected override void InitSubSystem()
	{
		finderSystem = new TutorialCollectFinderSubSystem();
		subsystemInited = true;
	}
}
