using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GreenT.HornyScapes.UI;

public class AdvanceScaleButton : ScaleButton
{
	[SerializeField]
	private Image overlay;

	[SerializeField]
	private Sprite highlightedOverlay;

	[SerializeField]
	private Sprite pressedOverlay;

	[SerializeField]
	private Sprite selectedOverlay;

	[SerializeField]
	private Sprite disableOverlay;

	private Sprite defaultOverlay;

	protected override void Start()
	{
		base.Start();
		if (Application.isPlaying)
		{
			defaultOverlay = overlay.sprite;
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(ObserveExtensions.ObserveEveryValueChanged<AdvanceScaleButton, bool>(this, (Func<AdvanceScaleButton, bool>)((AdvanceScaleButton x) => x.interactable), (FrameCountType)0, false), (Action<bool>)InteractableSprite), (Component)this);
		}
	}

	public void ChangeDefaultOverlay(Sprite newOverlay)
	{
		defaultOverlay = newOverlay;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		if (pressedOverlay != null && IsInteractable())
		{
			overlay.sprite = pressedOverlay;
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		if (IsInteractable())
		{
			overlay.sprite = defaultOverlay;
		}
	}

	public override void OnSelect(BaseEventData eventData)
	{
		base.OnSelect(eventData);
		if (selectedOverlay != null && IsInteractable())
		{
			overlay.sprite = selectedOverlay;
		}
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		base.OnDeselect(eventData);
		if (IsInteractable())
		{
			overlay.sprite = defaultOverlay;
		}
	}

	private void InteractableSprite(bool interactable)
	{
		if (IsInteractable())
		{
			overlay.sprite = defaultOverlay;
		}
		else
		{
			overlay.sprite = disableOverlay;
		}
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		if (IsInteractable() && highlightedOverlay != null)
		{
			overlay.sprite = highlightedOverlay;
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (IsInteractable())
		{
			overlay.sprite = defaultOverlay;
		}
	}
}
