using System;

namespace GreenT.UI;

public class WindowArgs : EventArgs
{
	public bool Active { get; protected set; }

	public WindowArgs(bool active)
	{
		Active = active;
	}
}
