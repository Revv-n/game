using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerListener : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public bool IsPointerOver { get; private set; }

	public event Action OnEnter;

	public event Action OnExit;

	public PointerListener AddEnterCallback(Action callback)
	{
		OnEnter += callback;
		return this;
	}

	public PointerListener AddExitCallback(Action callback)
	{
		OnExit += callback;
		return this;
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
	{
		IsPointerOver = true;
		this.OnEnter?.Invoke();
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
	{
		StartCoroutine(WaitNextFrameCrt());
	}

	private IEnumerator WaitNextFrameCrt()
	{
		yield return new WaitForEndOfFrame();
		IsPointerOver = false;
		this.OnExit?.Invoke();
	}
}
