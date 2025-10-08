using UnityEngine;

namespace GreenT.HornyScapes.Settings.UI;

public class URLOpenerPlatformDataChanger : PlatformDataChanger<URLOpener>
{
	[SerializeField]
	private string keyNutaku;

	[SerializeField]
	private string keyEpocha;

	[SerializeField]
	private string keySteam;

	[SerializeField]
	private string keyHarem;

	[SerializeField]
	private string keyErolabs;

	protected override void SetNutaku()
	{
		entity.Set(keyNutaku);
	}

	protected override void SetEpocha()
	{
		entity.Set(keyEpocha);
	}

	protected override void SetSteam()
	{
		entity.Set(keySteam);
	}

	protected override void SetHarem()
	{
		entity.Set(keyHarem);
	}

	protected override void SetErolabs()
	{
		entity.Set(keyErolabs);
	}
}
