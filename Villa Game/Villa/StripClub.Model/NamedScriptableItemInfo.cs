using StripClub.Utility;
using UnityEngine;

namespace StripClub.Model;

public abstract class NamedScriptableItemInfo : ScriptableItemInfo
{
	[SerializeField]
	protected string key = string.Empty;

	[ReadOnly]
	[SerializeField]
	private string localizationKey;

	public override string LocalizationKey => localizationKey;

	public override void OnValidate()
	{
		base.OnValidate();
		localizationKey = GetKey();
	}

	protected abstract string GetKey();
}
