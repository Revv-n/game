using UnityEngine;

namespace StripClub.UI.Level;

public class InterfaceObject : MonoBehaviour, IVisible
{
	public bool Visible => base.gameObject.activeSelf;

	public virtual void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public virtual void Show()
	{
		base.gameObject.SetActive(value: true);
	}
}
