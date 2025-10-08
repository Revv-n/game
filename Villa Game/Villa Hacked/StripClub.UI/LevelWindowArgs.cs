using System;

namespace StripClub.UI;

public class LevelWindowArgs : EventArgs, ICloneable
{
	public InterfaceElements visibleElements;

	public InterfaceElements interactableElements;

	public LevelWindowArgs(InterfaceElements visible = InterfaceElements.All, InterfaceElements interactable = InterfaceElements.All)
	{
		interactableElements = interactable;
		visibleElements = visible;
	}

	public object Clone()
	{
		return new LevelWindowArgs(visibleElements, interactableElements);
	}
}
