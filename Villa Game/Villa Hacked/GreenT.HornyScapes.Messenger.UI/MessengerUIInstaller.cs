using GreenT.HornyScapes.Extensions;
using GreenT.Settings.Data;
using StripClub.Messenger.UI;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public class MessengerUIInstaller : MonoInstaller
{
	[SerializeField]
	private MessengerTracker messengerTracker;

	[SerializeField]
	private TabManager tabManager;

	[SerializeField]
	private ConversationTab tabPrefab;

	[SerializeField]
	private ConversationTabProvider tabContainer;

	[SerializeField]
	private MessageManager messageManager;

	[SerializeField]
	private MonoViewFactory messageViewFactory;

	[SerializeField]
	private MonoViewFactory playerOptionsViewFactory;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindPostRequest<UpdateUserRequest>(PostRequestType.UserUpdate);
		((FromBinderGeneric<MessengerTracker>)(object)((MonoInstallerBase)this).Container.Bind<MessengerTracker>()).FromInstance(messengerTracker).AsSingle();
		((FromBinderGeneric<ConversationTabProvider>)(object)((MonoInstallerBase)this).Container.Bind<ConversationTabProvider>()).FromInstance(tabContainer).AsSingle();
		((FromBinderGeneric<ConversationTab>)(object)((MonoInstallerBase)this).Container.Bind<ConversationTab>()).FromInstance(tabPrefab).AsSingle();
		((FromBinderGeneric<TabManager>)(object)((MonoInstallerBase)this).Container.Bind<TabManager>()).FromInstance(tabManager).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ConversationTabFactory>()).AsSingle();
		((FromBinderGeneric<MonoViewFactory>)(object)((MonoInstallerBase)this).Container.Bind<MonoViewFactory>().WithId((object)"Messages")).FromInstance(messageViewFactory);
		((FromBinderGeneric<MonoViewFactory>)(object)((MonoInstallerBase)this).Container.Bind<MonoViewFactory>().WithId((object)"Player options")).FromInstance(playerOptionsViewFactory).AsSingle();
		((FromBinderGeneric<MessageManager>)(object)((MonoInstallerBase)this).Container.Bind<MessageManager>()).FromInstance(messageManager).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<PromoteOpener>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<RelationshipOpener>()).AsSingle();
	}
}
