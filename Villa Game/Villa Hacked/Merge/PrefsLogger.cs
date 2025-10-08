using UnityEngine;

namespace Merge;

public class PrefsLogger<T> : BaseLogger<T> where T : new()
{
	public override bool SendLogs
	{
		get
		{
			return PlayerPrefs.GetInt(Key + "_AllowLogs", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt(Key + "_AllowLogs", value ? 1 : 0);
		}
	}
}
