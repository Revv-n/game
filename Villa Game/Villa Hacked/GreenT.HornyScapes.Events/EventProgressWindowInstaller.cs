using GreenT.HornyScapes.Characters.Skins.Events;
using GreenT.HornyScapes.Extensions;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventProgressWindowInstaller : MonoInstaller<EventProgressWindowInstaller>
{
	public EventGirlRewardCard GirlRewardCard;

	public EventItemsRewardCard ItemRewardCard;

	public EventSkinRewardCard SkinRewardCard;

	public Transform RewardsCardContainer;

	public override void InstallBindings()
	{
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventRewardCardManager>()).FromNewComponentOn(RewardsCardContainer.gameObject).AsSingle();
		BindFactories();
	}

	private void BindFactories()
	{
		((MonoInstallerBase)this).Container.BindViewFactory<EventReward, EventGirlRewardCard>(RewardsCardContainer, GirlRewardCard);
		((MonoInstallerBase)this).Container.BindViewFactory<EventReward, EventItemsRewardCard>(RewardsCardContainer, ItemRewardCard);
		((MonoInstallerBase)this).Container.BindViewFactory<EventReward, EventSkinRewardCard>(RewardsCardContainer, SkinRewardCard);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<EventRewardCardFactory>()).AsSingle();
	}
}
