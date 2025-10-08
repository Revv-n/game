using UnityEngine;
using Zenject;

namespace GreenT.Core;

public class AspectRatioInstaller : MonoInstaller<AspectRatioInstaller>
{
	[SerializeField]
	private float width = 1920f;

	[SerializeField]
	private float height = 1080f;

	public override void InstallBindings()
	{
	}

	private void SetAspect(InjectContext context, AspectRatioSetter setter)
	{
		setter.SetRatio(width, height);
	}
}
