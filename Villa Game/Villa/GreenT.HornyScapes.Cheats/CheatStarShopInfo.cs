using System;
using GreenT.HornyScapes.StarShop;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatStarShopInfo : MonoBehaviour
{
	private StarShopManager _starShopManager;

	[SerializeField]
	private TMP_Text _info;

	[SerializeField]
	private TMP_Text _lastUpdateInfo;

	private IDisposable _subscription;

	[Inject]
	private void Constructor(StarShopManager starShopManager)
	{
		_starShopManager = starShopManager;
	}

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		_subscription = _starShopManager.OnUpdate.Subscribe(UpdateText);
		UpdateText();
	}

	private void UpdateText(StarShopItem x = null)
	{
		int completeMaxId = _starShopManager.GetCompleteMaxId();
		int lastId = _starShopManager.GetLastId();
		_info.text = $"Rewarded starshop:{completeMaxId}";
		_lastUpdateInfo.text = $"Starshop to complete:{lastId}";
	}

	private void OnDisable()
	{
		_subscription?.Dispose();
	}
}
