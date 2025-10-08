using System.Collections.Generic;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.UI;
using Zenject;

namespace GreenT.HornyScapes.Tasks.Data;

public class MergeTaskInstaller : MonoInstaller<MergeTaskInstaller>
{
	public const string MainToCollection = "MainToCollection";

	public const string ToSummon = "ToSummon";

	public WindowOpener MainToCollections;

	public WindowOpener ToSummons;

	public MergeTaskViewManagerView MergeTaskViewManagerView;

	public override void InstallBindings()
	{
		Dictionary<string, WindowOpener> param = new Dictionary<string, WindowOpener>
		{
			["MainToCollection"] = MainToCollections,
			["ToSummon"] = ToSummons
		};
		base.Container.BindInterfacesAndSelfTo<MiniEventTabRedirector>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ButtonStrategyProvider>().AsSingle().WithArguments(param);
		base.Container.BindInterfacesAndSelfTo<MergeTaskViewManagerView>().FromInstance(MergeTaskViewManagerView);
		BindContentSelector();
	}

	private void BindContentSelector()
	{
		base.Container.BindInterfacesAndSelfTo<TaskContentSelector>().AsSingle().OnInstantiated<TaskContentSelector>(ContentSelectorInstaller.AddSelectorToContainer)
			.NonLazy();
	}
}
