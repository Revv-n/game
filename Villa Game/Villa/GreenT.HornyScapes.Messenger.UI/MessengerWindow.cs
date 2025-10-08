using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Gallery;
using GreenT.HornyScapes.Resources.UI;
using GreenT.Model.Reactive;
using StripClub.Gallery;
using StripClub.Messenger;
using StripClub.Messenger.UI;
using StripClub.Model;
using StripClub.UI;
using StripClub.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public class MessengerWindow : PopupWindow
{
	[SerializeField]
	private ConversationHeader header;

	[SerializeField]
	private ConversationHistory history;

	[SerializeField]
	private UnlockInformation unlockInformation;

	private List<MonoView<Conversation>> conversationViews = new List<MonoView<Conversation>>();

	[Space]
	private MessageManager messageManager;

	[Space]
	[SerializeField]
	private Button closeBtn;

	[SerializeField]
	private Button galleryBtn;

	private IDisposable userUpdateRequestDisposable;

	private IDisposable multiChatDisposable;

	private IMessengerManager messenger;

	private GreenT.HornyScapes.Gallery.IGallery gallery;

	private TabManager tabManager;

	private GreenT.Model.Reactive.ReactiveCollection<CurrencyType> visibleCurrencies;

	private bool isMultiChatAvailable;

	private UpdateUserRequest updateUserRequest;

	private int _targetDisplayedConversation = 1;

	private ResourcesWindow resourceWindow;

	private IDisposable conversationUpdatesStream;

	private CurrencyType[] prevWindowsVisibleCurrencies;

	public MessageManager Manager => messageManager;

	public Conversation DisplayedConversation { get; private set; }

	[Inject]
	private void Init(IMessengerManager messenger, GreenT.HornyScapes.Gallery.IGallery gallery, TabManager tabManager, MessageManager messageManager, GreenT.Model.Reactive.ReactiveCollection<CurrencyType> visibleCurrencies, UpdateUserRequest updateUserRequest)
	{
		this.messenger = messenger;
		this.gallery = gallery;
		this.tabManager = tabManager;
		this.messageManager = messageManager;
		this.visibleCurrencies = visibleCurrencies;
		this.updateUserRequest = updateUserRequest;
	}

	protected override void Awake()
	{
		base.Awake();
		conversationViews.Add(header);
		conversationViews.Add(history);
		conversationViews.Add(unlockInformation);
		closeBtn.onClick.AddListener(Close);
	}

	private void OnEnable()
	{
		tabManager.HideAll();
		(from conversation in messenger.GetConversations()
			where conversation.Dialogues.Any()
			select conversation).ToObservable().DelayFrame(1).SelectMany((Func<Conversation, IObservable<Conversation>>)EmitConversationOnSelect)
			.TakeUntilDisable(this)
			.Subscribe(DisplayConversation)
			.AddTo(this);
		(from _updateArgs in messenger.OnUpdate
			where _updateArgs.Conversation != null
			select _updateArgs.Conversation).TakeUntilDisable(this).Subscribe(OnNewConversation).AddTo(this);
	}

	public override void Open()
	{
		if (!IsOpened)
		{
			prevWindowsVisibleCurrencies = visibleCurrencies.ToArray();
		}
		visibleCurrencies.RemoveItems(prevWindowsVisibleCurrencies);
		base.Open();
		if (resourceWindow == null)
		{
			resourceWindow = windowsManager.Get<ResourcesWindow>();
		}
		resourceWindow.Close();
	}

	public override void Close()
	{
		visibleCurrencies.SetItems(prevWindowsVisibleCurrencies);
		base.Close();
		resourceWindow.Open();
	}

	private void AddReplyEnergy()
	{
		windowsManager.Get<ReplyExtenderWindow>().Open();
	}

	public void SetupGalleryButtons()
	{
		Type type = StripClub.Utility.Content.ToType(MediaType.Photos);
		MediaFilter filter = new MediaFilter().WhereType(type);
		IEnumerable<Media> media = gallery.GetMedia(filter);
		galleryBtn.gameObject.SetActive(media.Any());
		if (!media.Any())
		{
			gallery.OnMediaAdded.Where((Media _media) => StripClub.Utility.Content.ToMediaType(_media.Info.Type) == MediaType.Photos).First().Subscribe(delegate
			{
				galleryBtn.gameObject.SetActive(value: true);
			}, delegate(Exception ex)
			{
				ex.LogException();
			})
				.AddTo(this);
		}
	}

	private IObservable<Conversation> EmitConversationOnSelect(Conversation conversation)
	{
		return from x in tabManager.Display(conversation, _targetDisplayedConversation.Equals(conversation.ID)).Toggle.OnValueChangedAsObservable()
			where x
			select x into _
			select conversation;
	}

	private void OnNewConversation(Conversation conversation)
	{
		EmitConversationOnSelect(conversation).Subscribe(DisplayConversation).AddTo(this);
	}

	public void RequestShowConversationOnOpenWindow(Conversation conversation)
	{
		_targetDisplayedConversation = conversation.ID;
	}

	private void DisplayConversation(Conversation conversation)
	{
		if (DisplayedConversation == conversation)
		{
			return;
		}
		conversationUpdatesStream?.Dispose();
		DisplayedConversation = conversation;
		_targetDisplayedConversation = conversation.ID;
		foreach (MonoView<Conversation> conversationView in conversationViews)
		{
			conversationView.Set(DisplayedConversation);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		conversationUpdatesStream?.Dispose();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		multiChatDisposable?.Dispose();
		userUpdateRequestDisposable?.Dispose();
	}

	public void IsMultiChatAvailable()
	{
	}
}
