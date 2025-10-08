using System;
using GreenT.Localizations;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.Messenger.UI;

public class ConversationHeader : MonoView<Conversation>
{
	[SerializeField]
	private TMP_Text conversationTitle;

	[SerializeField]
	private Image conversationIcon;

	private Conversation currentConversation;

	private LocalizationService _localizationService;

	private IDisposable _titleDisposable;

	[Inject]
	public void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public override void Set(Conversation conversation)
	{
		base.Set(conversation);
		currentConversation = conversation;
		_titleDisposable?.Dispose();
		_titleDisposable = _localizationService.ObservableText(currentConversation.NameKey).Subscribe(delegate(string localizedName)
		{
			conversationTitle.text = localizedName;
		});
		conversationIcon.sprite = conversation.GetIcon();
	}

	private void OnDisable()
	{
		_titleDisposable?.Dispose();
	}
}
