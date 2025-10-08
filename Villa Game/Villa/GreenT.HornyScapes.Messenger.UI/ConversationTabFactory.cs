using GreenT.Localizations;
using StripClub.Messenger;
using StripClub.Messenger.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public class ConversationTabFactory : IFactory<ConversationTab>, IFactory
{
	private readonly ConversationTabProvider _conversationTabProvider;

	private readonly ConversationTab _prefab;

	private readonly IMessengerManager _messenger;

	private readonly LocalizationService _localizationService;

	public ConversationTabFactory(ConversationTabProvider conversationTabProvider, ConversationTab prefab, IMessengerManager messenger, LocalizationService localizationService)
	{
		_conversationTabProvider = conversationTabProvider;
		_prefab = prefab;
		_messenger = messenger;
		_localizationService = localizationService;
	}

	public ConversationTab Create()
	{
		ConversationTab conversationTab = Object.Instantiate(_prefab, _conversationTabProvider.Root);
		conversationTab.Init(_messenger, _localizationService);
		_conversationTabProvider.AddTab(conversationTab);
		return conversationTab;
	}
}
