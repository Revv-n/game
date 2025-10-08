using UnityEngine;
using Zenject;

namespace GreenT.Login;

public class LoginSceneInstaller : MonoInstaller<LoginSceneInstaller>
{
	[SerializeField]
	private GameObject loginWindowPrefab;

	public override void InstallBindings()
	{
	}
}
