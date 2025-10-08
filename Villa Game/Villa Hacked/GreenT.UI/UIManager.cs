using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Extensions;
using UniRx;
using UnityEngine;

namespace GreenT.UI;

[DisallowMultipleComponent]
public class UIManager : MonoBehaviour, IWindowsManager
{
	[SerializeField]
	private BackgroundNew background;

	private Dictionary<WindowID, IWindow> windows = new Dictionary<WindowID, IWindow>();

	private Dictionary<IWindow, int> windowCanvasSortingOrder = new Dictionary<IWindow, int>();

	private Subject<IWindow> onNew = new Subject<IWindow>();

	private List<IWindow> openedWindows = new List<IWindow>();

	public IObservable<IWindow> OnNew => Observable.AsObservable<IWindow>((IObservable<IWindow>)onNew);

	public event Action<IWindow> OnOpenWindow;

	public event Action<IWindow> OnCloseWindow;

	public IEnumerable<IWindow> GetOpened()
	{
		return openedWindows;
	}

	public void OpenOnly(IEnumerable<IWindow> windows)
	{
		CloseAll(windows);
		foreach (IWindow window in windows)
		{
			window.Open();
		}
	}

	public void Open(IWindow window)
	{
		if (openedWindows.Contains(window))
		{
			MoveWindowForward(window);
			background.UpdateBackGround(window);
			return;
		}
		openedWindows.Add(window);
		MoveWindowForward(window);
		background.UpdateBackGround(window);
		this.OnOpenWindow?.Invoke(window);
	}

	public void CloseAll(IEnumerable<IWindow> keepOpenedWindows)
	{
		IWindow[] array = openedWindows.Where((IWindow _window) => (_window.Settings & WindowSettings.VisibleOnLoad) == 0 && !keepOpenedWindows.Contains(_window)).ToArray();
		for (int num = array.Length - 1; num >= 0; num--)
		{
			array[num].Close();
		}
	}

	public void Remove(IWindow window)
	{
		WindowID windowID = window.WindowID;
		Close(window);
		windows.Remove(windowID);
	}

	public void Close(IWindow window)
	{
		openedWindows.Remove(window);
		IWindow window2 = GetBackgroundWindows().LastOrDefault();
		if (window2 != null)
		{
			background.UpdateBackGround(window2);
		}
		else
		{
			background.Hide();
		}
		if (window.Canvas != null)
		{
			window.Canvas.sortingOrder = windowCanvasSortingOrder[window];
		}
		this.OnCloseWindow?.Invoke(window);
	}

	public void MoveWindowForward(IWindow window)
	{
		IEnumerable<IWindow> source = GetNonStaticWindows().Except(window.AsEnumerable());
		if (!IsWindowStatic(window) && source.Any())
		{
			int num = (((window.Settings & WindowSettings.UseBackground) == 0) ? 1 : 2);
			window.Canvas.sortingOrder = source.Max((IWindow _window) => _window.Canvas.sortingOrder) + num;
		}
	}

	public void MoveWindowToSortingOrder(IWindow window, int sortingOrder)
	{
		window.Canvas.sortingOrder = sortingOrder;
	}

	public void MoveWindowBackwards(IWindow window, IWindow frontWindow)
	{
		IEnumerable<IWindow> source = GetNonStaticWindows().Except(window.AsEnumerable());
		if (!IsWindowStatic(window) && source.Any())
		{
			window.Canvas.sortingOrder = frontWindow.Canvas.sortingOrder - 1;
		}
	}

	private IEnumerable<IWindow> GetNonStaticWindows()
	{
		return openedWindows.Where((IWindow window) => (window.Settings & WindowSettings.StaticDisplayOrder) == 0);
	}

	private IEnumerable<IWindow> GetBackgroundWindows()
	{
		return openedWindows.Where((IWindow window) => (window.Settings & WindowSettings.UseBackground) != 0);
	}

	private bool IsWindowStatic(IWindow window)
	{
		return (window.Settings & WindowSettings.StaticDisplayOrder) != 0;
	}

	public void Add(IWindow window)
	{
		WindowID windowID = window.WindowID;
		windows[windowID] = window;
		windowCanvasSortingOrder[window] = ((window.Canvas != null) ? window.Canvas.sortingOrder : 0);
		if (window.IsOpened)
		{
			openedWindows.Add(window);
		}
		onNew.OnNext(window);
	}

	public T Get<T>() where T : IWindow
	{
		Type windowType = typeof(T);
		return (T)windows.Values.FirstOrDefault((IWindow _windowType) => _windowType.GetType() == windowType);
	}

	public IEnumerable<IWindow> GetWindows(params WindowID[] windowsID)
	{
		return windowsID.Join(windows, (WindowID _windowID) => _windowID, delegate(KeyValuePair<WindowID, IWindow> _window)
		{
			KeyValuePair<WindowID, IWindow> keyValuePair2 = _window;
			return keyValuePair2.Key;
		}, delegate(WindowID _windowID, KeyValuePair<WindowID, IWindow> _window)
		{
			KeyValuePair<WindowID, IWindow> keyValuePair = _window;
			return keyValuePair.Value;
		});
	}

	public IEnumerable<IWindow> GetWindows(params Type[] windowTypes)
	{
		return from _type in windowTypes
			join _window in windows.Values on _type equals _window.GetType()
			select _window;
	}

	public IWindow GetWindow(WindowID windowsID)
	{
		return windows.Values.FirstOrDefault((IWindow _window) => _window.WindowID == windowsID);
	}

	private void OnDestroy()
	{
		onNew?.Dispose();
	}
}
