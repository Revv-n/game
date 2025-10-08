using System.Text;
using TMPro;
using UnityEngine;

namespace StripClub.UI.Collections.Promote;

public class PrefView : MonoBehaviour, IView
{
	public class Manager : ViewManager<PrefView>
	{
	}

	[SerializeField]
	private TextMeshProUGUI pref;

	public int SiblingIndex
	{
		get
		{
			return base.transform.GetSiblingIndex();
		}
		set
		{
			base.transform.SetSiblingIndex(value);
		}
	}

	public void Display(bool isOn)
	{
		base.gameObject.SetActive(isOn);
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public void Init(object name, object value)
	{
		StringBuilder stringBuilder = BuildString(name, value);
		pref.text = stringBuilder.ToString();
	}

	public virtual StringBuilder BuildString(object name, object value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(name).Append(value);
		return stringBuilder;
	}

	public void Init(params object[] values)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(values[0]);
		for (int i = 1; i < values.Length; i++)
		{
			stringBuilder.Append(values[i]);
			stringBuilder.Append(" ");
		}
		pref.text = stringBuilder.ToString();
	}
}
