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
		base.Container.BindPostRequest<UpdateUserRequest>(PostRequestType.UserUpdate);
		base.Container.Bind<MessengerTracker>().FromInstance(messengerTracker).AsSingle();
		base.Container.Bind<ConversationTabProvider>().FromInstance(tabContainer).AsSingle();
		base.Container.Bind<ConversationTab>().FromInstance(tabPrefab).AsSingle();
		base.Container.Bind<TabManager>().FromInstance(tabManager).AsSingle();
		base.Container.BindInterfacesAndSelfTo<ConversationTabFactory>().AsSingle();
		base.Container.Bind<MonoViewFactory>().WithId("Messages").FromInstance(messageViewFactory);
		base.Container.Bind<MonoViewFactory>().WithId("Player options").FromInstance(playerOptionsViewFactory)
			.AsSingle();
		base.Container.Bind<MessageManager>().FromInstance(messageManager).AsSingle();
		base.Container.BindInterfacesTo<PromoteOpener>().AsSingle();
		base.Container.Bind<RelationshipOpener>().AsSingle();
	}
}
