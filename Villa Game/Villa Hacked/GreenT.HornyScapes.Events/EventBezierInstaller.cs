using StripClub.Rewards;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventBezierInstaller : MonoInstaller<EventBezierInstaller>
{
	[SerializeField]
	private ImagePool imagePool;

	[SerializeField]
	private ImageFactory imagePoolFactory;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesTo<ImageFactory>().FromInstance((object)imagePoolFactory);
		((FromBinderGeneric<ImagePool>)(object)((MonoInstallerBase)this).Container.Bind<ImagePool>()).FromInstance(imagePool).AsSingle();
	}
}
