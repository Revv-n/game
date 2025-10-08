using Steamworks;
using Zenject;

namespace GreenT.Steam;

public sealed class SteamCallbackRunner : ITickable
{
	public void Tick()
	{
		SteamAPI.RunCallbacks();
	}
}
