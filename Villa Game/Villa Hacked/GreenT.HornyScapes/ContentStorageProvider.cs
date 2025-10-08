using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.External.StripClub._Scripts.NewEventScripts;
using GreenT.Types;
using JetBrains.Annotations;
using StripClub.Model;
using StripClub.NewEvent.Data;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
[MementoHolder]
public class ContentStorageProvider : ISavableState, IDisposable
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		[field: SerializeField]
		public List<LinkedContent.Map> ContentMap { get; private set; }

		[field: SerializeField]
		public ContentType ContentType { get; private set; }

		public Memento(ContentStorageProvider storageProvider)
			: base(storageProvider)
		{
			ContentMap = storageProvider.mainStoredContent?.GetContentMap() ?? new List<LinkedContent.Map>();
			List<LinkedContent.Map> collection = storageProvider.mainShowContent?.GetContentMap() ?? new List<LinkedContent.Map>();
			ContentMap.AddRange(collection);
			ContentType = storageProvider.ContentType;
		}
	}

	private readonly ContentSelectorGroup contentSelector;

	private readonly IContentAdder contentAdder;

	private readonly EventProvider _provider;

	private readonly EventStateService _stateService;

	private readonly IContentFactory contentFactory;

	private readonly Subject<ContentStorageProvider> onUpdate = new Subject<ContentStorageProvider>();

	private LinkedContent mainShowContent;

	private LinkedContent mainStoredContent;

	public IObservable<ContentStorageProvider> OnUpdate => Observable.AsObservable<ContentStorageProvider>((IObservable<ContentStorageProvider>)onUpdate);

	public ContentType ContentType { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public ContentStorageProvider(IContentFactory contentFactory, EventStateService stateService, ContentSelectorGroup contentSelector, IContentAdder contentAdder, EventProvider provider)
	{
		this.contentFactory = contentFactory;
		this.contentSelector = contentSelector;
		this.contentAdder = contentAdder;
		_provider = provider;
		_stateService = stateService;
	}

	public string UniqueKey()
	{
		return "content.waiting.to.apply";
	}

	public void UpdateState()
	{
		ContentType = contentSelector.Current;
	}

	public bool TrySetStoredContent(LinkedContent content, ContentType contentType)
	{
		if (!TryDistributeSetStoredContent(content, contentType))
		{
			return false;
		}
		if (contentType == ContentType.Event && !_stateService.HaveActiveEvent)
		{
			contentType = ContentType.Main;
		}
		ContentType = contentType;
		onUpdate.OnNext(this);
		return true;
	}

	private bool TryDistributeSetStoredContent(LinkedContent content, ContentType contentType)
	{
		mainStoredContent = content;
		return content != null;
	}

	public bool TrySetStoredContent(LinkedContent content)
	{
		return TrySetStoredContent(content, contentSelector.Current);
	}

	public bool TrySetStoreShow(LinkedContent content)
	{
		if (!TryDistributeSetStoreShow(content, contentSelector.Current))
		{
			return false;
		}
		ContentType contentType = contentSelector.Current;
		if (contentType == ContentType.Event && !_stateService.HaveActiveEvent)
		{
			contentType = ContentType.Main;
		}
		ContentType = contentType;
		onUpdate.OnNext(this);
		return true;
	}

	private bool TryDistributeSetStoreShow(LinkedContent content, ContentType contentType)
	{
		mainShowContent = content;
		return content != null;
	}

	public void AddToPlayer()
	{
		if (TryGetStoredContent(ContentType, out var content))
		{
			contentAdder.AddContent(content);
			Clear();
		}
	}

	public bool TryGetStoredContent(ContentType contentType, out LinkedContent content)
	{
		content = mainStoredContent;
		return content != null;
	}

	public bool HasStoredContent()
	{
		LinkedContent content;
		if (ContentType == ContentType.Event && !_stateService.HaveActiveEvent)
		{
			return TryGetStoredContent(ContentType, out content);
		}
		if (ContentType == contentSelector.Current)
		{
			return TryGetStoredContent(ContentType, out content);
		}
		return false;
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		if (memento2.ContentMap == null || memento2.ContentMap.Count == 0)
		{
			return;
		}
		mainStoredContent = contentFactory.Create(memento2.ContentMap[0]);
		foreach (LinkedContent.Map item in memento2.ContentMap.Skip(1))
		{
			LinkedContent content = contentFactory.Create(item);
			mainStoredContent.Insert(content);
		}
		ContentType = memento2.ContentType;
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void Dispose()
	{
		onUpdate.OnCompleted();
		onUpdate.Dispose();
	}

	public void ClearShow()
	{
		mainShowContent = null;
		onUpdate.OnNext(this);
	}

	private void Clear()
	{
		mainStoredContent = null;
		onUpdate.OnNext(this);
	}
}
