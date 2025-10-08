using GreenT.HornyScapes.Relationships.Animations;
using GreenT.HornyScapes.Relationships.Views;
using StripClub.Rewards;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Windows;

public class RelationshipUiInstaller : MonoInstaller<RelationshipUiInstaller>
{
	[SerializeField]
	private ActiveRewardTracker _activeRewardTracker;

	[SerializeField]
	private LevelUpAnimationService _levelUpAnimationService;

	[SerializeField]
	private LevelUpCommandHandler _levelUpCommandHandler;

	[SerializeField]
	private Transform _levelContainerParent;

	[SerializeField]
	private RelationshipLevelContainer _levelPrefab;

	[SerializeField]
	private ImageFactory _imagePoolFactory;

	public override void InstallBindings()
	{
		base.Container.Bind<LevelUpAnimationService>().FromInstance(_levelUpAnimationService).AsSingle();
		base.Container.Bind<LevelUpCommandHandler>().FromInstance(_levelUpCommandHandler).AsSingle();
		base.Container.BindIFactory<RelationshipLevelContainer>().FromComponentInNewPrefab(_levelPrefab).UnderTransform(_levelContainerParent);
		base.Container.BindInterfacesAndSelfTo<RelationshipLevelContainer.ViewManager>().FromNewComponentOn(_levelContainerParent.gameObject).AsSingle();
		base.Container.Bind<ActiveRewardTracker>().FromInstance(_activeRewardTracker).AsSingle();
		base.Container.BindInterfacesTo<ImageFactory>().FromInstance(_imagePoolFactory).AsSingle();
	}
}
