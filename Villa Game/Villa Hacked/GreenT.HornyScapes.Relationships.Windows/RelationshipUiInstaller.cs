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
		((FromBinderGeneric<LevelUpAnimationService>)(object)((MonoInstallerBase)this).Container.Bind<LevelUpAnimationService>()).FromInstance(_levelUpAnimationService).AsSingle();
		((FromBinderGeneric<LevelUpCommandHandler>)(object)((MonoInstallerBase)this).Container.Bind<LevelUpCommandHandler>()).FromInstance(_levelUpCommandHandler).AsSingle();
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<RelationshipLevelContainer>()).FromComponentInNewPrefab((Object)_levelPrefab)).UnderTransform(_levelContainerParent);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RelationshipLevelContainer.ViewManager>()).FromNewComponentOn(_levelContainerParent.gameObject).AsSingle();
		((FromBinderGeneric<ActiveRewardTracker>)(object)((MonoInstallerBase)this).Container.Bind<ActiveRewardTracker>()).FromInstance(_activeRewardTracker).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesTo<ImageFactory>().FromInstance((object)_imagePoolFactory).AsSingle();
	}
}
