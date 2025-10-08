using UnityEngine;

namespace GreenT.HornyScapes;

public abstract class PlatformDataChanger<TType> : MonoBehaviour where TType : MonoBehaviour
{
	[SerializeField]
	protected TType entity;

	private bool flowNeedsMagic = true;

	protected virtual void OnValidate()
	{
		if (!Application.isPlaying)
		{
			CheckAndSetPlatform();
		}
	}

	private void OnEnable()
	{
		if (flowNeedsMagic)
		{
			CheckAndSetPlatform();
			flowNeedsMagic = false;
		}
	}

	private void CheckAndSetPlatform()
	{
		if (!entity)
		{
			entity = GetComponent<TType>();
		}
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

	protected abstract void SetNutaku();

	protected abstract void SetEpocha();

	protected abstract void SetSteam();

	protected abstract void SetHarem();

	protected abstract void SetErolabs();
}
