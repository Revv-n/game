using System.Linq;
using GreenT.HornyScapes.Characters;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.Messenger.UI;

public class TabManager : ViewManagerBase<Conversation, ConversationTab>
{
	[SerializeField]
	private Transform tabContainer;

	private IFactory<ConversationTab> _tabFactory;

	private ToggleGroup ToggleGroup { get; set; }

	[Inject]
	public void Init(IFactory<ConversationTab> tabFactory, CharacterManager characterManager)
	{
		_tabFactory = tabFactory;
		InitToggleGroup();
	}

	private void InitToggleGroup()
	{
		ToggleGroup = tabContainer.gameObject.AddComponent<ToggleGroup>();
		ToggleGroup.allowSwitchOff = false;
	}

	protected override ConversationTab Create(Conversation conversation)
	{
		ConversationTab conversationTab = _tabFactory.Create();
		conversationTab.Set(conversation);
		Sprite icon = conversation.GetIcon();
		conversationTab.SetAvatar(icon);
		conversationTab.Toggle.isOn = false;
		conversationTab.Toggle.group = ToggleGroup;
		return conversationTab;
	}

	public ConversationTab Display(Conversation source, bool selected)
	{
		ConversationTab conversationTab = Display(source);
		if (!selected)
		{
			return conversationTab;
		}
		conversationTab.Toggle.isOn = true;
		ToggleGroup.NotifyToggleOn(conversationTab.Toggle);
		return conversationTab;
	}

	public override ConversationTab Display(Conversation source)
	{
		ConversationTab conversationTab = views.SingleOrDefault((ConversationTab obj) => obj.Source == source);
		if (conversationTab == null)
		{
			conversationTab = base.Display(source);
		}
		else
		{
			conversationTab.Display(display: true);
		}
		Sprite icon = source.GetIcon();
		conversationTab.SetAvatar(icon);
		return conversationTab;
	}

	public override void HideAll()
	{
		foreach (ConversationTab visibleView in base.VisibleViews)
		{
			visibleView.Hide();
		}
		base.HideAll();
	}
}
