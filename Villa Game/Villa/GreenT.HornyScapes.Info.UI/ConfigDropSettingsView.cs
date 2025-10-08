using GreenT.Multiplier;
using Merge;
using Zenject;

namespace GreenT.HornyScapes.Info.UI;

public class ConfigDropSettingsView : GameItemConfigView
{
	[Inject]
	private MultiplierManager multiplierManager;

	public override void Set(GIConfig giConfig)
	{
		base.Set(giConfig);
		if (!giConfig.TryGetModule<ModuleConfigs.ClickSpawn>(out var result))
		{
			Display(display: false);
			return;
		}
		Display(display: true);
		int num = result.MaxAmount;
		double num2 = (giConfig.NotAffectedAll ? GetMaxAmountWithoutTotalMultiplier(giConfig).Factor.Value : GetMaxAmountMulty(giConfig).Factor.Value);
		bool flag = num2 > 0.0;
		if (num2 > 0.0)
		{
			num += (int)num2;
			string text = result.MaxAmount + " + " + num2;
			Bonus.SetArguments(text);
		}
		Bonus.gameObject.SetActive(flag);
		NoBonusPlaceholder.gameObject.SetActive(!flag);
		Total.SetArguments(num);
	}

	private IMultiplier GetMaxAmountMulty(GIConfig config)
	{
		return multiplierManager.SpawnerMaxAmountMultipliers.TotalByKey(config.UniqId);
	}

	private IMultiplier GetMaxAmountWithoutTotalMultiplier(GIConfig config)
	{
		return multiplierManager.SpawnerMaxAmountMultipliers.GetMultiplier(config.UniqId);
	}
}
