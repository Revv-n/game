using StripClub.Test;
using UnityEngine;

namespace StripClub.Registration;

public class RegistrationLootBoxOpener : CheatLootboxOpener
{
	[SerializeField]
	private int _lootBoxId;

	private void Awake()
	{
		OnEnterValue(_lootBoxId.ToString());
		openButton.onClick.AddListener(delegate
		{
			OpenLootbox(lootbox);
		});
	}

	private void OnDestroy()
	{
		openButton.onClick.RemoveAllListeners();
	}
}
