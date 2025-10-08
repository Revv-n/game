using System;
using StripClub.UI.Rewards;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Collections.Promote;

public class PromoteWindowInstaller : MonoInstaller<PromoteWindowInstaller>
{
	[SerializeField]
	private PromoteWindow promoteWindow;

	[SerializeField]
	private PromoteTab promoteTabPrefab;

	[SerializeField]
	private Transform tabsContainer;

	[SerializeField]
	private ToggleGroup promoteTabGroup;

	[SerializeField]
	private PrefView promotePrefPrefab;

	[SerializeField]
	private Transform prefsContainer;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PromoteWindow>().FromInstance((object)promoteWindow).AsSingle();
		((InstantiateCallbackConditionCopyNonLazyBinder)((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<PromoteTab>()).FromComponentInNewPrefab((UnityEngine.Object)promoteTabPrefab)).UnderTransform(tabsContainer).AsSingle()).OnInstantiated<PromoteTab>((Action<InjectContext, PromoteTab>)SetToggleGroup);
		((FromBinder)((MonoInstallerBase)this).Container.Bind<PromoteTab.Manager>()).FromNewComponentOn(tabsContainer.gameObject).AsSingle();
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<PrefView>()).FromComponentInNewPrefab((UnityEngine.Object)promotePrefPrefab)).UnderTransform(prefsContainer).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<PrefView.Manager>()).FromNewComponentOn(prefsContainer.gameObject).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<GirlPromoOpener>()).AsSingle();
	}

	private void SetToggleGroup(InjectContext arg1, PromoteTab promoteTab)
	{
		promoteTab.GetComponent<Toggle>().group = promoteTabGroup;
	}
}
