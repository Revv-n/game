using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Level.UI;

public class LevelUpWindowAnimations : MonoBehaviour
{
	[Header("OverView Settings")]
	[SerializeField]
	private float animationDuration = 0.15f;

	[SerializeField]
	private float startYScale = 1.5f;

	[SerializeField]
	private float startAlpha = 0.6f;

	[SerializeField]
	private float hideDuration = 0.6f;

	[SerializeField]
	private float hideScaleX = 0.5f;

	[SerializeField]
	private float hideScaleY = 0.9f;

	[SerializeField]
	private Image popUp;

	[Header("Inner Settings")]
	[SerializeField]
	private float innerStartScale = 1.2f;

	[Header("Girls Settings")]
	[SerializeField]
	private float girlsSlideDistance = 300f;

	[SerializeField]
	private float girlsSlideTime = 0.5f;

	[SerializeField]
	private Ease girlsSlideEase = Ease.OutFlash;

	[SerializeField]
	private CanvasGroup main;

	[SerializeField]
	private CanvasGroup allGraphic;

	[SerializeField]
	private Image leftGirl;

	[SerializeField]
	private Image rightGirl;

	private Vector3 startWindowScale;

	private Color popUpColor;

	private Vector3 popUpScale;

	private Vector3 allGraphicScale;

	private float leftGirlX;

	private float rightGirlX;

	private void Awake()
	{
		startWindowScale = base.transform.localScale;
		popUpColor = popUp.color;
		popUpScale = popUp.transform.localScale;
		allGraphicScale = allGraphic.transform.localScale;
		leftGirlX = leftGirl.transform.localPosition.x;
		rightGirlX = rightGirl.transform.localPosition.x;
	}

	public void ShowPopUp()
	{
		ResetAnimation();
		popUp.DOColor(popUpColor, animationDuration);
		popUp.transform.DOScale(popUpScale, animationDuration).OnComplete(ShowMain);
	}

	private void ShowMain()
	{
		InitGirl(leftGirl, leftGirlX);
		InitGirl(rightGirl, rightGirlX);
		Vector3 localScale = main.transform.localScale;
		main.transform.localScale *= innerStartScale;
		main.DOFade(1f, animationDuration);
		main.gameObject.SetActive(value: true);
		main.transform.DOScale(localScale, animationDuration);
	}

	private void InitGirl(Image girl, float slide)
	{
		girl.gameObject.SetActive(value: true);
		girl.DOFade(1f, animationDuration);
		girl.transform.DOLocalMoveX(slide, girlsSlideTime).SetEase(girlsSlideEase);
	}

	public Sequence Hide()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(allGraphic.transform.DOScale(new Vector3(hideScaleX, hideScaleY, startWindowScale.z), hideDuration));
		sequence.Join(allGraphic.DOFade(0f, hideDuration));
		return sequence;
	}

	private void ResetAnimation()
	{
		ResetPopUp();
		ResetGirl(leftGirl, Vector3.right);
		ResetGirl(rightGirl, Vector3.left);
	}

	private void ResetGirl(Image girl, Vector3 slideVector)
	{
		girl.gameObject.SetActive(value: false);
		Color color = girl.color;
		color.a = 0f;
		girl.color = color;
		girl.transform.localPosition += slideVector * girlsSlideDistance;
	}

	private void ResetPopUp()
	{
		base.transform.localScale = startWindowScale;
		Color color = popUpColor;
		color.a = 0f;
		popUp.transform.localScale = new Vector3(0f, startYScale, popUpScale.z);
		popUp.color = color;
		allGraphic.transform.localScale = allGraphicScale;
		main.gameObject.SetActive(value: false);
		main.alpha = 0f;
		allGraphic.alpha = 1f;
	}
}
