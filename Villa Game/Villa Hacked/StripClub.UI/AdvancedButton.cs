using System;
using ModestTree;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StripClub.UI;

public class AdvancedButton : Button
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

	protected override void Awake()
	{
		Assert.IsNotNull((object)overlay);
	}

	protected override void Start()
	{
		base.Start();
		defaultOverlay = overlay.sprite;
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(ObserveExtensions.ObserveEveryValueChanged<AdvancedButton, bool>(this, (Func<AdvancedButton, bool>)((AdvancedButton x) => x.interactable), (FrameCountType)0, false), (Action<bool>)InteractableSprite), (Component)this);
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
