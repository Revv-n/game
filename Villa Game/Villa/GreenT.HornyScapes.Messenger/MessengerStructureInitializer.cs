using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using GreenT.Settings.Data;
using StripClub.Messenger.Data;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.Messenger;

public class MessengerStructureInitializer : StructureInitializer<ConfigParser.Folder>
{
	private IStructureInitializer<ConfigParser.Folder, RequestType> playerMessageInitializer { get; }

	private IStructureInitializer<ConfigParser.Folder, RequestType> characterMessageInitializer { get; }

	private IStructureInitializer<ConfigParser.Folder, RequestType> conversationInitializer { get; }

	private IStructureInitializer<ConfigParser.Folder, RequestType> dialogueInitializer { get; }

	public MessengerStructureInitializer(StructureInitializerProxyWithArrayFromConfig<PlayerMessageConfigMapper> playerMessageInitializer, StructureInitializerProxyWithArrayFromConfig<CharacterMessageConfigMapper> characterMessageInitializer, StructureInitializerProxyWithArrayFromConfig<ConversationConfigMapper> conversationInitializer, StructureInitializerProxyWithArrayFromConfig<DialogueConfigMapper> dialogueInitializer)
		: base((IEnumerable<IStructureInitializer>)null)
	{
		this.playerMessageInitializer = playerMessageInitializer;
		this.characterMessageInitializer = characterMessageInitializer;
		this.conversationInitializer = conversationInitializer;
		this.dialogueInitializer = dialogueInitializer;
	}

	public override IObservable<bool> Initialize(ConfigParser.Folder configStructure)
	{
		return (from _isInited in (from _isInited in (from _isInited in (from _isInited in playerMessageInitializer.Initialize(configStructure, RequestType.PlayerMessages)
						where _isInited
						select _isInited).ContinueWith((bool _) => characterMessageInitializer.Initialize(configStructure, RequestType.CharacterMessages))
					where _isInited
					select _isInited).ContinueWith((bool _) => conversationInitializer.Initialize(configStructure, RequestType.ConversationOverview))
				where _isInited
				select _isInited).ContinueWith((bool _) => dialogueInitializer.Initialize(configStructure, RequestType.DialoguesOverview))
			where _isInited
			select _isInited).SubscribeOnMainThread().Share().Debug(GetType().Name + ": Load", LogType.Data);
	}
}
