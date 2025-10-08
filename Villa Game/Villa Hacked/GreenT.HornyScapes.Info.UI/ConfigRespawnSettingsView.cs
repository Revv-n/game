using System;
using GreenT.Multiplier;
using Merge;
using StripClub.Extensions;
using Zenject;

namespace GreenT.HornyScapes.Info.UI;

public class ConfigRespawnSettingsView : GameItemConfigView
{
	private MultiplierManager multiplierManager;

	private TimeHelper timeHelper;

	[Inject]
	public void Init(MultiplierManager multiplierManager, TimeHelper timeHelper)
	{
		this.multiplierManager = multiplierManager;
		this.timeHelper = timeHelper;
	}

	public override void Set(GIConfig giConfig)
	{
		base.Set(giConfig);
		if (!giConfig.TryGetModule<ModuleConfigs.ClickSpawn>(out var result) || result.RestoreTime <= 0f)
		{
			Display(display: false);
			return;
		}
		Display(display: true);
		float restoreTime = result.RestoreTime;
		double num = (giConfig.NotAffectedAll ? GetReloadWithoutTotalMultiplier(giConfig).Factor.Value : GetReloadMultiplier(giConfig).Factor.Value);
		string text = timeHelper.UseCombineFormat(TimeSpan.FromSeconds(restoreTime));
		bool flag = num != 1.0;
		if (flag)
		{
			restoreTime *= (float)num;
			text = timeHelper.UseCombineFormat(TimeSpan.FromSeconds(restoreTime));
			double num2 = (0.0 - (1.0 - num)) * 100.0;
			Bonus.SetArguments($"{num2:.##}");
		}
		Total.SetArguments(text);
		Bonus.gameObject.SetActive(flag);
		NoBonusPlaceholder.gameObject.SetActive(!flag);
	}

	private IMultiplier GetReloadMultiplier(GIConfig config)
	{
		return multiplierManager.SpawnerReloadMultipliers.TotalByKey(config.UniqId);
	}

	private IMultiplier GetReloadWithoutTotalMultiplier(GIConfig config)
	{
		return multiplierManager.SpawnerReloadMultipliers.GetMultiplier(config.UniqId);
	}
}
