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
		base.Container.BindInterfacesAndSelfTo<EventRewardCardManager>().FromNewComponentOn(RewardsCardContainer.gameObject).AsSingle();
		BindFactories();
	}

	private void BindFactories()
	{
		base.Container.BindViewFactory<EventReward, EventGirlRewardCard>(RewardsCardContainer, GirlRewardCard);
		base.Container.BindViewFactory<EventReward, EventItemsRewardCard>(RewardsCardContainer, ItemRewardCard);
		base.Container.BindViewFactory<EventReward, EventSkinRewardCard>(RewardsCardContainer, SkinRewardCard);
		base.Container.BindInterfacesTo<EventRewardCardFactory>().AsSingle();
	}
}
