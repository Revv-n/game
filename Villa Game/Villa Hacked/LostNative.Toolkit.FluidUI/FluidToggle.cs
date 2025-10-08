using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LostNative.Toolkit.FluidUI;

public class FluidToggle : MonoBehaviour
{
	public delegate void ToggleDelegate(bool isOn);

	public ToggleDelegate OnToggle;

	[SerializeField]
	protected RectTransform toggleRectTransform;

	[SerializeField]
	protected Image toggleContainerImage;

	[SerializeField]
	protected Image toggleLeftImage;

	[SerializeField]
	protected Image toggleCenterImage;

	[SerializeField]
	protected Image toggleRightImage;

	[Header("Toggle Sprites")]
	[SerializeField]
	protected Sprite toggleLeftSpriteOn;

	[SerializeField]
	protected Sprite toggleCenterSpriteOn;

	[SerializeField]
	protected Sprite toggleRightSpriteOn;

	[SerializeField]
	protected Sprite toggleLeftSpriteOff;

	[SerializeField]
	protected Sprite toggleCenterSpriteOff;

	[SerializeField]
	protected Sprite toggleRightSpriteOff;

	[Header("Toggle Label")]
	[SerializeField]
	protected TextMeshProUGUI toggleLabel;

	[SerializeField]
	protected string optionAText;

	[SerializeField]
	protected string optionBText;

	[Header("Animation Variables")]
	[SerializeField]
	protected float stretchTime = 0.2f;

	[SerializeField]
	protected float movementTime = 0.25f;

	[SerializeField]
	protected float compressedTime = 0.2f;

	[SerializeField]
	protected float stretchedSize = 117f;

	[SerializeField]
	protected float compressedSize = 35f;

	private float targetXPositionA;

	private float targetXPositionB;

	private float pivotResetPositionA;

	private float pivotResetPositionB;

	private const float RightPivot = 0f;

	private const float LeftPivot = 1f;

	private Sequence toggleSequence;

	private bool toggleSequenceAlive;

	private bool isOff;

	public bool IsOn => !isOff;

	private void Awake()
	{
		Init();
	}

	public void Init(bool isOn = true)
	{
		toggleSequenceAlive = false;
		isOff = !isOn;
		HandleSizing();
		SetSprites(isOn);
		StartToggle(isOff);
		toggleSequence.Complete();
	}

	public void SetWithoutNotify(bool isOn)
	{
		isOff = !isOn;
	}

	private void SetSprites(bool isOn)
	{
		toggleLeftImage.sprite = (isOn ? toggleLeftSpriteOn : toggleLeftSpriteOff);
		toggleCenterImage.sprite = (isOn ? toggleCenterSpriteOn : toggleCenterSpriteOff);
		toggleRightImage.sprite = (isOn ? toggleRightSpriteOn : toggleRightSpriteOff);
	}

	private void HandleSizing()
	{
		pivotResetPositionA = toggleRectTransform.anchoredPosition.x;
		toggleRectTransform.sizeDelta = new Vector2(compressedSize, toggleRectTransform.sizeDelta.y);
		targetXPositionA = toggleRectTransform.anchoredPosition.x - toggleRectTransform.sizeDelta.x;
		targetXPositionB = toggleRectTransform.anchoredPosition.x * -1f;
		pivotResetPositionB = targetXPositionB - toggleRectTransform.sizeDelta.x;
	}

	protected void StartToggle(bool isOff)
	{
		float targetXPosition = (isOff ? targetXPositionA : targetXPositionB);
		StartToggleSequence(targetXPosition);
	}

	protected virtual void StartToggleSequence(float targetXPosition)
	{
		toggleSequence = DOTween.Sequence();
		TweenerCore<Vector2, Vector2, VectorOptions> t = DOTweenModuleUI.DOAnchorPosX(toggleRectTransform, targetXPosition, movementTime);
		TweenerCore<Vector2, Vector2, VectorOptions> t2 = DOTweenModuleUI.DOSizeDelta(toggleRectTransform, new Vector2(stretchedSize, toggleRectTransform.sizeDelta.y), stretchTime);
		TweenerCore<Vector2, Vector2, VectorOptions> t3 = DOTweenModuleUI.DOSizeDelta(toggleRectTransform, new Vector2(compressedSize, toggleRectTransform.sizeDelta.y), compressedTime);
		toggleSequence.Append(t);
		toggleSequence.Join(t2);
		toggleSequence.Insert(stretchTime / 2f, t3);
		toggleSequence.OnComplete(OnToggleSequenceComplete);
		toggleSequence.Play();
		toggleSequenceAlive = true;
	}

	private void OnToggleSequenceComplete()
	{
		ResetPositionAtPivot();
		isOff = !isOff;
		SetSprites(isOff);
		toggleSequenceAlive = false;
	}

	private void ResetPositionAtPivot()
	{
		toggleRectTransform.pivot = new Vector2(isOff ? 1f : 0f, toggleRectTransform.pivot.y);
		toggleRectTransform.anchoredPosition = new Vector2(isOff ? pivotResetPositionA : pivotResetPositionB, toggleRectTransform.anchoredPosition.y);
	}

	[UsedImplicitly]
	public virtual void OnToggleClick()
	{
		FinishCurrentToggle();
		StartToggle(isOff);
		OnToggle?.Invoke(!isOff);
	}

	protected void FinishCurrentToggle()
	{
		if (toggleSequenceAlive)
		{
			toggleSequence.Complete();
		}
	}
}
