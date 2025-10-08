using System;
using System.Collections.Generic;

namespace GreenT.UI;

public interface IWindowsManager
{
	IObservable<IWindow> OnNew { get; }

	event Action<IWindow> OnOpenWindow;

	event Action<IWindow> OnCloseWindow;

	void Add(IWindow window);

	void Open(IWindow window);

	void OpenOnly(IEnumerable<IWindow> window);

	void Close(IWindow window);

	void MoveWindowForward(IWindow window);

	void MoveWindowBackwards(IWindow window, IWindow frontWindow);

	void Remove(IWindow window);

	T Get<T>() where T : IWindow;

	IEnumerable<IWindow> GetWindows(params WindowID[] windowsID);

	IEnumerable<IWindow> GetWindows(params Type[] windowTypes);

	IWindow GetWindow(WindowID windowsID);

	IEnumerable<IWindow> GetOpened();

	void MoveWindowToSortingOrder(IWindow window, int sortingOrder);
}
