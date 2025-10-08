using GreenT.HornyScapes;
using UnityEngine;
using Zenject;

namespace GreenT.Debugs;

public class Reload : MonoBehaviour
{
	[Inject]
	private GameStarter gameStarter;

	public void ReloadGame()
	{
		gameStarter.RestartApplication();
	}
}
