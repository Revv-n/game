using System.Linq;
using GreenT.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class OpenProgressWindow : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private WindowOpener openWindow;

	public int WindowState;

	private EventProgressView eventProgressView;

	private void OnValidate()
	{
		if (WindowState != 0 && WindowState != 1 && WindowState != 2)
		{
			Debug.LogError(GetType().Name + ": unknown window state");
		}
	}

	[Inject]
	private void InnerInit(EventProgressView eventProgressView)
	{
		this.eventProgressView = eventProgressView;
	}

	private void Awake()
	{
		button.onClick.AddListener(OpenWindow);
	}

	private void OnDestroy()
	{
		button.onClick.RemoveAllListeners();
	}

	private void OpenWindow()
	{
		if (eventProgressView.Source.RewardHolder.GetAllRewardsContent().Any())
		{
			SetViewWindow();
			openWindow.Click();
		}
	}

	private void SetViewWindow()
	{
		eventProgressView.SetViewState(WindowState);
	}
}
