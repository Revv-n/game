using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipInstaller : MonoInstaller<ToolTipInstaller>
{
	[SerializeField]
	private GameObject hintPrefab;

	[SerializeField]
	private GameObject hintParent;

	[SerializeField]
	private GameObject houseFlyTextPrefab;

	[SerializeField]
	private GameObject houseFlyTextParent;

	[SerializeField]
	private GameObject girlBubblePrefab;

	[SerializeField]
	private GameObject girlBubbleParent;

	[SerializeField]
	private GameObject hintImagePrefab;

	[SerializeField]
	private GameObject hintImageParent;

	[SerializeField]
	private GameObject flyTextPrefab;

	[SerializeField]
	private GameObject flyTextParent;

	[SerializeField]
	private GameObject dropViewPrefab;

	[SerializeField]
	private GameObject dropViewParent;

	[SerializeField]
	private GameObject bankInfoViewPrefab;

	[SerializeField]
	private GameObject bankInfoViewParent;

	[SerializeField]
	private GameObject energyInfoViewPrefab;

	[SerializeField]
	private GameObject energyInfoViewParent;

	public override void InstallBindings()
	{
		BindTooltip<UIToolTipView, ToolTipUISettings, UIToolTipManager>(hintPrefab, hintParent.transform);
		BindTooltip<ToolTipMainView, ToolTipSettings, ToolTipMainView.Manager>(houseFlyTextPrefab, houseFlyTextParent.transform);
		BindTooltip<ToolTipGirlView, ToolTipSettings, ToolTipGirlView.Manager>(girlBubblePrefab, girlBubbleParent.transform);
		BindTooltip<ToolTipImageView, ToolTipImageSettings, ToolTipImageView.Manager>(hintImagePrefab, hintImageParent.transform);
		BindTooltip<HighlightedTextView, ToolTipSettings, HighlightedTextView.Manager>(flyTextPrefab, flyTextParent.transform);
		BindTooltip<DropViewToolTip, TailedToolTipSettings, DropViewToolTip.Manager>(dropViewPrefab, dropViewParent.transform);
		BindTooltip<BankInfoToolTip, TailedToolTipSettings, BankInfoToolTip.Manager>(bankInfoViewPrefab, bankInfoViewParent.transform);
		BindTooltip<EnergyToolTip, TailedToolTipSettings, EnergyToolTip.Manager>(energyInfoViewPrefab, energyInfoViewParent.transform);
	}

	private void BindTooltip<TView, TSettings, TManager>(GameObject prefab, Transform parent) where TView : IView<TSettings> where TSettings : ToolTipSettings where TManager : IViewManager<TSettings, TView>
	{
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<TView>()).FromComponentInNewPrefab((Object)prefab)).UnderTransform(parent);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<TManager>()).FromNewComponentOn(parent.gameObject).AsSingle();
	}
}
