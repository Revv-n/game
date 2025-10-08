using System;
using System.Linq;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

public sealed class RatingSelfView : RatingPlayerView
{
	[SerializeField]
	private StatableComponent _rewardStatable;

	[SerializeField]
	private Button _claimRewardButton;

	[SerializeField]
	private GameObject _claimRewardButtonText;

	[SerializeField]
	private GameObject _rewardClaimedButtonText;

	[SerializeField]
	private GameObject _glowVFX;

	private Action _onRewardButtonClick;

	private void OnDestroy()
	{
		_claimRewardButton.onClick.RemoveAllListeners();
	}

	public void Init(Action onRewardButtonClick)
	{
		_onRewardButtonClick = onRewardButtonClick;
		_claimRewardButton.onClick.AddListener(delegate
		{
			OnClaimRewardButtonClick();
		});
	}

	public void SetupDefaultState()
	{
		_rewardStatable.Set(0);
	}

	public override void SetupRewardState()
	{
		if (_miniEventItemRewardViewManager.VisibleViews.Any())
		{
			_claimRewardButtonText.SetActive(value: true);
			_glowVFX.SetActive(value: true);
			_rewardClaimedButtonText.SetActive(value: false);
			_rewardStatable.Set(1);
		}
	}

	private void OnClaimRewardButtonClick()
	{
		_claimRewardButtonText.SetActive(value: false);
		_glowVFX.SetActive(value: false);
		_rewardClaimedButtonText.SetActive(value: true);
		_claimRewardButton.interactable = false;
		_onRewardButtonClick();
	}
}
