using System;
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
		Dictionary<string, WindowOpener> dictionary = new Dictionary<string, WindowOpener>
		{
			["MainToCollection"] = MainToCollections,
			["ToSummon"] = ToSummons
		};
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventTabRedirector>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ButtonStrategyProvider>()).AsSingle()).WithArguments<Dictionary<string, WindowOpener>>(dictionary);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MergeTaskViewManagerView>().FromInstance((object)MergeTaskViewManagerView);
		BindContentSelector();
	}

	private void BindContentSelector()
	{
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskContentSelector>()).AsSingle()).OnInstantiated<TaskContentSelector>((Action<InjectContext, TaskContentSelector>)ContentSelectorInstaller.AddSelectorToContainer)).NonLazy();
	}
}
