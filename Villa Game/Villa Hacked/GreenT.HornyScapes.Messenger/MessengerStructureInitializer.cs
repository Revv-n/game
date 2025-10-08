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
		return Observable.Share<bool>(Observable.SubscribeOnMainThread<bool>(Observable.Where<bool>(Observable.ContinueWith<bool, bool>(Observable.Where<bool>(Observable.ContinueWith<bool, bool>(Observable.Where<bool>(Observable.ContinueWith<bool, bool>(Observable.Where<bool>(playerMessageInitializer.Initialize(configStructure, RequestType.PlayerMessages), (Func<bool, bool>)((bool _isInited) => _isInited)), (Func<bool, IObservable<bool>>)((bool _) => characterMessageInitializer.Initialize(configStructure, RequestType.CharacterMessages))), (Func<bool, bool>)((bool _isInited) => _isInited)), (Func<bool, IObservable<bool>>)((bool _) => conversationInitializer.Initialize(configStructure, RequestType.ConversationOverview))), (Func<bool, bool>)((bool _isInited) => _isInited)), (Func<bool, IObservable<bool>>)((bool _) => dialogueInitializer.Initialize(configStructure, RequestType.DialoguesOverview))), (Func<bool, bool>)((bool _isInited) => _isInited)))).Debug(GetType().Name + ": Load", LogType.Data);
	}
}
