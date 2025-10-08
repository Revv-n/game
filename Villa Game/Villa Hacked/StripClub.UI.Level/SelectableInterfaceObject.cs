using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI.Level;

[RequireComponent(typeof(Selectable))]
public class SelectableInterfaceObject : InterfaceObject, IInteractable
{
	[SerializeField]
	private Selectable source;

	public virtual bool Interactable
	{
		get
		{
			return source.interactable;
		}
		set
		{
			source.interactable = value;
		}
	}

	protected virtual void OnValidate()
	{
		if (source == null)
		{
			source = GetComponentInChildren<Selectable>();
		}
	}
}
