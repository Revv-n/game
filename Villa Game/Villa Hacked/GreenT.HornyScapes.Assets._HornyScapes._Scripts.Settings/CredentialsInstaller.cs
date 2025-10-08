using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Assets._HornyScapes._Scripts.Settings;

public class CredentialsInstaller : MonoInstaller
{
	[SerializeField]
	private CredentialsWindow window;

	public override void InstallBindings()
	{
		((FromBinderGeneric<CredentialsWindow>)(object)((MonoInstallerBase)this).Container.Bind<CredentialsWindow>()).FromInstance(window);
	}
}
