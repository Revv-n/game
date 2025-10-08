using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.ToolTips;
using Merge;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialInstaller : MonoInstaller<TutorialInstaller>
{
	private const string AutoEnd = "tutorial_autoend";

	public TutorialLightningSystem lightningSystem;

	public GameObject HintPrefab;

	public ToolTipTutorialHintOpener HintParent;

	public GameObject ArrowPrefab;

	public ToolTipTutorialArrowOpener ArrowParent;

	public TutorialConfigSO Config;

	public override void InstallBindings()
	{
		base.Container.Bind<TutorialConfigSO>().FromScriptableObject(Config).AsSingle();
		base.Container.Bind<TutorialGroupManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<TutorialSystem>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<TutorialFinisherResolver>().AsSingle().WithArguments("tutorial_autoend");
		base.Container.BindInterfacesAndSelfTo<TutorialInitializer>().AsSingle();
		BindFactories();
		BindHint();
		BindArrow();
		BindOpeners();
		BindLight();
	}

	private void BindLight()
	{
		base.Container.BindInstance(lightningSystem);
	}

	private void BindOpeners()
	{
		base.Container.Bind<ToolTipTutorialHintOpener>().FromInstance(HintParent).AsSingle();
		base.Container.Bind<ToolTipTutorialArrowOpener>().FromInstance(ArrowParent).AsSingle();
		base.Container.Bind<ToolTipTutorialOpener>().AsSingle();
	}

	private void BindFactories()
	{
		base.Container.BindInterfacesTo<TutorialGroupFactory>().AsCached();
		base.Container.BindInterfacesTo<TutorialStepFactory>().AsCached();
	}

	private void BindHint()
	{
		base.Container.BindIFactory<ToolTipTutorialView>().FromComponentInNewPrefab(HintPrefab).UnderTransform(HintParent.transform)
			.OnInstantiated(delegate(InjectContext context, ToolTipTutorialView view)
			{
				view.SetActive(active: false);
			});
		base.Container.BindInterfacesAndSelfTo<ToolTipTutorialViewManager>().FromNewComponentOn(HintParent.gameObject).AsSingle();
	}

	private void BindArrow()
	{
		base.Container.BindIFactory<ToolTipArrowTutorialView>().FromComponentInNewPrefab(ArrowPrefab).UnderTransform(ArrowParent.transform)
			.OnInstantiated(delegate(InjectContext context, ToolTipArrowTutorialView view)
			{
				view.SetActive(active: false);
			});
		base.Container.BindInterfacesAndSelfTo<ToolTipArrowManager>().FromNewComponentOn(ArrowParent.gameObject).AsSingle();
	}
}
