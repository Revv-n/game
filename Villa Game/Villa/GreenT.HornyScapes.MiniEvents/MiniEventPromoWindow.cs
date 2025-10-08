using System;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Events;
using StripClub.Extensions;
using StripClub.UI;
using StripClub.UI.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventPromoWindow : MonoView<MiniEvent>
{
	[SerializeField]
	private MonoTimer _timer;

	[SerializeField]
	private Button _startButton;

	[SerializeField]
	private Button _skipButton;

	[SerializeField]
	private LocalizedTextMeshPro _title;

	[SerializeField]
	private LocalizedTextMeshPro _titleShadow;

	[SerializeField]
	private LocalizedTextMeshPro _description;

	[SerializeField]
	private PromoContentView _promoContentView;

	private GenericTimer _genericTimer;

	private TimeHelper _timeHelper;

	private PopupWindow _rootPopupWindow;

	private Action<MiniEvent> _onStart;

	private const string TITLE_KEY = "ui.minievent.eventtitle_promo{0}.name";

	private const string DESCRIPTION_KEY = "ui.minievent.eventtitle_promo{0}.descr";

	private void OnDestroy()
	{
		_skipButton.onClick.RemoveListener(OnSkipButtonClick);
		_startButton.onClick.RemoveListener(OnStartButtonClick);
	}

	public void Init(Action<MiniEvent> onStart, GenericTimer genericTimer, TimeHelper timeHelper, PopupWindow root)
	{
		_rootPopupWindow = root;
		_onStart = onStart;
		_genericTimer = genericTimer;
		_timeHelper = timeHelper;
	}

	public override void Set(MiniEvent source)
	{
		base.Set(source);
		SetupText(source.EventId);
		SetupContent(source);
		ApplyTimer();
		_skipButton.onClick.AddListener(OnSkipButtonClick);
		_startButton.onClick.AddListener(OnStartButtonClick);
	}

	private void SetupText(int eventId)
	{
		string key = $"ui.minievent.eventtitle_promo{eventId}.name";
		_title.Init(key);
		_titleShadow.Init(key);
		string key2 = $"ui.minievent.eventtitle_promo{eventId}.descr";
		_description.Init(key2);
	}

	private void SetupContent(MiniEvent source)
	{
		_promoContentView.Set(source.Promo.PromoContent);
	}

	private void ApplyTimer()
	{
		_timer.Init(_genericTimer, _timeHelper.UseCombineFormat);
	}

	private void OnStartButtonClick()
	{
		OnSkipButtonClick();
		base.Source.StartWindowShown = true;
		_onStart(base.Source);
	}

	private void OnSkipButtonClick()
	{
		Display(display: false);
		_rootPopupWindow.Close();
	}
}
