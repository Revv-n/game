using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Cheats;

public class CheatConsole : MonoBehaviour
{
	public bool ShowOnStart = true;

	public ScrollRect ScrollRect;

	[SerializeField]
	private GameObject logPrefab;

	[SerializeField]
	private RectTransform consolePanel;

	public bool moveScroll;

	public float scrollValue;

	private void Awake()
	{
		ChangePanelView(ShowOnStart);
	}

	public void ChangePanelView(bool state)
	{
		base.gameObject.SetActive(state);
		if (!state)
		{
			ClearConsole();
		}
	}

	public void HandleLog(string condition, string stacktrace, UnityEngine.LogType type)
	{
		AddMessageToConsole(condition);
	}

	public void AddMessageToConsole(string text)
	{
		if (base.gameObject.activeSelf)
		{
			if (text.Length > 512)
			{
				text = text.Remove(509) + " ...";
			}
			Text component = Object.Instantiate(logPrefab, consolePanel).GetComponent<Text>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(text);
			component.text = stringBuilder.ToString();
			SnapTo(component);
		}
	}

	public void ClearConsole()
	{
		foreach (Transform item in consolePanel.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public void ScrollTop()
	{
		scrollValue = 1f;
		moveScroll = true;
	}

	public void ScrollDown()
	{
		scrollValue = 0f;
		moveScroll = true;
	}

	private void SnapTo(MonoBehaviour obj)
	{
		if (obj.GetComponent<RectTransform>() != null)
		{
			ScrollDown();
		}
	}

	private string CollectionToString(string title, IEnumerable collection)
	{
		StringBuilder stringBuilder = new StringBuilder(title);
		bool flag = false;
		foreach (object item in collection)
		{
			stringBuilder.Append(item.ToString() + "; ");
			flag = true;
		}
		if (!flag)
		{
			stringBuilder.Append(" No elements.");
		}
		else
		{
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
			stringBuilder.Append(".");
		}
		return stringBuilder.ToString();
	}

	public void DisplayCollection(string title, IEnumerable collection, LogType type = (LogType)0)
	{
		string text = CollectionToString(title, collection);
		AddMessageToConsole(text);
	}
}
