using UnityEngine.Events;
using UnityEngine.UI;

namespace Merge;

public static class ButtonExtention
{
	public static void AddClickCallback(this Button btn, UnityAction callback)
	{
		btn.onClick.AddListener(callback);
	}

	public static void SetClickCallback(this Button btn, UnityAction callback)
	{
		btn.onClick.RemoveAllListeners();
		btn.onClick.AddListener(callback);
	}
}
