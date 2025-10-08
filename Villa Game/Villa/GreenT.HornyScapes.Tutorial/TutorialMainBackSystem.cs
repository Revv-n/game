using System.Collections.Generic;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;
using Zenject;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialMainBackSystem : TutorialEntitySystem<TutorialComponent, TutorialStepSO>
{
	public List<TutorialStepSO> Steps = new List<TutorialStepSO>();

	private TutorialMainBackSubSystem mainBackSubSystem;

	private IndicatorDisplayService _displayService;

	private SignalBus _signalBus;

	private bool subsystemInited;

	[Inject]
	public void Construct(IndicatorDisplayService displayService, SignalBus signalBus)
	{
		_displayService = displayService;
		_signalBus = signalBus;
	}

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		InitSubSystem();
		foreach (TutorialComponent component in components)
		{
			mainBackSubSystem.SubscribeOnActivate(component);
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
			mainBackSubSystem.Dispose();
			mainBackSubSystem = null;
		}
	}

	protected override void InitSubSystem()
	{
		mainBackSubSystem = new TutorialMainBackSubSystem(_displayService, _signalBus);
		subsystemInited = true;
	}
}
