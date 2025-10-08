using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.UI;

public abstract class ScreenIndicator : MonoBehaviour
{
	private ReactiveProperty<bool> isVisible = new ReactiveProperty<bool>(initialValue: false);

	public IReadOnlyReactiveProperty<bool> IsVisible;

	protected IWindowsManager windowsManager;

	protected WindowGroupID windowPreset;

	protected GameStarter gameStarter;

	private List<WindowID> acceptableWindowIDs = new List<WindowID>();

	private IDisposable disposable;

	[Inject]
	private void Init(IWindowsManager windowsManager, WindowGroupID windowPreset, GameStarter gameStarter)
	{
		this.windowsManager = windowsManager;
		this.windowPreset = windowPreset;
		this.gameStarter = gameStarter;
		acceptableWindowIDs = windowPreset.GetWindows().ToList();
	}

	protected virtual void Awake()
	{
		IsVisible = isVisible.ToReadOnlyReactiveProperty();
	}

	protected virtual void Start()
	{
		SubscribeToWindowsOpenEvent();
	}

	protected virtual void OnEnable()
	{
		if (windowsManager != null)
		{
			UpdateIndicator();
			SubscribeToWindowsOpenEvent();
		}
	}

	protected virtual void OnDisable()
	{
		UnsubscribeToWindowsOpenEvent();
	}

	private void SubscribeToWindowsOpenEvent()
	{
		windowsManager.OnOpenWindow += OnAnyWindowChangeState;
		windowsManager.OnCloseWindow += OnAnyWindowChangeState;
	}

	private void UnsubscribeToWindowsOpenEvent()
	{
		windowsManager.OnOpenWindow -= OnAnyWindowChangeState;
		windowsManager.OnCloseWindow -= OnAnyWindowChangeState;
	}

	private void OnAnyWindowChangeState(IWindow obj)
	{
		if (gameStarter.IsGameActive.Value)
		{
			disposable?.Dispose();
			disposable = Observable.TimerFrame(1, FrameCountType.EndOfFrame).Subscribe(delegate
			{
				UpdateIndicator();
			});
		}
	}

	protected virtual void UpdateIndicator()
	{
		IEnumerable<WindowID> source = (from _window in windowsManager.GetOpened()
			select _window.WindowID).Except(acceptableWindowIDs);
		isVisible.Value = !source.Any();
	}

	protected virtual void OnDestroy()
	{
		isVisible.Dispose();
		(IsVisible as ReadOnlyReactiveProperty<bool>).Dispose();
	}
}
