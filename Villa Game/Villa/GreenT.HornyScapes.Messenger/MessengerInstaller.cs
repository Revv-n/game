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
		base.Container.BindInterfacesAndSelfTo<MessengerStructureInitializer>().AsSingle();
		BindConversations();
		BindDialogues();
		BindMessages();
		base.Container.BindInterfacesAndSelfTo<MessengerNotifyController>().AsSingle();
		base.Container.Bind<IMessengerDataLoader>().To<MessengerConfigDataLoader>().AsSingle();
		base.Container.BindInterfacesTo<MessengerManager>().AsSingle();
		base.Container.Bind<MessengerState>().FromFactory<StateFactory>().AsSingle();
		base.Container.Bind<SavableVariable<int>>().AsSingle().WithArguments("reply_energy_bought_count", 1)
			.OnInstantiated(delegate(InjectContext _context, SavableVariable<int> _variable)
			{
				base.Container.Resolve<ISaver>().Add(_variable);
			});
	}

	private void BindConversations()
	{
		base.Container.BindInterfacesAndSelfTo<ConversationFactory>().AsCached();
		Bind<ConfigDataLoader<ConversationConfigMapper>, StructureInitializerProxyWithArrayFromConfig<ConversationConfigMapper>, MapperStructureInitializer<ConversationConfigMapper>, ConversationConfigMapper.Manager>();
		base.Container.BindInterfacesAndSelfTo<ConversationConfigDataLoader>().AsSingle();
	}

	private void BindDialogues()
	{
		base.Container.Bind<StructureInitializerProxyWithArrayFromConfig<DialogueConfigMapper>>().AsCached();
		base.Container.Bind<MapperStructureInitializer<DialogueConfigMapper>>().AsSingle();
		base.Container.BindInterfacesTo<ConfigDataLoader<DialogueConfigMapper>>().AsSingle();
		base.Container.BindInterfacesTo<DialogueLockerInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DialogueConfigMapper.Manager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DialoguesConfigDataLoader>().AsSingle();
		base.Container.BindInterfacesTo<EntityLoader<DialogueConfigMapper, DialogueLocker>>().AsSingle();
		base.Container.BindInterfacesTo<DialogueFactory>().AsCached();
		base.Container.BindInterfacesTo<DialogueLockerFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<DialoguesTracker>().AsSingle();
	}

	private void BindMessages()
	{
		Bind<ConfigDataLoader<PlayerMessageConfigMapper>, StructureInitializerProxyWithArrayFromConfig<PlayerMessageConfigMapper>, MapperStructureInitializer<PlayerMessageConfigMapper>, PlayerMessageConfigMapper.Manager>();
		Bind<ConfigDataLoader<CharacterMessageConfigMapper>, StructureInitializerProxyWithArrayFromConfig<CharacterMessageConfigMapper>, MapperStructureInitializer<CharacterMessageConfigMapper>, CharacterMessageConfigMapper.Manager>();
		base.Container.BindInterfacesAndSelfTo<MessagesConfigDataLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MessageFactory>().AsCached();
		base.Container.Bind<MessageOptionsFactory>().AsSingle();
	}

	private void Bind<TConfigLoader, TStructureInitializer, TMapperStructure, TMapperManager>()
	{
		base.Container.BindInterfacesTo<TConfigLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<TStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<TMapperStructure>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<TMapperManager>().AsSingle();
	}
}
