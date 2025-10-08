using System;
using GreenT.HornyScapes.Constants;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class RatingUpdater : MonoBehaviour
{
	[SerializeField]
	private RatingView _ratingView;

	private const string AUTO_COOLDOWN = "leaderboard_autoupdate_cooldown";

	private const string ON_ENABLE_COOLDOWN = "leaderboard_onenable_cooldown";

	private IDisposable _autoCooldownDisposable;

	private IDisposable _onEnableCooldownDisposable;

	private IConstants<int> _intConstants;

	public bool IsInited { get; private set; }

	public bool IsOnEnableCooldown { get; private set; }

	[Inject]
	private void Init(IConstants<int> intConstants)
	{
		_intConstants = intConstants;
	}

	public void Enable(bool isActive)
	{
		base.gameObject.SetActive(isActive);
		if (isActive)
		{
			if (!IsInited)
			{
				IsInited = true;
			}
			if (!IsOnEnableCooldown)
			{
				_ratingView.AutoUpdate();
				StartOnEnableUpdateCooldown();
			}
			StartAutoUpdateCooldown();
		}
	}

	private void OnDisable()
	{
		_autoCooldownDisposable?.Dispose();
	}

	private void OnDestroy()
	{
		_onEnableCooldownDisposable?.Dispose();
		_autoCooldownDisposable?.Dispose();
	}

	private void StartOnEnableUpdateCooldown()
	{
		IsOnEnableCooldown = true;
		_onEnableCooldownDisposable = Observable.Timer(TimeSpan.FromSeconds(_intConstants["leaderboard_onenable_cooldown"])).Subscribe(delegate
		{
			IsOnEnableCooldown = false;
		});
	}

	private void StartAutoUpdateCooldown()
	{
		_autoCooldownDisposable?.Dispose();
		_autoCooldownDisposable = Observable.Timer(TimeSpan.FromSeconds(_intConstants["leaderboard_autoupdate_cooldown"])).Repeat().Subscribe(delegate
		{
			_ratingView.AutoUpdate();
		});
	}
}
