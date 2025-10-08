using System;
using GreenT.Data;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Messenger.Data;
using StripClub.Messenger;
using StripClub.Messenger.Data;
using StripClub.Model.Data;
using Zenject;

namespace GreenT.HornyScapes.Messenger;

public class MessengerInstaller : Installer<MessengerInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MessengerStructureInitializer>()).AsSingle();
		BindConversations();
		BindDialogues();
		BindMessages();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MessengerNotifyController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderGeneric<IMessengerDataLoader>)(object)((InstallerBase)this).Container.Bind<IMessengerDataLoader>()).To<MessengerConfigDataLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<MessengerManager>()).AsSingle();
		((FromBinderGeneric<MessengerState>)(object)((InstallerBase)this).Container.Bind<MessengerState>()).FromFactory<StateFactory>().AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<SavableVariable<int>>()).AsSingle()).WithArguments<string, int>("reply_energy_bought_count", 1).OnInstantiated<SavableVariable<int>>((Action<InjectContext, SavableVariable<int>>)delegate(InjectContext _context, SavableVariable<int> _variable)
		{
			((InstallerBase)this).Container.Resolve<ISaver>().Add(_variable);
		});
	}

	private void BindConversations()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<ConversationFactory>()).AsCached();
		Bind<ConfigDataLoader<ConversationConfigMapper>, StructureInitializerProxyWithArrayFromConfig<ConversationConfigMapper>, MapperStructureInitializer<ConversationConfigMapper>, ConversationConfigMapper.Manager>();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<ConversationConfigDataLoader>()).AsSingle();
	}

	private void BindDialogues()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<StructureInitializerProxyWithArrayFromConfig<DialogueConfigMapper>>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<MapperStructureInitializer<DialogueConfigMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<ConfigDataLoader<DialogueConfigMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<DialogueLockerInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DialogueConfigMapper.Manager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DialoguesConfigDataLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<EntityLoader<DialogueConfigMapper, DialogueLocker>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<DialogueFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<DialogueLockerFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DialoguesTracker>()).AsSingle();
	}

	private void BindMessages()
	{
		Bind<ConfigDataLoader<PlayerMessageConfigMapper>, StructureInitializerProxyWithArrayFromConfig<PlayerMessageConfigMapper>, MapperStructureInitializer<PlayerMessageConfigMapper>, PlayerMessageConfigMapper.Manager>();
		Bind<ConfigDataLoader<CharacterMessageConfigMapper>, StructureInitializerProxyWithArrayFromConfig<CharacterMessageConfigMapper>, MapperStructureInitializer<CharacterMessageConfigMapper>, CharacterMessageConfigMapper.Manager>();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MessagesConfigDataLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MessageFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<MessageOptionsFactory>()).AsSingle();
	}

	private void Bind<TConfigLoader, TStructureInitializer, TMapperStructure, TMapperManager>()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<TConfigLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<TStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<TMapperStructure>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<TMapperManager>()).AsSingle();
	}
}
