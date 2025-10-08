using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

public class IsUserMigrated : MonoBehaviour
{
	public Button SelfMigrateButton;

	private void OnEnable()
	{
		SelfMigrateButton.onClick.AddListener(RequestSystemAlertWindowPermission);
	}

	private void OnDisable()
	{
		SelfMigrateButton.onClick.RemoveListener(RequestSystemAlertWindowPermission);
	}

	public void RequestSystemAlertWindowPermission()
	{
	}
}
