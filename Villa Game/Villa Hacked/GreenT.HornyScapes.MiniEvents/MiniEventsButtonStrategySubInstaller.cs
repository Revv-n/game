using System.Collections.Generic;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventsButtonStrategySubInstaller : MonoInstaller<MiniEventsButtonStrategySubInstaller>
{
	public const string MainToCollection = "MainToCollection";

	public const string ToSummon = "ToSummon";

	public const string ToBankSoft = "ToBankSoft";

	public const string ToBankHard = "ToBankHard";

	public const string ToMerge = "ToMerge";

	public const string ToChat = "ToChat";

	public const string ToBattlePass = "ToBattlePass";

	public const string ToRoulette = "ToRoulette";

	public const string ToBankMerge = "ToBankMerge";

	public WindowOpener MainToCollectionsOpener;

	public WindowOpener ToSummonsOpener;

	public WindowOpener ToBankSoftOpener;

	public WindowOpener ToBankHardOpener;

	public WindowOpener ToMergeOpener;

	public WindowOpener ToChatOpener;

	public WindowOpener ToBattlePassOpener;

	public WindowOpener ToBankMergeOpener;

	public override void InstallBindings()
	{
		Dictionary<string, WindowOpener> dictionary = new Dictionary<string, WindowOpener>
		{
			["MainToCollection"] = MainToCollectionsOpener,
			["ToSummon"] = ToSummonsOpener,
			["ToBankSoft"] = ToBankSoftOpener,
			["ToBankHard"] = ToBankHardOpener,
			["ToMerge"] = ToMergeOpener,
			["ToChat"] = ToChatOpener,
			["ToBattlePass"] = ToBattlePassOpener,
			["ToBankMerge"] = ToBankMergeOpener
		};
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventTabRedirector>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ButtonStrategyProvider>()).AsSingle()).WithArguments<Dictionary<string, WindowOpener>>(dictionary);
	}
}
