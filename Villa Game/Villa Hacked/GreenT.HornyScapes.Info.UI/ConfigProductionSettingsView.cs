using GreenT.Multiplier;
using Merge;
using Zenject;

namespace GreenT.HornyScapes.Info.UI;

public class ConfigProductionSettingsView : GameItemConfigView
{
	[Inject]
	private MultiplierManager multiplierManager;

	public override void Set(GIConfig giConfig)
	{
		base.Set(giConfig);
		if (!giConfig.TryGetModule<ModuleConfigs.ClickSpawn>(out var result) || result.RestoreTime <= 0f)
		{
			Display(display: false);
			return;
		}
		Display(display: true);
		int num = 1;
		double num2 = (giConfig.NotAffectedAll ? GetProductionWithoutTotalMultiplier(giConfig).Factor.Value : GetProductionMulty(giConfig).Factor.Value);
		bool flag = num2 != 0.0;
		if (flag)
		{
			num = (int)(((double)num - num2) * 100.0);
			double num3 = (1.0 + num2) * 100.0;
			Bonus.SetArguments(num3.ToString("+#;-#;0"));
		}
		Bonus.gameObject.SetActive(flag);
		NoBonusPlaceholder.gameObject.SetActive(!flag);
		Total.SetArguments(num);
	}

	private IMultiplier GetProductionMulty(GIConfig config)
	{
		return multiplierManager.SpawnerProductionMultipliers.TotalByKey(config.UniqId);
	}

	private IMultiplier GetProductionWithoutTotalMultiplier(GIConfig config)
	{
		return multiplierManager.SpawnerProductionMultipliers.GetMultiplier(config.UniqId);
	}
}
