using System;
using System.Collections.Generic;
using StripClub.Messenger;
using StripClub.Messenger.Data;
using Zenject;

namespace GreenT.HornyScapes.Messenger.Data;

public class MessageFactory : IFactory<MessageConfigMapper, ChatMessage>, IFactory
{
	private readonly IClock _clock;

	private readonly MessageOptionsFactory _messageOptionsFactory;

	public MessageFactory(IClock clock, MessageOptionsFactory messageOptionsFactory)
	{
		_clock = clock;
		_messageOptionsFactory = messageOptionsFactory;
	}

	public ChatMessage Create(MessageConfigMapper mapper)
	{
		try
		{
			if (!(mapper is PlayerMessageConfigMapper mapper2))
			{
				if (mapper is CharacterMessageConfigMapper mapper3)
				{
					return CreatCharacterMessage(mapper3);
				}
				throw new ArgumentOutOfRangeException($"Can't create message for this type of mapper: {mapper.GetType()}");
			}
			return CreatPlayerMessage(mapper2);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"Error when creating message for dialogue (id: {mapper.DialogueID}) with number: {mapper.Number}");
		}
	}

	private ChatMessage CreatPlayerMessage(PlayerMessageConfigMapper mapper)
	{
		List<ResponseOption> options = _messageOptionsFactory.CreatePlayerOptions(mapper.ItemMappers, mapper.DialogueID, mapper.Number);
		return new PlayerChatMessage(mapper.DialogueID, mapper.Number, _clock.GetTime(), options);
	}

	private ChatMessage CreatCharacterMessage(CharacterMessageConfigMapper mapper)
	{
		return new CharacterChatMessage(mapper.CharacterID, mapper.DialogueID, mapper.Number, _clock.GetTime(), mapper.MediaID);
	}
}
