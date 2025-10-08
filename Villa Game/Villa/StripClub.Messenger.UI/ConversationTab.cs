using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.UI;
using GreenT.Localizations;
using StripClub.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.Messenger.UI;

[RequireComponent(typeof(Toggle))]
public sealed class ConversationTab : ConversationView
{
	private const string typingPlaceholder = "ui.messenger.somebody_is_typing";

	private const string photoPlaceholder = "ui.messenger.conversation_tab.photo_placeholder";

	[SerializeField]
	private Toggle toggle;

	[SerializeField]
	private List<ConversationView> childConversationViews;

	[SerializeField]
	private Color textColor;

	[SerializeField]
	private Color textColorSelected;

	[SerializeField]
	private Image avatar;

	[SerializeField]
	private TMP_Text characterName;

	[SerializeField]
	private TMP_Text overviewTxt;

	[SerializeField]
	private TMP_Text currentMessages;

	[SerializeField]
	private TMP_Text totalMessages;

	[SerializeField]
	private ProgressBar mediaProgressBar;

	public ReactiveProperty<Conversation> OnSet = new ReactiveProperty<Conversation>();

	private IMessengerManager messenger;

	private LocalizationService _localizationService;

	private IDisposable _nameDisposable;

	private IDisposable _overviewDisposable;

	private string _overviewKey;

	private IDisposable updateStream;

	public Toggle Toggle => toggle;

	public DateTime LastTimeUpdate => OnSet.Value.LastTimeUpdate;

	[Inject]
	public void Init(IMessengerManager messenger, LocalizationService localizationService)
	{
		this.messenger = messenger;
		_localizationService = localizationService;
	}

	private void Start()
	{
		ApplySettings(toggle.isOn);
		toggle.onValueChanged.AddListener(ApplySettings);
	}

	public override void Set(Conversation conversation)
	{
		base.Set(conversation);
		SetConversationName(conversation.NameKey);
		UpdateView(conversation);
		updateStream?.Dispose();
		updateStream = messenger.OnUpdate.Where(delegate(MessengerUpdateArgs _args)
		{
			if (_args.Message != null)
			{
				ChatMessage message = _args.Message;
				CharacterChatMessage characterMessage = message as CharacterChatMessage;
				if (characterMessage != null && conversation.Dialogues.Any((Dialogue _dialogue) => _dialogue.ID == characterMessage.DialogueID))
				{
					return true;
				}
			}
			return _args.Dialogue != null && conversation.Dialogues.Any((Dialogue _dialogue) => _dialogue.ID == _args.Dialogue.ID);
		}).Subscribe(delegate
		{
			UpdateView(conversation);
		}).AddTo(this);
		foreach (ConversationView childConversationView in childConversationViews)
		{
			childConversationView.Set(conversation);
		}
		OnSet.Value = base.Source;
	}

	private void OnEnable()
	{
		if (base.Source != null)
		{
			Set(base.Source);
		}
	}

	private void OnDisable()
	{
		updateStream?.Dispose();
	}

	public void SetOverview(string message)
	{
		overviewTxt.text = message;
	}

	public void SetConversationName(string nameKey)
	{
		_nameDisposable?.Dispose();
		_nameDisposable = _localizationService.ObservableText(nameKey).Subscribe(delegate(string text)
		{
			characterName.text = text;
		});
	}

	public void SetMessageProgress(int currentMessagesAmount, int totalMessages)
	{
		if (currentMessages != null)
		{
			currentMessages.text = currentMessagesAmount.ToString();
		}
		if (this.totalMessages != null)
		{
			this.totalMessages.text = totalMessages.ToString();
		}
	}

	public void SetAvatar(Sprite icon = null)
	{
		avatar.sprite = icon;
		avatar.transform.parent.gameObject.SetActive(icon != null);
	}

	private void UpdateView(Conversation conversation)
	{
		SetMessageProgress(conversation.CurrentMessages, conversation.TotalMessages);
		mediaProgressBar.Init(conversation.CurrentMedia, conversation.TotalMedia, 0f);
		string overview = string.Empty;
		CharacterChatMessage characterChatMessage = conversation.Dialogues.Last().Messages.OfType<CharacterChatMessage>().LastOrDefault();
		if (characterChatMessage != null)
		{
			_overviewKey = ((!characterChatMessage.State.Contains(ChatMessage.MessageState.Delivered)) ? "ui.messenger.somebody_is_typing" : (characterChatMessage.MediaID.HasValue ? _localizationService.Text("ui.messenger.conversation_tab.photo_placeholder") : characterChatMessage.LocalizationKey));
			overview = _localizationService.Text(_overviewKey);
			_overviewDisposable?.Dispose();
			_overviewDisposable = _localizationService.ObservableText(_overviewKey).Subscribe(delegate(string text)
			{
				SetOverview(text);
			});
		}
		SetOverview(overview);
	}

	private void ApplySettings(bool isOn)
	{
		mediaProgressBar.gameObject.SetActive(isOn);
		if (!isOn)
		{
			characterName.color = textColor;
			overviewTxt.color = textColor;
		}
		else
		{
			characterName.color = textColorSelected;
			overviewTxt.color = textColorSelected;
		}
	}

	public void Hide()
	{
		Toggle.isOn = false;
		_nameDisposable?.Dispose();
		_overviewDisposable?.Dispose();
	}

	private void OnValidate()
	{
		if (toggle == null)
		{
			toggle = GetComponent<Toggle>();
		}
	}
}
