using System;
using GreenT.HornyScapes.Characters;
using ModestTree;
using StripClub.Messenger;
using StripClub.Messenger.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Messenger;

public class ConversationFactory : IFactory<ConversationConfigMapper, Conversation>, IFactory
{
	private readonly CharacterManager _characterManager;

	private const string LockKey = "content.chat.custom_conversation.{0}";

	public ConversationFactory(CharacterManager charactersBase, CharacterManager characterManager)
	{
		Assert.IsNotNull((object)charactersBase);
		_characterManager = characterManager;
	}

	public Conversation Create(ConversationConfigMapper mapper)
	{
		return CreateNew(mapper, null);
	}

	public Conversation Create(ConversationConfigMapper mapper, CustomConversationData customData)
	{
		return CreateNew(mapper, customData.Icon);
	}

	private Conversation CreateNew(ConversationConfigMapper mapper, Sprite icon)
	{
		string nameKey = ValidateNameKey(mapper);
		return new Conversation(_characterManager, mapper.ID, mapper.DialoguesCount, mapper.TotalMedia, mapper.ParticipantIDCollection, mapper.NamesVisibility, nameKey, icon);
	}

	private string ValidateNameKey(ConversationConfigMapper mapper)
	{
		if (!string.IsNullOrEmpty(mapper.CustomBundleName))
		{
			return $"content.chat.custom_conversation.{mapper.CustomBundleName}";
		}
		try
		{
			int characterID = mapper.ParticipantIDCollection[0];
			return _characterManager.Get(characterID).NameKey;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Error on parsing name key for conversation with ID:" + mapper.ID);
		}
	}
}
