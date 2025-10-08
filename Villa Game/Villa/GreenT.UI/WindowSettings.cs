using System;

namespace GreenT.UI;

[Flags]
public enum WindowSettings
{
	VisibleOnLoad = 1,
	UseBackground = 2,
	StaticDisplayOrder = 4
}
