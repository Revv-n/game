using System;
using UnityEngine;
using Zenject;

namespace GreenT.UI;

public class Window : MonoBehaviour, IWindow
{
	protected IWindowsManager windowsManager;

	[field: SerializeField]
	public WindowID WindowID { get; protected set; }

	[field: SerializeField]
	public WindowSettings Settings { get; protected set; }

	[field: SerializeField]
	public Canvas Canvas { get; set; }

	public virtual bool IsOpened { get; protected set; }

	public event EventHandler<EventArgs> OnChangeState;

	protected virtual void OnValidate()
	{
		if (WindowID == null)
		{
			Debug.LogWarning(GetType().Name + ": has no window ID", this);
		}
	}

	[Inject]
	public virtual void Init(IWindowsManager windowsOpener)
	{
		windowsManager = windowsOpener;
		windowsManager.Add(this);
	}

	protected virtual void Awake()
	{
		if (Settings.HasFlag(WindowSettings.VisibleOnLoad))
		{
			Open();
		}
		else
		{
			SetActive(isActive: false);
		}
	}

	protected virtual void SetActive(bool isActive)
	{
		base.gameObject.SetActive(isActive);
		IsOpened = isActive;
	}

	public virtual void Open()
	{
		if (IsOpened)
		{
			windowsManager.Open(this);
			return;
		}
		SetActive(isActive: true);
		windowsManager.Open(this);
		this.OnChangeState?.Invoke(this, new WindowArgs(IsOpened));
	}

	public virtual void Close()
	{
		if (IsOpened)
		{
			SetActive(isActive: false);
			windowsManager.Close(this);
			this.OnChangeState?.Invoke(this, new WindowArgs(IsOpened));
		}
	}

	protected virtual void OnDestroy()
	{
		windowsManager?.Remove(this);
	}
}
