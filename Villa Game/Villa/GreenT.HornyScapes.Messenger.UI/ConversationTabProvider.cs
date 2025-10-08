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
		tab.OnSet.Where((Conversation x) => x != null).Take(1).Subscribe(delegate
		{
			OnTabInitialize(tab);
		});
	}

	private void OnTabInitialize(ConversationTab tab)
	{
		_tabs.Add(tab);
		tab.OnSet.Value.OnHaveNewUpdate.Subscribe(OnUpdate).AddTo(_compositeDisposable);
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
