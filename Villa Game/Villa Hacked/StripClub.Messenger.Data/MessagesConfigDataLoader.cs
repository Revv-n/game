using System;
using System.Collections.Generic;
using System.Linq;
using GreenT;
using StripClub.Model.Data;
using UniRx;

namespace StripClub.Messenger.Data;

public class MessagesConfigDataLoader : ILoader<IEnumerable<int>, IEnumerable<MessageConfigMapper>>
{
	private readonly ILoader<IEnumerable<PlayerMessageConfigMapper>> playerMessagesLoader;

	private readonly ILoader<IEnumerable<CharacterMessageConfigMapper>> characterMessagesLoader;

	private Dictionary<int, IEnumerable<MessageConfigMapper>> cache = new Dictionary<int, IEnumerable<MessageConfigMapper>>();

	public MessagesConfigDataLoader(ILoader<IEnumerable<PlayerMessageConfigMapper>> playerMessagesLoader, ILoader<IEnumerable<CharacterMessageConfigMapper>> characterMessagesLoader)
	{
		this.playerMessagesLoader = playerMessagesLoader;
		this.characterMessagesLoader = characterMessagesLoader;
	}

	public IObservable<IEnumerable<MessageConfigMapper>> Load(IEnumerable<int> dialogueIDCollection)
	{
		IEnumerable<int> source = dialogueIDCollection.Except(cache.Keys);
		IEnumerable<MessageConfigMapper> cachedMappers = cache.Where((KeyValuePair<int, IEnumerable<MessageConfigMapper>> _pair) => dialogueIDCollection.Any((int _id) => _id == _pair.Key)).SelectMany((KeyValuePair<int, IEnumerable<MessageConfigMapper>> _pair) => _pair.Value);
		IObservable<IEnumerable<MessageConfigMapper>> observable = null;
		if (!source.Any())
		{
			return Observable.Return<IEnumerable<MessageConfigMapper>>(cachedMappers);
		}
		return Observable.Select<IEnumerable<MessageConfigMapper>, MessageConfigMapper[]>(Observable.Do<IEnumerable<MessageConfigMapper>>(Observable.Zip<IEnumerable<CharacterMessageConfigMapper>, IEnumerable<PlayerMessageConfigMapper>, IEnumerable<MessageConfigMapper>>(characterMessagesLoader.Load().Debug(GetType().Name + ": Character messages mappers loading", LogType.Data | LogType.Messenger), playerMessagesLoader.Load().Debug(GetType().Name + ": Player messages mappers loading", LogType.Data | LogType.Messenger), (Func<IEnumerable<CharacterMessageConfigMapper>, IEnumerable<PlayerMessageConfigMapper>, IEnumerable<MessageConfigMapper>>)((IEnumerable<CharacterMessageConfigMapper> character, IEnumerable<PlayerMessageConfigMapper> player) => character.Cast<MessageConfigMapper>().Union(player))), (Action<IEnumerable<MessageConfigMapper>>)SaveInCache), (Func<IEnumerable<MessageConfigMapper>, MessageConfigMapper[]>)((IEnumerable<MessageConfigMapper> _) => cachedMappers.ToArray())).Debug(GetType().Name + ": Messenger messages mappers loading", LogType.Data | LogType.Messenger);
	}

	private void SaveInCache(IEnumerable<MessageConfigMapper> mapperCollection)
	{
		foreach (IGrouping<int, MessageConfigMapper> item in from _mapper in mapperCollection
			where !cache.ContainsKey(_mapper.DialogueID)
			group _mapper by _mapper.DialogueID)
		{
			cache[item.Key] = item.AsEnumerable();
		}
	}
}
