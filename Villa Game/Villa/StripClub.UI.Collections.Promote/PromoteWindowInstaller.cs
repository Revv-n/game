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
		base.Container.BindInterfacesAndSelfTo<PromoteWindow>().FromInstance(promoteWindow).AsSingle();
		base.Container.BindIFactory<PromoteTab>().FromComponentInNewPrefab(promoteTabPrefab).UnderTransform(tabsContainer)
			.AsSingle()
			.OnInstantiated<PromoteTab>(SetToggleGroup);
		base.Container.Bind<PromoteTab.Manager>().FromNewComponentOn(tabsContainer.gameObject).AsSingle();
		base.Container.BindIFactory<PrefView>().FromComponentInNewPrefab(promotePrefPrefab).UnderTransform(prefsContainer)
			.AsSingle();
		base.Container.Bind<PrefView.Manager>().FromNewComponentOn(prefsContainer.gameObject).AsSingle();
		base.Container.Bind<GirlPromoOpener>().AsSingle();
	}

	private void SetToggleGroup(InjectContext arg1, PromoteTab promoteTab)
	{
		promoteTab.GetComponent<Toggle>().group = promoteTabGroup;
	}
}
