using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameToggle : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private RectTransform knob;

	[SerializeField]
	private Image back;

	[SerializeField]
	private RectTransform a;

	[SerializeField]
	private RectTransform b;

	[SerializeField]
	private Sprite pressedSprite;

	[SerializeField]
	private Sprite notPressedSprite;

	[SerializeField]
	private float changeTime;

	private Tween changeTween;

	public bool Value { get; private set; }

	public event Action<bool> OnValueChange;

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		DoChangeValue();
	}

	public void DoChangeValue()
	{
		changeTween?.Kill();
		Value = !Value;
		back.sprite = (Value ? pressedSprite : notPressedSprite);
		Vector2 endValue = (Value ? b.anchoredPosition : a.anchoredPosition);
		DOTweenModuleUI.DOAnchorPos(knob, endValue, changeTime);
		this.OnValueChange?.Invoke(Value);
	}

	public void SetValue(bool value)
	{
		changeTween?.Kill();
		Value = value;
		back.sprite = (Value ? pressedSprite : notPressedSprite);
		knob.anchoredPosition = (Value ? b.anchoredPosition : a.anchoredPosition);
	}

	private void OnDisable()
	{
		changeTween?.Kill(complete: true);
	}
}
