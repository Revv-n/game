using System;
using System.Collections.Generic;
using GreenT.HornyScapes.MergeCore;
using StripClub.NewEvent.Model;

namespace StripClub.NewEvent.Data;

[Serializable]
public class EventDataCase
{
	private int id;

	private EventDataSaver eventDataSaver;

	private EventWallet eventWallet;

	private IReadOnlyList<IDisposableEventInformation> disposableInforation;

	private MergeField mergeField;

	private PocketRepository pocketRepository;

	public int ID => id;

	public EventDataSaver Saver => eventDataSaver;

	public EventWallet Wallet => eventWallet;

	public MergeField MergeField => mergeField;

	public PocketRepository PocketRepository => pocketRepository;

	public IReadOnlyList<IDisposableEventInformation> DisposableInforation => disposableInforation;

	public EventDataCase()
	{
	}

	public EventDataCase(EventDataCreateCase createCase)
	{
		SetData(createCase);
	}

	public void Initialization(EventDataCreateCase createCase)
	{
		SetData(createCase);
	}

	private void SetData(EventDataCreateCase createCase)
	{
		id = createCase.Id;
		disposableInforation = createCase.ClearingInforation;
		eventWallet = createCase.Wallet;
		eventDataSaver = createCase.Saver;
		mergeField = createCase.MergeField;
		pocketRepository = createCase.PocketRepository;
	}
}
