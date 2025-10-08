using System;
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
		((FromBinder)((MonoInstallerBase)this).Container.Bind<TutorialConfigSO>()).FromScriptableObject((ScriptableObject)Config).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<TutorialGroupManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TutorialSystem>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TutorialFinisherResolver>()).AsSingle()).WithArguments<string>("tutorial_autoend");
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TutorialInitializer>()).AsSingle();
		BindFactories();
		BindHint();
		BindArrow();
		BindOpeners();
		BindLight();
	}

	private void BindLight()
	{
		((MonoInstallerBase)this).Container.BindInstance<TutorialLightningSystem>(lightningSystem);
	}

	private void BindOpeners()
	{
		((FromBinderGeneric<ToolTipTutorialHintOpener>)(object)((MonoInstallerBase)this).Container.Bind<ToolTipTutorialHintOpener>()).FromInstance(HintParent).AsSingle();
		((FromBinderGeneric<ToolTipTutorialArrowOpener>)(object)((MonoInstallerBase)this).Container.Bind<ToolTipTutorialArrowOpener>()).FromInstance(ArrowParent).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<ToolTipTutorialOpener>()).AsSingle();
	}

	private void BindFactories()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<TutorialGroupFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<TutorialStepFactory>()).AsCached();
	}

	private void BindHint()
	{
		((InstantiateCallbackConditionCopyNonLazyBinder)((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<ToolTipTutorialView>()).FromComponentInNewPrefab((UnityEngine.Object)HintPrefab)).UnderTransform(HintParent.transform)).OnInstantiated<ToolTipTutorialView>((Action<InjectContext, ToolTipTutorialView>)delegate(InjectContext context, ToolTipTutorialView view)
		{
			view.SetActive(active: false);
		});
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ToolTipTutorialViewManager>()).FromNewComponentOn(HintParent.gameObject).AsSingle();
	}

	private void BindArrow()
	{
		((InstantiateCallbackConditionCopyNonLazyBinder)((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<ToolTipArrowTutorialView>()).FromComponentInNewPrefab((UnityEngine.Object)ArrowPrefab)).UnderTransform(ArrowParent.transform)).OnInstantiated<ToolTipArrowTutorialView>((Action<InjectContext, ToolTipArrowTutorialView>)delegate(InjectContext context, ToolTipArrowTutorialView view)
		{
			view.SetActive(active: false);
		});
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ToolTipArrowManager>()).FromNewComponentOn(ArrowParent.gameObject).AsSingle();
	}
}
