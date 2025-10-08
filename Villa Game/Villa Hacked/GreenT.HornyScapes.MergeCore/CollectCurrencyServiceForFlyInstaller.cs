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
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.Bind<CurrencyFlyTweenBuilder>()).FromComponentInNewPrefab((Object)CurrencyFlyTweenBuilder)).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<CollectCurrencyServiceForFly>()).AsSingle();
	}
}
