using System.Linq;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using StripClub.Extensions;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventsPromoPusherView : PopupWindow
{
	[SerializeField]
	private Transform _promoRoot;

	[SerializeField]
	private MiniEventWindowView _miniEventWindowView;

	private TimeHelper _timeHelper;

	private MiniEventsBundlesProvider _bundlesProvider;

	private CalendarQueue _calendarQueue;

	private DiContainer _container;

	[Inject]
	private void Init(MiniEventsBundlesProvider bundlesProvider, CalendarQueue calendarQueue, TimeHelper timeHelper, DiContainer container)
	{
		_bundlesProvider = bundlesProvider;
		_calendarQueue = calendarQueue;
		_timeHelper = timeHelper;
		_container = container;
	}

	public void TryShowPromos(MiniEvent minievent)
	{
		if (_promoRoot.childCount > 0)
		{
			for (int i = 0; i < _promoRoot.childCount; i++)
			{
				Object.Destroy(_promoRoot.GetChild(i).gameObject);
			}
		}
		SpawnAndShowPromo(minievent);
		Open();
	}

	private void SpawnAndShowPromo(MiniEvent minievent)
	{
		if (minievent == null)
		{
			return;
		}
		GameObject gameObject = _bundlesProvider.TryGet(minievent.EventId).LoadAsset<GameObject>(minievent.Promo.PromoView);
		if (!(gameObject != null))
		{
			return;
		}
		MiniEventPromoWindow component = _container.InstantiatePrefab(gameObject, _promoRoot).GetComponent<MiniEventPromoWindow>();
		if (component != null)
		{
			CalendarModel calendarModel = _calendarQueue.FirstOrDefault((CalendarModel calendar) => calendar.BalanceId == minievent.EventId && calendar.EventType == EventStructureType.Mini);
			component.Init(OnStartMinieventButtonClick, calendarModel.Duration, _timeHelper, this);
			component.Set(minievent);
		}
	}

	private void OnStartMinieventButtonClick(MiniEvent miniEvent)
	{
		_miniEventWindowView.Open();
		_miniEventWindowView.InteractMiniEventView(miniEvent.Identificator, miniEvent.EventId, miniEvent.IsMultiTabbed, miniEvent.CurrencyIdentificator, miniEvent.ConfigType);
		miniEvent.WasFirstTimeSeen.Value = true;
	}
}
