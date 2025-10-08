using UnityEngine;

namespace GreenT.HornyScapes;

public class ApplicationCloser : MonoBehaviour
{
	public void Exit()
	{
		PlayerWantsToQuit.AllowQuitting = true;
		Application.Quit();
	}
}
