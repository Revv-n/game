using System;
using GreenT.HornyScapes.Cheats.UI;
using GreenT.HornyScapes.Events.Content;
using StripClub.Test;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatMergeInstaller : MonoInstaller<CheatMergeInstaller>
{
	public AddAllItemsInPocketCheat addAllItemsInPocketCheat;

	private IDisposable disposable;

	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<HouseCheat>()).AsSingle();
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((FromBinderGeneric<AddAllItemsInPocketCheat>)(object)((MonoInstallerBase)this).Container.Bind<AddAllItemsInPocketCheat>()).FromInstance(addAllItemsInPocketCheat).AsSingle()).OnInstantiated<AddAllItemsInPocketCheat>((Action<InjectContext, AddAllItemsInPocketCheat>)Setup)).NonLazy();
		Installer<RewindTimeCheatsInstaller>.Install(((MonoInstallerBase)this).Container);
	}

	private void Setup(InjectContext context, AddAllItemsInPocketCheat cheat)
	{
		GameStarter gameStarter = context.Container.Resolve<GameStarter>();
		ContentSelectorGroup group = context.Container.Resolve<ContentSelectorGroup>();
		disposable?.Dispose();
		disposable = ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Action<bool>)delegate
		{
			group.Add(cheat);
		});
	}

	private void OnDestroy()
	{
		disposable?.Dispose();
	}
}
