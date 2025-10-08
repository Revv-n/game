using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace StripClub.Stories;

public class AnimatedSpeaker : MonoBehaviour
{
	[SerializeField]
	[Range(0f, 1f)]
	private float _fadePower;

	[SerializeField]
	private float _animationDuration = 0.5f;

	[SerializeField]
	private float _scalePower;

	[SerializeField]
	private GameObject animatedSpeaker;

	private Canvas sortingOrderCanvas;

	private SkeletonGraphic image;

	private RectTransform rectTransform;

	private Canvas canvas;

	private Vector3 scale;

	public int CharacterData { get; set; }

	public void Fade()
	{
		float num = 1f - _fadePower;
		image.DOColor(new Color(num, num, num), _animationDuration);
		canvas.overrideSorting = false;
	}

	public void UnFade()
	{
		image.DOColor(Color.white, _animationDuration);
		canvas.overrideSorting = true;
		canvas.sortingOrder = sortingOrderCanvas.sortingOrder + 1;
	}

	public void Despawn()
	{
		Object.Destroy(base.gameObject);
	}

	public void Init(Canvas sortingOrderCanvas)
	{
		this.sortingOrderCanvas = sortingOrderCanvas;
		image = animatedSpeaker.GetComponent<SkeletonGraphic>();
		rectTransform = animatedSpeaker.GetComponent<RectTransform>();
		canvas = animatedSpeaker.GetComponent<Canvas>();
		scale = rectTransform.localScale;
	}
}
