using System.Collections.Generic;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialLockerSystem : TutorialEntitySystem<TutorialLockerComponent, TutorialLockerStepSO>
{
	public List<TutorialLockerStepSO> Steps = new List<TutorialLockerStepSO>();

	private bool subsystemInited;

	private TutorialLockerSubSystem lockerSubSystem;

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		InitSubSystem();
		foreach (TutorialLockerComponent component in components)
		{
			lockerSubSystem.SubscribeOnActivate(component);
		}
		return true;
	}

	protected override void InitComponents()
	{
		foreach (TutorialLockerStepSO step in Steps)
		{
			TutorialLockerComponent tutorialLockerComponent = new TutorialLockerComponent(step, groupManager);
			if (tutorialLockerComponent.IsInited.Value)
			{
				components.Add(tutorialLockerComponent);
			}
		}
	}

	protected override void DestroySubSystem()
	{
		if (subsystemInited)
		{
			lockerSubSystem.Dispose();
			lockerSubSystem = null;
		}
	}

	protected override void InitSubSystem()
	{
		lockerSubSystem = new TutorialLockerSubSystem();
		subsystemInited = true;
	}
}
