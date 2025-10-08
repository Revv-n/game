using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Types;
using StripClub.Model;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class RatingPlayerView : MonoView
{
	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private StatableComponent[] _statables;

	[SerializeField]
	private Transform[] _loadingViews;

	[SerializeField]
	private RectTransform _rectTransform;

	[SerializeField]
	private Transform _parentRoot;

	[SerializeField]
	private TMP_Text _place;

	[SerializeField]
	private TMP_Text _name;

	[SerializeField]
	private TMP_Text _score;

	[SerializeField]
	private List<Material> _materials;

	[SerializeField]
	private List<Color> _textColors;

	[SerializeField]
	private List<Color> _vfxColors;

	[SerializeField]
	private Image _medalVfx;

	[SerializeField]
	private ParticleSystem _particleSystem;

	[SerializeField]
	private CurrencyView _currencyView;

	[SerializeField]
	private Image _medal;

	[SerializeField]
	private List<Sprite> _medals;

	private bool _isLoading;

	private IDisposable _loadingViewDisposable;

	protected MiniEventTaskItemRewardViewManager _miniEventItemRewardViewManager;

	private IEnumerable<LinkedContent> _currentRewards;

	protected Vector3 BIG_SIZE = new Vector3(1.075f, 1.075f, 1.075f);

	protected Vector3 SMALL_SIZE = Vector3.one;

	protected const int LOOPS_AMOUNT = 1;

	protected const float DURATION = 0.5f;

	protected const string UPDATING_SCORES = "Updating";

	protected const string ERROR_KEY = "-";

	public RectTransform RectTransform => _rectTransform;

	public Transform ParentRoot => _parentRoot;

	public int Place { get; set; }

	public int Index { get; set; }

	[Inject]
	private void Init(MiniEventTaskItemRewardViewManager miniEventItemRewardViewManager)
	{
		_miniEventItemRewardViewManager = miniEventItemRewardViewManager;
	}

	public virtual void SetupRewardState()
	{
	}

	public void SetupName(string name = null)
	{
		if (string.IsNullOrEmpty(name))
		{
			_name.text = "-";
		}
		else
		{
			_name.text = name ?? "";
		}
	}

	public void SetupPlace(int place = 0)
	{
		bool flag = place >= 0 && place < _medals.Count;
		_medal.gameObject.SetActive(flag);
		_medalVfx.gameObject.SetActive(flag);
		if (flag)
		{
			_medal.sprite = _medals[place];
			_medalVfx.color = _vfxColors[place];
			_place.color = _textColors[place + 1];
			_place.fontSharedMaterial = _materials[place + 1];
		}
		else
		{
			_place.fontSharedMaterial = _materials[0];
			_place.color = _textColors[0];
		}
		if (place == 0)
		{
			((Component)(object)_particleSystem).gameObject.SetActive(value: true);
			_particleSystem.Play();
		}
		else
		{
			_particleSystem.Pause();
			((Component)(object)_particleSystem).gameObject.SetActive(value: false);
		}
		if (place == -1)
		{
			_place.text = "-";
			_place.fontSharedMaterial = _materials[0];
			_place.color = _textColors[0];
			Index = -1;
			Place = -1;
		}
		else
		{
			Index = place;
			Place = ++place;
			_place.text = $"{Place}";
		}
	}

	public void SetupScore(int score = 0)
	{
		if (score == 0)
		{
			_score.text = "-";
		}
		else
		{
			_score.text = $"{score}";
		}
	}

	public void SetupRewards(IEnumerable<LinkedContent> rewards)
	{
		if (_currentRewards != null && _currentRewards == rewards)
		{
			return;
		}
		_currentRewards = rewards;
		_miniEventItemRewardViewManager.HideAll();
		if (rewards == null || !rewards.Any())
		{
			return;
		}
		foreach (LinkedContent reward in rewards)
		{
			_miniEventItemRewardViewManager.Display(reward);
		}
	}

	public void SetupCurrencyIcon(CurrencyType currencyType, CompositeIdentificator currencyIdentificator, int id = 0)
	{
		_currencyView.Setup(currencyType, currencyIdentificator, isTrackable: false, id);
	}

	public void ShowLoadingState()
	{
		_isLoading = true;
		StartLoadingAnimation();
		StatableComponent[] statables = _statables;
		for (int i = 0; i < statables.Length; i++)
		{
			statables[i].Set(1);
		}
		_score.text = "Updating";
	}

	public void StopLoadingState()
	{
		_isLoading = false;
		StatableComponent[] statables = _statables;
		for (int i = 0; i < statables.Length; i++)
		{
			statables[i].Set(0);
		}
	}

	public void SwapVisibleState(bool isActive)
	{
		if (isActive)
		{
			_canvasGroup.alpha = 1f;
		}
		else
		{
			_canvasGroup.alpha = 0f;
		}
	}

	public void PlayBigAnimation()
	{
		base.transform.DOKill();
		base.transform.DOScale(BIG_SIZE, 0.5f);
	}

	public void PlaySmallAnimation()
	{
		base.transform.DOKill();
		base.transform.DOScale(SMALL_SIZE, 0.5f);
	}

	private void StartLoadingAnimation()
	{
		_loadingViewDisposable?.Dispose();
		_loadingViewDisposable = ObservableExtensions.Subscribe<long>(Observable.EveryFixedUpdate(), (Action<long>)delegate
		{
			Transform[] loadingViews = _loadingViews;
			foreach (Transform transform in loadingViews)
			{
				if (!_isLoading)
				{
					_loadingViewDisposable?.Dispose();
				}
				else
				{
					transform.Rotate(Vector3.back * 5f, Space.Self);
				}
			}
		});
	}
}
