using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Messenger;
using StripClub.Messenger.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Messenger.UI;

public class ConversationTabProvider : MonoBehaviour
{
	[SerializeField]
	private RectTransform _root;

	private List<ConversationTab> _tabs = new List<ConversationTab>();

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public RectTransform Root => _root;

	public void AddTab(ConversationTab tab)
	{
		ObservableExtensions.Subscribe<Conversation>(Observable.Take<Conversation>(Observable.Where<Conversation>((IObservable<Conversation>)tab.OnSet, (Func<Conversation, bool>)((Conversation x) => x != null)), 1), (Action<Conversation>)delegate
		{
			OnTabInitialize(tab);
		});
	}

	private void OnTabInitialize(ConversationTab tab)
	{
		_tabs.Add(tab);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Conversation>(tab.OnSet.Value.OnHaveNewUpdate, (Action<Conversation>)OnUpdate), (ICollection<IDisposable>)_compositeDisposable);
		OnUpdate(tab.OnSet.Value);
	}

	private void OnUpdate(Conversation conversation)
	{
		_tabs = _tabs.OrderByDescending((ConversationTab x) => x.LastTimeUpdate).ToList();
		for (int i = 0; i < _tabs.Count; i++)
		{
			_tabs[i].transform.SetSiblingIndex(i);
		}
	}

	private void OnDestroy()
	{
		_compositeDisposable.Dispose();
	}
}
