using System;

namespace GreenT.HornyScapes.Monetization;

[Serializable]
public class SteamUserResponse
{
	public int status;

	public string error;

	public SteamUserData data;
}
