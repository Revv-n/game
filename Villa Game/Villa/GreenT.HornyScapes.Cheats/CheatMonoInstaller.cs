using GreenT.Cheats;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatMonoInstaller : MonoInstaller<CheatMonoInstaller>
{
	public ConsoleLogSettingsSO consoleLogSettings;

	public InputSettingCheats inputSetting;

	public ConsoleCanvas console;

	public override void InstallBindings()
	{
	}
}
