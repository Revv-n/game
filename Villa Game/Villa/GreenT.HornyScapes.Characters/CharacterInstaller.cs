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
		base.Container.BindInterfacesAndSelfTo<CharacterStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<CharacterInfoMapper>>().AsSingle();
		base.Container.BindInterfacesTo<CharacterBonusFactory>().AsSingle();
		base.Container.Bind<CharacterProvider>().AsSingle();
		base.Container.BindInterfacesTo<CharacterFactory>().AsCached();
		base.Container.Bind<ILoader<IEnumerable<CharacterInfo>>>().To<CharacterLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CharacterBundleLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CharacterStoryBundleLoader>().AsSingle().WithArguments(BundleType.CharacterStory);
		base.Container.Bind<CustomConversationDataBundleLoader>().AsSingle().WithArguments(BundleType.CustomConversationData);
		base.Container.BindInterfacesAndSelfTo<CharacterManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CharacterSettingsFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CharacterSettingsManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CharacterStoryLoadingController>().AsSingle();
		base.Container.Bind<CharacterManagerState>().AsSingle().OnInstantiated(delegate(InjectContext _context, CharacterManagerState _obj)
		{
			_context.Container.Resolve<ISaver>().Add(_obj);
		});
		base.Container.BindInterfacesAndSelfTo<CharacterLockerAutoDownloadController>().AsSingle();
	}
}
