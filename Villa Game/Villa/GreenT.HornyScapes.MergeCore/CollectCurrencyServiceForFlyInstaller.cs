using Merge.MotionDesign;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class CollectCurrencyServiceForFlyInstaller : MonoInstaller
{
	[SerializeField]
	private CurrencyFlyTweenBuilder CurrencyFlyTweenBuilder;

	public override void InstallBindings()
	{
		base.Container.Bind<CurrencyFlyTweenBuilder>().FromComponentInNewPrefab(CurrencyFlyTweenBuilder).AsSingle();
		base.Container.Bind<CollectCurrencyServiceForFly>().AsSingle();
	}
}
