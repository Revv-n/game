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
		base.Container.BindInterfacesTo<CardViewFactory>().FromInstance(cardViewFactory);
		base.Container.BindInterfacesTo<CardBattlePassLevelViewFactory>().FromInstance(battlePassLevelViewFactory);
		base.Container.BindInterfacesTo<ResourceViewFactory>().FromInstance(resourceViewFactory);
		base.Container.BindInterfacesTo<MergeViewFactory>().FromInstance(mergeViewFactory);
		base.Container.BindInterfacesTo<SkinViewFactory>().FromInstance(skinViewFactory);
		base.Container.BindInterfacesTo<DecorationViewFactory>().FromInstance(decorationViewFactory);
		base.Container.BindInterfacesTo<CardBoosterViewFactory>().FromInstance(_boosterViewFactory);
		base.Container.BindInterfacesTo<ImageFactory>().FromInstance(imagePoolFactory);
		BindPrefView();
	}

	private void BindPrefView()
	{
		base.Container.BindIFactory<LootboxPrefView>().FromComponentInNewPrefab(prefPrefab).UnderTransform(prefsContainer)
			.AsSingle();
		base.Container.Bind<LootboxPrefView.LootboxPrefManager>().FromNewComponentOn(prefsContainer.gameObject).AsSingle();
	}
}
