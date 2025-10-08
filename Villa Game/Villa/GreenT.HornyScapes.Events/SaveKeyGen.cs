using System.Text;
using StripClub.Model.Cards;

namespace GreenT.HornyScapes.Events;

public static class SaveKeyGen
{
	public static string OldEventSaveKey(this Event target)
	{
		return new StringBuilder("Event_").Append(target.EventId).ToString();
	}

	public static string EventSaveKey(this Event target)
	{
		_ = $"CalendarID_{target.CalendarId}.eventID_{target.EventId}";
		return new StringBuilder("CalendarID_").Append(target.CalendarId).Append(".eventID_").Append(target.EventId)
			.ToString();
	}

	public static string CharacterSaveKey(this ICard card)
	{
		return new StringBuilder("character_").Append(card.ID).ToString();
	}

	public static bool IsOldEventKey(this Event target)
	{
		return target.UniqueKey().Equals(target.OldEventSaveKey());
	}
}
