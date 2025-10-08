using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialStaticToggleSystem : TutorialComponentSystem<TutorialComponent, TutorialStepSO>
{
	[SerializeField]
	private Toggle toggle;

	protected TutorialToggle<TutorialComponent, TutorialStepSO> tutorToggle;

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

	protected override void InitSubSystem()
	{
		if (!subsystemInited)
		{
			base.InitSubSystem();
			tutorToggle = new TutorialToggle<TutorialComponent, TutorialStepSO>();
			tutorToggle.Init(highlighter, toggle);
		}
	}

	protected override void DestroySubSystem()
	{
		if (subsystemInited)
		{
			base.DestroySubSystem();
			tutorToggle.Dispose();
			tutorToggle = null;
		}
	}

	protected override void SubscribeInteract()
	{
		foreach (TutorialComponent component in components)
		{
			tutorToggle.SubscribeOnActivate(component);
		}
	}
}
