using Merge;

namespace GreenT.HornyScapes.Settings.UI;

public class CopyIDPlatformDependency : PlatformDataChanger<CopyID>
{
	private void Awake()
	{
		if (PlatformHelper.IsEpochaMonetization())
		{
			SetEpocha();
		}
		else if (PlatformHelper.IsNutakuMonetization())
		{
			SetNutaku();
		}
		else if (PlatformHelper.IsSteamMonetization())
		{
			SetSteam();
		}
		else if (PlatformHelper.IsHaremMonetization())
		{
			SetHarem();
		}
		else if (PlatformHelper.IsErolabsMonetization())
		{
			SetErolabs();
		}
	}

	protected override void SetNutaku()
	{
		entity.SetActive(active: false);
	}

	protected override void SetEpocha()
	{
		entity.SetActive(active: true);
	}

	protected override void SetSteam()
	{
		entity.SetActive(active: true);
	}

	protected override void SetHarem()
	{
		entity.SetActive(active: true);
	}

	protected override void SetErolabs()
	{
		entity.SetActive(active: true);
	}
}
