using System;
using GreenT;
using GreenT.AssetBundles.Scheduler;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Gallery;
using GreenT.Localizations;
using StripClub.Extensions;
using StripClub.Gallery;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace StripClub.Messenger.UI;

public class CharacterMessageView : MessageView<CharacterChatMessage>
{
	[Serializable]
	private struct CharacterMessageAuthorInfo
	{
		public GameObject Root;

		public Image Icon;

		public TMP_Text Name;
	}

	[SerializeField]
	private float secondsToMarkMessageAsRead = 1.5f;

	[SerializeField]
	private TMP_Text authorText;

	[SerializeField]
	private TMP_Text msgText;

	[SerializeField]
	private TMP_Text timeStamp;

	[SerializeField]
	private Image mediaThumbnail;

	[SerializeField]
	private GameObject messageArea;

	[SerializeField]
	private GameObject messageTail;

	[SerializeField]
	private GameObject imageTail;

	[SerializeField]
	private CharacterMessageAuthorInfo[] authorInfo;

	private GalleryController galleryController;

	private CharacterManager characterManager;

	private LocalizationService _localizationService;

	private bool isLast = true;

	private IDisposable _authorDisposable;

	private IDisposable _messageDisposable;

	private UnityAction OnButtonClick;

	private IDisposable delayStreamToMarkAsRead;

	[field: SerializeField]
	public Button MediaButton { get; private set; }

	public event Action<Media> OnMediaRequest;

	[Inject]
	public void Init(GalleryController galleryController, CharacterManager characterManager, LocalizationService localizationService)
	{
		this.galleryController = galleryController;
		this.characterManager = characterManager;
		_localizationService = localizationService;
	}

	public void OnEnable()
	{
		if (base.Source != null)
		{
			SetMessageAsRead(base.Source);
		}
	}

	public override void Set(CharacterChatMessage message)
	{
		base.Set(message);
		SetMessageAsRead(base.Source);
		this.OnMediaRequest = null;
		if (OnButtonClick != null)
		{
			MediaButton.onClick.RemoveListener(OnButtonClick);
		}
		UpdateAuthorInfo(message);
		_authorDisposable?.Dispose();
		_messageDisposable?.Dispose();
		if (authorText != null)
		{
			ICharacter character = characterManager.Get(message.CharacterID);
			_authorDisposable = ObservableExtensions.Subscribe<string>((IObservable<string>)_localizationService.ObservableText(character.NameKey), (Action<string>)delegate(string text)
			{
				authorText.text = text;
			});
		}
		_messageDisposable = ObservableExtensions.Subscribe<string>((IObservable<string>)_localizationService.ObservableText(message.LocalizationKey), (Action<string>)delegate(string text)
		{
			messageArea.SetActive(!string.IsNullOrEmpty(text));
			msgText.text = text;
		});
		timeStamp.text = message.Time.ToShortTimeString();
		if (!message.MediaID.HasValue)
		{
			messageTail.SetActive(isLast);
			MediaButton.gameObject.SetActive(value: false);
			return;
		}
		messageTail.SetActive(value: false);
		imageTail.SetActive(isLast);
		MediaButton.gameObject.SetActive(value: true);
		mediaThumbnail.sprite = null;
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Media>(galleryController.GetOrLoadMedia(message.MediaID.Value), (Action<Media>)delegate(Media _media)
		{
			mediaThumbnail.sprite = _media.Thumbnail;
			OnButtonClick = delegate
			{
				this.OnMediaRequest(_media);
			};
			MediaButton.onClick.AddListener(OnButtonClick);
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (Component)this);
		if (!MediaQuality.CheckSD(message.MediaID.Value))
		{
			return;
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Media>(Observable.Do<Media>(galleryController.GetOrLoadMedia(message.MediaID.Value), (Action<Media>)delegate(Media _media)
		{
			mediaThumbnail.sprite = _media.Thumbnail;
			OnButtonClick = delegate
			{
				this.OnMediaRequest(_media);
			};
		}), (Action<Media>)delegate
		{
		}), (Component)this);
	}

	private void UpdateAuthorInfo(CharacterChatMessage message)
	{
		CharacterMessageAuthorInfo[] array = authorInfo;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Root.SetActive(message.NamesVisibility);
		}
		if (message.NamesVisibility)
		{
			ICharacter character = characterManager.Get(message.CharacterID);
			string text = _localizationService.Text(character.NameKey);
			Sprite messengerAvatar = character.GetBundleData().MessengerAvatar;
			array = authorInfo;
			for (int i = 0; i < array.Length; i++)
			{
				CharacterMessageAuthorInfo characterMessageAuthorInfo = array[i];
				characterMessageAuthorInfo.Icon.sprite = messengerAvatar;
				characterMessageAuthorInfo.Name.text = text;
			}
		}
	}

	public override void MarkAsLast(bool isLast)
	{
		base.MarkAsLast(isLast);
		this.isLast = isLast;
		messageTail.SetActive(isLast && !base.Source.MediaID.HasValue);
		imageTail.SetActive(isLast && base.Source.MediaID.HasValue);
	}

	public void SetMessageAsRead(CharacterChatMessage chatMessage)
	{
		if (!chatMessage.State.Contains(ChatMessage.MessageState.Read))
		{
			delayStreamToMarkAsRead?.Dispose();
			delayStreamToMarkAsRead = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.TakeUntilDisable<long>(Observable.Timer(TimeSpan.FromSeconds(secondsToMarkMessageAsRead)), (Component)this), (Action<long>)delegate
			{
				chatMessage.AddFlag(ChatMessage.MessageState.Read);
			}), (Component)this);
		}
	}

	public override void Display(bool display)
	{
		base.Display(display);
	}

	private void OnDisable()
	{
		delayStreamToMarkAsRead?.Dispose();
		_authorDisposable?.Dispose();
		_messageDisposable?.Dispose();
	}
}
