using System;
using UnityEngine;

namespace GreenT.HornyScapes;

public class DeviceSystem
{
	public string GetId()
	{
		string empty = string.Empty;
		if (PlayerPrefs.HasKey("amp_deviceID"))
		{
			empty = PlayerPrefs.GetString("amp_deviceID");
		}
		else
		{
			empty = Guid.NewGuid().ToString();
			PlayerPrefs.SetString("amp_deviceID", empty);
			PlayerPrefs.Save();
		}
		return empty;
	}

	public string GetPlatform()
	{
		_ = string.Empty;
		return "windows";
	}

	public string GetSessionId()
	{
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
	}
}
