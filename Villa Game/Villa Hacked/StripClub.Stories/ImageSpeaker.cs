using DG.Tweening;
using GreenT.HornyScapes.Sounds;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.Stories;

[RequireComponent(typeof(Image), typeof(Canvas))]
public class ImageSpeaker : MonoBehaviour
{
	[SerializeField]
	private SoundSO completedPhrase;

	[SerializeField]
	private Canvas _sortingOrderCanvas;

	[SerializeField]
	[Range(0f, 1f)]
	private float _fadePower;

	[SerializeField]
	private float _animationDuration = 0.5f;

	[SerializeField]
	private float _scalePower;

	private Canvas _canvas;

	private Image _image;

	private RectTransform _rectTransform;

	private Vector3 _scale;

	[Inject]
	private IAudioPlayer audioPlayer;

	public Sprite Image
	{
		set
		{
			_image.sprite = value;
		}
	}

	public int CharacterData { get; set; }

	protected void OnValidate()
	{
		if (completedPhrase == null)
		{
			Debug.LogError("Empty completedPhrase sound", this);
		}
	}

	private void Awake()
	{
		_image = GetComponent<Image>();
		_rectTransform = GetComponent<RectTransform>();
		_canvas = GetComponent<Canvas>();
		_scale = _rectTransform.localScale;
		Fade();
	}

	public void Fade()
	{
		float num = 1f - _fadePower;
		DOTweenModuleUI.DOColor(_image, new Color(num, num, num), _animationDuration);
		_canvas.overrideSorting = false;
	}

	public void UnFade()
	{
		DOTweenModuleUI.DOColor(_image, Color.white, _animationDuration);
		_canvas.overrideSorting = true;
		_canvas.sortingOrder = _sortingOrderCanvas.sortingOrder + 1;
		audioPlayer.PlayOneShotAudioClip2D(completedPhrase.Sound);
	}
}
