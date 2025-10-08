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
		base.Container.BindInterfacesTo<ImageFactory>().FromInstance(imagePoolFactory);
		base.Container.Bind<ImagePool>().FromInstance(imagePool).AsSingle();
	}
}
