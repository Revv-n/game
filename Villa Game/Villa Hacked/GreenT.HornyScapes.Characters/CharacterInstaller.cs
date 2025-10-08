using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Card.Bonus;
using GreenT.HornyScapes.Characters.Data;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Data;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using Zenject;

namespace GreenT.HornyScapes.Characters;

public class CharacterInstaller : Installer<CharacterInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CharacterStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<CharacterInfoMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<CharacterBonusFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<CharacterProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<CharacterFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderGeneric<ILoader<IEnumerable<CharacterInfo>>>)(object)((InstallerBase)this).Container.Bind<ILoader<IEnumerable<CharacterInfo>>>()).To<CharacterLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CharacterBundleLoader>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CharacterStoryBundleLoader>()).AsSingle()).WithArguments<BundleType>(BundleType.CharacterStory);
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<CustomConversationDataBundleLoader>()).AsSingle()).WithArguments<BundleType>(BundleType.CustomConversationData);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CharacterManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CharacterSettingsFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CharacterSettingsManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CharacterStoryLoadingController>()).AsSingle();
		((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<CharacterManagerState>()).AsSingle()).OnInstantiated<CharacterManagerState>((Action<InjectContext, CharacterManagerState>)delegate(InjectContext _context, CharacterManagerState _obj)
		{
			_context.Container.Resolve<ISaver>().Add(_obj);
		});
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CharacterLockerAutoDownloadController>()).AsSingle();
	}
}
