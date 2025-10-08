using GreenT.HornyScapes.UI;
using GreenT.UI;
using Merge;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class StarsCloser : MonoBehaviour
{
	private StarResourceWindow _starResourceWindow;

	[Inject]
	private void Init(IWindowsManager windowsManager)
	{
		_starResourceWindow = windowsManager.Get<StarResourceWindow>();
	}

	private void OnEnable()
	{
		_starResourceWindow.SetActive(active: false);
	}

	private void OnDisable()
	{
		_starResourceWindow.SetActive(active: true);
	}
}
