using System.Collections.Generic;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialStaticComponentSystem : TutorialComponentSystem<TutorialComponent, TutorialStepSO>
{
	public List<TutorialStepSO> Steps = new List<TutorialStepSO>();

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		SubscribeInteract();
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

	protected override void SubscribeInteract()
	{
		if (components == null)
		{
			return;
		}
		foreach (TutorialComponent component in components)
		{
			tutorButton.SubscribeOnActivate(component);
		}
	}
}
