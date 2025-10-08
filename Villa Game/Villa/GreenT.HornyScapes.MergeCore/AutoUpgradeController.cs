using Merge;

namespace GreenT.HornyScapes.MergeCore;

public class AutoUpgradeController : Controller<AutoUpgradeController>
{
	private GameItemController Field => Controller<GameItemController>.Instance;

	public override void Preload()
	{
		base.Preload();
		Field.OnItemCreated += AtItemCreated;
		Field.OnItemTakenFromSomethere += AtItemCreated;
	}

	private void AtItemCreated(GameItem item)
	{
		if (item.Config.TryGetModule<ModuleConfigs.AutoUpgrade>(out var result))
		{
			ModuleDatas.AutoUpgrade autoUpgrade = item.Data.GetModule<ModuleDatas.AutoUpgrade>() ?? AddDefaultModuleData(item, result);
			if (autoUpgrade.MainTimer.IsCompleted)
			{
				UpgradeItem(item, autoUpgrade, result);
				return;
			}
			GIBox.AutoUpgrade autoUpgrade2 = new GIBox.AutoUpgrade(autoUpgrade, result);
			autoUpgrade2.OnTimerComplete += AtBoxTimerComplite;
			autoUpgrade2.StartTweenTimer(autoUpgrade.MainTimer);
			item.AddBox(autoUpgrade2);
		}
	}

	private void AtBoxTimerComplite(IControlClocks sender)
	{
		GIBox.AutoUpgrade autoUpgrade = sender as GIBox.AutoUpgrade;
		UpgradeItem(autoUpgrade.Parent, autoUpgrade.Data, autoUpgrade.Config);
	}

	private void UpgradeItem(GameItem item, ModuleDatas.AutoUpgrade data, ModuleConfigs.AutoUpgrade config)
	{
		Field.RemoveItem(item);
		Field.CreateItem(config.Target.Copy().SetCoordinates(item.Coordinates)).DoCreate();
	}

	private ModuleDatas.AutoUpgrade AddDefaultModuleData(GameItem sender, ModuleConfigs.AutoUpgrade config)
	{
		ModuleDatas.AutoUpgrade autoUpgrade = new ModuleDatas.AutoUpgrade();
		autoUpgrade.MainTimer = TimeMaster.GetRefTimer(config.Time);
		sender.Data.Modules.Add(autoUpgrade);
		return autoUpgrade;
	}
}
