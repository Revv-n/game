using System;

namespace GreenT.Steam;

public interface ISteamBridge
{
	void InitAuth(Action<User> onSuccess);
}
