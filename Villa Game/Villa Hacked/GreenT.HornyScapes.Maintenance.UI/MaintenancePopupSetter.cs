using System;
using GreenT.UI;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Maintenance.UI;

public class MaintenancePopupSetter : MonoBehaviour
{
	private const string NonReasonKey = "ui.maintenance.description.non_reason";

	private const string UpdateConfigKey = "ui.maintenance.description.update_config";

	private const string UpdateClientKey = "ui.maintenance.description.update_client";

	private const string UpdateSteamClientKey = "ui.maintenance.description.steam_update_client";

	private const string MaintenanceTimeKey = "ui.maintenance.description.maintenance_time";

	public Window Window;

	public Button ReloadButton;

	public LocalizedTextMeshPro Description;

	public TextMeshProUGUI _textExitButton;

	private DisplayMaintenancePopup _displayMaintenancePopup;

	private VersionProvider _versionProvider;

	[Inject]
	private void Constructor(DisplayMaintenancePopup displayMaintenancePopup, VersionProvider versionProvider)
	{
		_displayMaintenancePopup = displayMaintenancePopup;
		_versionProvider = versionProvider;
	}

	private void Awake()
	{
		ReloadButton.onClick.AddListener(Exit);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<MaintenanceInfo>(Observable.Where<MaintenanceInfo>((IObservable<MaintenanceInfo>)_displayMaintenancePopup.Reason, (Func<MaintenanceInfo, bool>)((MaintenanceInfo x) => x != null)), (Action<MaintenanceInfo>)Set), (Component)this);
		_textExitButton.text = "EXIT";
	}

	private void Set(MaintenanceInfo info)
	{
		string key = SelectDescription(info);
		Description.Init(key);
	}

	private void Exit()
	{
		PlayerWantsToQuit.AllowQuitting = true;
		Application.Quit();
	}

	private string SelectDescription(MaintenanceInfo info)
	{
		if (info.NeedUpdateClient)
		{
			return "ui.maintenance.description.steam_update_client";
		}
		if (info.ConfigIsOld)
		{
			return "ui.maintenance.description.update_config";
		}
		if (info.MaintenanceTime)
		{
			return "ui.maintenance.description.maintenance_time";
		}
		return "ui.maintenance.description.non_reason";
	}

	private void OnDestroy()
	{
		ReloadButton.onClick.RemoveListener(Exit);
	}
}
