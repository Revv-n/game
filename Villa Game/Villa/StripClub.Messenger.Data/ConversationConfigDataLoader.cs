using System;
using System.Collections.Generic;
using System.Linq;
using GreenT;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Messenger;
using StripClub.Model.Data;
using UniRx;

namespace StripClub.Messenger.Data;

public class ConversationConfigDataLoader : IBundlesLoader<IEnumerable<int>, IEnumerable<Conversation>>, ILoader<IEnumerable<int>, IEnumerable<Conversation>>, IDisposable
{
	private readonly ILoader<IEnumerable<ConversationConfigMapper>> _loader;

	private readonly ConversationFactory _conversationFactory;

	private readonly CustomConversationDataBundleLoader _conversationCustomDataLoader;

	private IDisposable releasingDisposable;

	public ConversationConfigDataLoader(ILoader<IEnumerable<ConversationConfigMapper>> loader, ConversationFactory conversationFactory, CustomConversationDataBundleLoader conversationCustomDataLoader)
	{
		_loader = loader;
		_conversationFactory = conversationFactory;
		_conversationCustomDataLoader = conversationCustomDataLoader;
	}

	public IObservable<IEnumerable<Conversation>> Load(IEnumerable<int> conversationIDs)
	{
		IObservable<ConversationConfigMapper> conversationMapper = GetConversationMapper(conversationIDs);
		return CreatConversation(conversationMapper).Merge(CreatCustomConversation(conversationMapper)).ToArray().Debug(GetType().Name + ": Return conversation array", LogType.Data | LogType.Messenger);
	}

	public void ReleaseBundle(IEnumerable<int> param)
	{
		IObservable<ConversationConfigMapper> conversationMapper = GetConversationMapper(param);
		releasingDisposable = conversationMapper.Where((ConversationConfigMapper mapper) => !string.IsNullOrEmpty(mapper.CustomBundleName)).Do(delegate(ConversationConfigMapper mapper)
		{
			_conversationCustomDataLoader.ReleaseBundle(mapper.CustomBundleName);
		}).Subscribe();
	}

	private IObservable<ConversationConfigMapper> GetConversationMapper(IEnumerable<int> conversationIDs)
	{
		return (from mapperSet in _loader.Load()
			select from mapper in mapperSet
				where conversationIDs.Contains(mapper.ID)
				select mapper).SelectMany((IEnumerable<ConversationConfigMapper> x) => x).DelayFrameSubscription(1).Share();
	}

	private IObservable<Conversation> CreatConversation(IObservable<ConversationConfigMapper> observableConversationMapperLoad)
	{
		return observableConversationMapperLoad.Where((ConversationConfigMapper mapper) => string.IsNullOrEmpty(mapper.CustomBundleName)).Do(SendLog).Select(_conversationFactory.Create);
	}

	private IObservable<Conversation> CreatCustomConversation(IObservable<ConversationConfigMapper> observableConversationMapperLoad)
	{
		return observableConversationMapperLoad.Where((ConversationConfigMapper mapper) => !string.IsNullOrEmpty(mapper.CustomBundleName)).Share().SelectMany((Func<ConversationConfigMapper, IObservable<Conversation>>)CreatCustomConversationWithLoadIcon);
	}

	private IObservable<Conversation> CreatCustomConversationWithLoadIcon(ConversationConfigMapper conversationConfigMapper)
	{
		string customBundleName = conversationConfigMapper.CustomBundleName;
		return from data in _conversationCustomDataLoader.Load(customBundleName)
			select _conversationFactory.Create(conversationConfigMapper, data);
	}

	private void SendLog(ConversationConfigMapper x)
	{
	}

	public void Dispose()
	{
		releasingDisposable?.Dispose();
	}
}
