using GreenT.HornyScapes.Cheats;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class CheatJewelsHistoryButton : MonoBehaviour
{
	[SerializeField]
	private Button _buttonShowHistory;

	private JewelsObserverTemp _taskObserver;

	[Inject]
	private void Construct(JewelsObserverTemp taskObserver)
	{
		_taskObserver = taskObserver;
	}

	private void Awake()
	{
		_buttonShowHistory.onClick.AddListener(ShowHistory);
	}

	private void ShowHistory()
	{
	}
}
