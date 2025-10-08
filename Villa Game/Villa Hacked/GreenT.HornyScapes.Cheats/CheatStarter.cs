using UnityEngine.SceneManagement;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatStarter : IInitializable
{
	public void Initialize()
	{
		SceneManager.LoadSceneAsync(6, LoadSceneMode.Additive);
	}
}
