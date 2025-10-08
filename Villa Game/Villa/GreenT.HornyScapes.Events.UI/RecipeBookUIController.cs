using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events.UI;

public class RecipeBookUIController : Controller<RecipeBookUIController>, IDisposable
{
	[SerializeField]
	private RecipeBookView _recipeBookView;

	[SerializeField]
	private RecipeBookButtonView _recipeBookButton;

	[SerializeField]
	private Window _recipeBookWindow;

	private IDisposable _eventChangeStream;

	private CalendarQueue _calendarQueue;

	private EventSettingsProvider _eventProvider;

	[Inject]
	private void InnerInit(CalendarQueue calendarQueue, EventSettingsProvider eventProvider)
	{
		_calendarQueue = calendarQueue;
		_eventProvider = eventProvider;
	}

	public override void Init()
	{
		_eventChangeStream = (from calendarModel in _calendarQueue.OnCalendarActiveNotNull()
			where calendarModel.EventType == EventStructureType.Event
			select calendarModel).Subscribe(SetRecipeBook);
	}

	protected override void OnDestroy()
	{
		Dispose();
		base.OnDestroy();
	}

	private void SetRecipeBook(CalendarModel calendarModel)
	{
		Event @event = _eventProvider.GetEvent(calendarModel.BalanceId);
		if (!@event.HasRecipeBook)
		{
			_recipeBookButton.Display(display: false);
			_recipeBookWindow.Close();
			return;
		}
		ViewSettings viewSettings = @event.ViewSettings;
		_recipeBookView.Set(viewSettings.RecipeBook);
		_recipeBookButton.Set(viewSettings.RecipeBookButton);
		_recipeBookButton.Display(display: true);
	}

	public void Dispose()
	{
		_eventChangeStream.Dispose();
	}
}
