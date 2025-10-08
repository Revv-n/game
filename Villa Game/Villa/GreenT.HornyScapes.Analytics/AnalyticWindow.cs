using GreenT.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics;

public abstract class AnalyticWindow<T> : AnalyticMonoBehaviour where T : Window
{
	[SerializeField]
	protected T window;

	private void OnValidate()
	{
		if (!window)
		{
			TryGetComponent<T>(out window);
		}
		if (!window)
		{
			Debug.LogError(GetType().Name + ": AnalyticMonobeh is empty", this);
		}
	}
}
