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
		return Observable.ToArray<Conversation>(Observable.Merge<Conversation>(CreatConversation(conversationMapper), new IObservable<Conversation>[1] { CreatCustomConversation(conversationMapper) })).Debug(GetType().Name + ": Return conversation array", LogType.Data | LogType.Messenger);
	}

	public void ReleaseBundle(IEnumerable<int> param)
	{
		IObservable<ConversationConfigMapper> conversationMapper = GetConversationMapper(param);
		releasingDisposable = ObservableExtensions.Subscribe<ConversationConfigMapper>(Observable.Do<ConversationConfigMapper>(Observable.Where<ConversationConfigMapper>(conversationMapper, (Func<ConversationConfigMapper, bool>)((ConversationConfigMapper mapper) => !string.IsNullOrEmpty(mapper.CustomBundleName))), (Action<ConversationConfigMapper>)delegate(ConversationConfigMapper mapper)
		{
			_conversationCustomDataLoader.ReleaseBundle(mapper.CustomBundleName);
		}));
	}

	private IObservable<ConversationConfigMapper> GetConversationMapper(IEnumerable<int> conversationIDs)
	{
		return Observable.Share<ConversationConfigMapper>(Observable.DelayFrameSubscription<ConversationConfigMapper>(Observable.SelectMany<IEnumerable<ConversationConfigMapper>, ConversationConfigMapper>(Observable.Select<IEnumerable<ConversationConfigMapper>, IEnumerable<ConversationConfigMapper>>(_loader.Load(), (Func<IEnumerable<ConversationConfigMapper>, IEnumerable<ConversationConfigMapper>>)((IEnumerable<ConversationConfigMapper> mapperSet) => mapperSet.Where((ConversationConfigMapper mapper) => conversationIDs.Contains(mapper.ID)))), (Func<IEnumerable<ConversationConfigMapper>, IEnumerable<ConversationConfigMapper>>)((IEnumerable<ConversationConfigMapper> x) => x)), 1, (FrameCountType)0));
	}

	private IObservable<Conversation> CreatConversation(IObservable<ConversationConfigMapper> observableConversationMapperLoad)
	{
		return Observable.Select<ConversationConfigMapper, Conversation>(Observable.Do<ConversationConfigMapper>(Observable.Where<ConversationConfigMapper>(observableConversationMapperLoad, (Func<ConversationConfigMapper, bool>)((ConversationConfigMapper mapper) => string.IsNullOrEmpty(mapper.CustomBundleName))), (Action<ConversationConfigMapper>)SendLog), (Func<ConversationConfigMapper, Conversation>)_conversationFactory.Create);
	}

	private IObservable<Conversation> CreatCustomConversation(IObservable<ConversationConfigMapper> observableConversationMapperLoad)
	{
		return Observable.SelectMany<ConversationConfigMapper, Conversation>(Observable.Share<ConversationConfigMapper>(Observable.Where<ConversationConfigMapper>(observableConversationMapperLoad, (Func<ConversationConfigMapper, bool>)((ConversationConfigMapper mapper) => !string.IsNullOrEmpty(mapper.CustomBundleName)))), (Func<ConversationConfigMapper, IObservable<Conversation>>)CreatCustomConversationWithLoadIcon);
	}

	private IObservable<Conversation> CreatCustomConversationWithLoadIcon(ConversationConfigMapper conversationConfigMapper)
	{
		string customBundleName = conversationConfigMapper.CustomBundleName;
		return Observable.Select<CustomConversationData, Conversation>(_conversationCustomDataLoader.Load(customBundleName), (Func<CustomConversationData, Conversation>)((CustomConversationData data) => _conversationFactory.Create(conversationConfigMapper, data)));
	}

	private void SendLog(ConversationConfigMapper x)
	{
	}

	public void Dispose()
	{
		releasingDisposable?.Dispose();
	}
}
