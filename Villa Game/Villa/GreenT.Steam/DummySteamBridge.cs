using System;

namespace GreenT.Steam;

public class DummySteamBridge : ISteamBridge
{
	private readonly User _user;

	public DummySteamBridge(User user)
	{
		_user = user;
	}

	public void InitAuth(Action<User> onSuccess)
	{
		onSuccess(_user);
	}
}
