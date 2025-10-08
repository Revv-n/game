using System.Collections.Generic;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialSelectionSystem : TutorialEntitySystem<TutorialEntityComponent, TutorialEntityStepSO>
{
	public List<TutorialEntityStepSO> Steps = new List<TutorialEntityStepSO>();

	private bool subsystemInited;

	private TutorialSelectionSubsystem selectionSubsystem;

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		InitSubSystem();
		foreach (TutorialEntityComponent component in components)
		{
			selectionSubsystem.SubscribeOnActivate(component);
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
			selectionSubsystem.Dispose();
			selectionSubsystem = null;
		}
	}

	protected override void InitSubSystem()
	{
		selectionSubsystem = new TutorialSelectionSubsystem();
		subsystemInited = true;
	}
}
