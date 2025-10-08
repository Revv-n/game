using GreenT.HornyScapes.ToolTips;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class SummonTooltipInstaller : MonoInstaller<LotViewInstaller>
{
	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private Transform container;

	public override void InstallBindings()
	{
		BindTooltip<DropChancesToolTipView, DropChanceToolTipSettings, DropChancesToolTipView.Manager>(prefab, container.transform);
	}

	private void BindTooltip<TView, TSettings, TManager>(GameObject prefab, Transform parent) where TView : IView<TSettings> where TSettings : ToolTipSettings where TManager : IViewManager<TSettings, TView>
	{
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<TView>()).FromComponentInNewPrefab((Object)prefab)).UnderTransform(parent);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<TManager>()).FromNewComponentOn(parent.gameObject).AsSingle();
	}
}
