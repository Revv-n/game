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
		base.Container.BindViewFactory<RewardInfo[], InfoSectionView>(InfoSectionViewContainer, InfoSectionViewPrefab);
		base.Container.BindInterfacesAndSelfTo<InfoSectionViewManager>().FromInstance(InfoSectionViewManager).AsSingle();
		base.Container.BindViewFactory<RewardInfo, DropInfoCardView>(DropInfoCardViewManager.HoldContainer, DropInfoCardView);
		base.Container.BindInterfacesAndSelfTo<DropInfoCardViewManager>().FromInstance(DropInfoCardViewManager).AsSingle();
	}
}
