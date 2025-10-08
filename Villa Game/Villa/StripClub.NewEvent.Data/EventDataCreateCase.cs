using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MergeCore;
using StripClub.NewEvent.Model;
using UniRx;

namespace StripClub.NewEvent.Data;

public class EventDataCreateCase
{
	public Event Event;

	public EventDataSaver Saver;

	public EventWallet Wallet;

	public MergeField MergeField;

	public readonly List<IDisposableEventInformation> ClearingInforation = new List<IDisposableEventInformation>(9);

	public PocketRepository PocketRepository;

	public int Id => Event.EventId;

	public IReadOnlyReactiveProperty<string> SaveKey => Event.GetSaveKey();
}
