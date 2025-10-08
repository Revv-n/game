using GreenT.HornyScapes.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace Merge.Meta.RoomObjects;

public class RoomObjectBord : MonoBehaviour
{
	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Vector3 showScale;

	[SerializeField]
	private float showAlpha;

	[SerializeField]
	private AnimationGroup showAnim;

	[SerializeField]
	private AnimationGroup rewardAnim;

	[SerializeField]
	private AnimationGroup collectableEffectAnim;

	[SerializeField]
	private Image collectableEffect;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Image bordSprite;

	[SerializeField]
	private Sprite backGroundCollectableSprite;

	[SerializeField]
	private Sprite backGroundNonCollectableSprite;

	[SerializeField]
	private Sprite bordCollectableSprite;

	[SerializeField]
	private Sprite bordNonCollectableSprite;

	[SerializeField]
	private Vector3 collectableScale;

	[SerializeField]
	private Vector3 nonCollectableScale;

	private void Awake()
	{
		canvas.worldCamera = Camera.main;
		showAnim.Init();
		rewardAnim.Init();
		collectableEffectAnim.Init();
	}

	public void Display(bool active)
	{
		if (!base.gameObject.activeSelf && active)
		{
			showAnim.transform.localScale = showScale;
			canvasGroup.alpha = showAlpha;
			base.gameObject.SetActive(value: true);
			showAnim.Play();
		}
		else
		{
			base.gameObject.SetActive(active);
		}
	}

	public void SetCollectable(bool active)
	{
		if (active)
		{
			collectableEffectAnim.Play();
			rewardAnim.Play();
			collectableEffect.gameObject.SetActive(value: true);
			background.sprite = backGroundCollectableSprite;
			bordSprite.sprite = bordCollectableSprite;
			bordSprite.transform.localScale = collectableScale;
		}
		else
		{
			collectableEffect.gameObject.SetActive(value: false);
			background.sprite = backGroundNonCollectableSprite;
			bordSprite.sprite = bordNonCollectableSprite;
			bordSprite.transform.localScale = nonCollectableScale;
		}
	}
}
