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
		base.Container.BindInterfacesAndSelfTo<HouseCheat>().AsSingle();
		base.Container.Bind<AddAllItemsInPocketCheat>().FromInstance(addAllItemsInPocketCheat).AsSingle()
			.OnInstantiated<AddAllItemsInPocketCheat>(Setup)
			.NonLazy();
		Installer<RewindTimeCheatsInstaller>.Install(base.Container);
	}

	private void Setup(InjectContext context, AddAllItemsInPocketCheat cheat)
	{
		GameStarter gameStarter = context.Container.Resolve<GameStarter>();
		ContentSelectorGroup group = context.Container.Resolve<ContentSelectorGroup>();
		disposable?.Dispose();
		disposable = gameStarter.IsGameActive.Where((bool x) => x).Subscribe(delegate
		{
			group.Add(cheat);
		});
	}

	private void OnDestroy()
	{
		disposable?.Dispose();
	}
}
