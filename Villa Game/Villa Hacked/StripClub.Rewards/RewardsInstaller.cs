using GreenT.HornyScapes.Characters.Skins.UI.Lootbox;
using GreenT.HornyScapes.UI;
using StripClub.UI.Collections.Promote;
using UnityEngine;
using Zenject;

namespace StripClub.Rewards;

public class RewardsInstaller : MonoInstaller
{
	[SerializeField]
	private CardViewFactory cardViewFactory;

	[SerializeField]
	private ResourceViewFactory resourceViewFactory;

	[SerializeField]
	private MergeViewFactory mergeViewFactory;

	[SerializeField]
	private SkinViewFactory skinViewFactory;

	[SerializeField]
	private ImageFactory imagePoolFactory;

	[SerializeField]
	private DecorationViewFactory decorationViewFactory;

	[SerializeField]
	private CardBattlePassLevelViewFactory battlePassLevelViewFactory;

	[SerializeField]
	private CardBoosterViewFactory _boosterViewFactory;

	[SerializeField]
	private LootboxPrefView prefPrefab;

	[SerializeField]
	private Transform prefsContainer;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesTo<CardViewFactory>().FromInstance((object)cardViewFactory);
		((MonoInstallerBase)this).Container.BindInterfacesTo<CardBattlePassLevelViewFactory>().FromInstance((object)battlePassLevelViewFactory);
		((MonoInstallerBase)this).Container.BindInterfacesTo<ResourceViewFactory>().FromInstance((object)resourceViewFactory);
		((MonoInstallerBase)this).Container.BindInterfacesTo<MergeViewFactory>().FromInstance((object)mergeViewFactory);
		((MonoInstallerBase)this).Container.BindInterfacesTo<SkinViewFactory>().FromInstance((object)skinViewFactory);
		((MonoInstallerBase)this).Container.BindInterfacesTo<DecorationViewFactory>().FromInstance((object)decorationViewFactory);
		((MonoInstallerBase)this).Container.BindInterfacesTo<CardBoosterViewFactory>().FromInstance((object)_boosterViewFactory);
		((MonoInstallerBase)this).Container.BindInterfacesTo<ImageFactory>().FromInstance((object)imagePoolFactory);
		BindPrefView();
	}

	private void BindPrefView()
	{
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<LootboxPrefView>()).FromComponentInNewPrefab((Object)prefPrefab)).UnderTransform(prefsContainer).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<LootboxPrefView.LootboxPrefManager>()).FromNewComponentOn(prefsContainer.gameObject).AsSingle();
	}
}
