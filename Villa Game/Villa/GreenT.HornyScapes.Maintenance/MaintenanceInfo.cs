namespace GreenT.HornyScapes.Maintenance;

public class MaintenanceInfo
{
	public readonly bool NeedUpdateClient;

	public readonly bool ConfigIsOld;

	public readonly bool MaintenanceTime;

	public MaintenanceInfo(bool needUpdateClient, bool configIsOld, bool maintenanceTime)
	{
		NeedUpdateClient = needUpdateClient;
		ConfigIsOld = configIsOld;
		MaintenanceTime = maintenanceTime;
	}
}
