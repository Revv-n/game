using GreenT.HornyScapes.Extensions;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class BannerContentUIInstaller : MonoInstaller<BannerContentUIInstaller>
{
	[Header("DropInfoCardView")]
	public DropInfoCardViewManager DropInfoCardViewManager;

	public DropInfoCardView DropInfoCardView;

	[Header("InfoSectionView")]
	public InfoSectionViewManager InfoSectionViewManager;

	public InfoSectionView InfoSectionViewPrefab;

	public RectTransform InfoSectionViewContainer;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindViewFactory<RewardInfo[], InfoSectionView>(InfoSectionViewContainer, InfoSectionViewPrefab);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<InfoSectionViewManager>().FromInstance((object)InfoSectionViewManager).AsSingle();
		((MonoInstallerBase)this).Container.BindViewFactory<RewardInfo, DropInfoCardView>(DropInfoCardViewManager.HoldContainer, DropInfoCardView);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<DropInfoCardViewManager>().FromInstance((object)DropInfoCardViewManager).AsSingle();
	}
}
