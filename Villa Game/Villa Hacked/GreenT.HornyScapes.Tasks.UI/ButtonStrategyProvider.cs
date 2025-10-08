using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Events.Content;
using GreenT.UI;

namespace GreenT.HornyScapes.Tasks.UI;

public class ButtonStrategyProvider
{
	private const string MainToCollection = "MainToCollection";

	private const string MainToSummon = "ToSummon";

	private const string ToBankSoft = "ToBankSoft";

	private const string ToBankHard = "ToBankHard";

	private const string ToMerge = "ToMerge";

	private const string ToChat = "ToChat";

	private const string ToBattlePass = "ToBattlePass";

	private const string ToBankMerge = "ToBankMerge";

	private readonly MiniEventTabRedirector miniEventTabRedirection;

	private readonly Dictionary<string, WindowOpener> strategies;

	private readonly ContentSelectorGroup content;

	public ButtonStrategyProvider(ContentSelectorGroup content, Dictionary<string, WindowOpener> strategies, MiniEventTabRedirector miniEventTabRedirection)
	{
		this.content = content;
		this.strategies = strategies;
		this.miniEventTabRedirection = miniEventTabRedirection;
	}

	public bool TryGetStrategy(ActionButtonType type, out WindowOpener windowsGroups)
	{
		switch (type)
		{
		case ActionButtonType.ToCollection:
			if (!content.IsMain())
			{
				goto case ActionButtonType.None;
			}
			if (content.IsMain())
			{
				windowsGroups = strategies["MainToCollection"];
				break;
			}
			goto default;
		case ActionButtonType.None:
			windowsGroups = null;
			break;
		case ActionButtonType.ToSummonBank:
			windowsGroups = strategies["ToSummon"];
			break;
		case ActionButtonType.ToBankSoft:
			windowsGroups = strategies["ToBankSoft"];
			break;
		case ActionButtonType.ToBankHard:
			windowsGroups = strategies["ToBankHard"];
			break;
		case ActionButtonType.ToMerge:
			windowsGroups = strategies["ToMerge"];
			break;
		case ActionButtonType.ToChat:
			windowsGroups = strategies["ToChat"];
			break;
		case ActionButtonType.ToBattlePass:
			windowsGroups = strategies["ToBattlePass"];
			break;
		case ActionButtonType.ToBankMerge:
			windowsGroups = strategies["ToBankMerge"];
			break;
		case ActionButtonType.ToRoulette:
			windowsGroups = null;
			break;
		default:
			throw new NotImplementedException($"Try get WindowOpener but, ActionButtonType {type} not implemented");
		}
		return windowsGroups != null;
	}

	public bool TryGetStrategy(ActionButtonType type, out MiniEventTabRedirector miniEventTabRedirection)
	{
		switch (type)
		{
		case ActionButtonType.None:
			miniEventTabRedirection = null;
			break;
		case ActionButtonType.ToRoulette:
			miniEventTabRedirection = this.miniEventTabRedirection;
			break;
		default:
			throw new NotImplementedException($"Try get miniEventTabRedirection, but ActionButtonType {type} not implemented");
		}
		return miniEventTabRedirection != null;
	}
}
