using System;
using UnityEngine;

namespace GreenT.UI;

public interface IWindow
{
	WindowID WindowID { get; }

	WindowSettings Settings { get; }

	Canvas Canvas { get; }

	bool IsOpened { get; }

	event EventHandler<EventArgs> OnChangeState;

	void Open();

	void Close();
}
