using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatTime : MonoBehaviour
{
	[SerializeField]
	private Button ShowNowTime;

	[Inject]
	private IClock clock;

	private void Awake()
	{
		ShowNowTime.onClick.AddListener(ShowTimeNow);
	}

	private void ShowTimeNow()
	{
	}
}
