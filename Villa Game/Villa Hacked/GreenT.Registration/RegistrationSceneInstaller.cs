using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.Registration;

public class RegistrationSceneInstaller : MonoInstaller<RegistrationSceneInstaller>
{
	[SerializeField]
	private Window registrationWindowPrefab;

	[SerializeField]
	private Canvas registrationWindowCanvas;

	public override void InstallBindings()
	{
	}

	private void UpdateWindow(InjectContext context, Window window)
	{
		window.Canvas = registrationWindowCanvas;
	}
}
