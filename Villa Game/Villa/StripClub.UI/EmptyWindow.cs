using GreenT.UI;
using UnityEngine;

namespace StripClub.UI;

public class EmptyWindow : Window
{
	[SerializeField]
	private GameObject uiBlocker;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (base.transform.parent.GetChild(base.transform.parent.childCount - 1) != base.transform)
		{
			base.transform.SetAsLastSibling();
			Debug.Log("Set as last", this);
		}
	}

	public override void Open()
	{
		base.Open();
		uiBlocker.SetActive(value: true);
	}

	public override void Close()
	{
		base.Close();
		uiBlocker.SetActive(value: false);
	}
}
