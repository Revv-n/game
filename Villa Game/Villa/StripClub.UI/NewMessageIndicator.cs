using DG.Tweening;
using GreenT.Localizations;
using StripClub.Extensions;
using StripClub.Messenger;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI;

public class NewMessageIndicator : MonoBehaviour
{
	[SerializeField]
	private Text message;

	[SerializeField]
	private Image flare;

	[Inject]
	private IMessengerManager messenger;

	[Header("Animation settings")]
	[SerializeField]
	private float appearanceDuraion = 1.5f;

	[SerializeField]
	private float flareAppearanceDuration = 1f;

	[SerializeField]
	private float flareAnimationDuration = 2f;

	[SerializeField]
	private Vector3 flareShakeSettings;

	[SerializeField]
	private Vector2 rotationSettings;

	private RectTransform rectTransform;

	private Vector2 messageAwakeSize;

	private Vector3 flareAwakeScale;

	private LocalizationService _localizationService;

	[Inject]
	public void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		messageAwakeSize = rectTransform.sizeDelta;
		flareAwakeScale = flare.rectTransform.localScale;
	}

	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		(from _args in messenger.OnUpdate.TakeUntilDisable(this)
			where _args.Message != null
			where _args.Message.State.Contains(ChatMessage.MessageState.Delivered) && !_args.Message.State.Contains(ChatMessage.MessageState.Read)
			select _args).Subscribe(delegate
		{
			message.text = _localizationService.Text("level.new_message_indicator");
			AppearanceAnimation();
		}).AddTo(this);
	}

	[EditorButton]
	public void TestAnimation()
	{
		AppearanceAnimation();
	}

	private void AppearanceAnimation()
	{
		if (!base.gameObject.activeSelf)
		{
			DOTween.Sequence().OnStart(AnimationInitialization).Append(rectTransform.DOSizeDelta(messageAwakeSize, appearanceDuraion))
				.Append(flare.DOFade(1f, flareAppearanceDuration))
				.Join(flare.transform.DOScale(flareAwakeScale, flareAppearanceDuration))
				.Append(flare.transform.DOShakeScale(flareAnimationDuration, flareShakeSettings.x, (int)flareShakeSettings.y, flareShakeSettings.z))
				.Join(flare.transform.DORotate(flare.transform.rotation.eulerAngles + rotationSettings.x * Vector3.forward, flareAnimationDuration))
				.Append(flare.transform.DOScale(0f, 1f))
				.Join(flare.DOFade(0f, flareAppearanceDuration))
				.Join(flare.transform.DOLocalRotate(flare.transform.rotation.eulerAngles + rotationSettings.x * Vector3.forward, rotationSettings.y))
				.Append(rectTransform.DOSizeDelta(new Vector2(0f, messageAwakeSize.y), appearanceDuraion))
				.OnComplete(delegate
				{
					base.gameObject.SetActive(value: false);
				});
		}
	}

	private void AnimationInitialization()
	{
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
		flare.color = new Color(flare.color.r, flare.color.g, flare.color.b, 0f);
		flare.rectTransform.localScale = Vector3.zero;
		base.gameObject.SetActive(value: true);
	}
}
