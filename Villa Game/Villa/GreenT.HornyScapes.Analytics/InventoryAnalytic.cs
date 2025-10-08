using GreenT.HornyScapes.MergeCore.Inventory;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics;

public class InventoryAnalytic : AnalyticMonoBehaviour
{
	private const string ANALYTIC_EVENT = "inventory_expand";

	[SerializeField]
	protected MergeInventoryWindow window;

	private void Start()
	{
		Track();
	}

	private void Track()
	{
		window.OnSlotBuy += SendEvent;
	}

	private void SendEvent()
	{
		int slot = window.Data.OpenedSlots - 1;
		window.Config.GetSlotUnlockPrice(slot);
		int openedSlots = window.Data.OpenedSlots;
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("inventory_expand");
		amplitudeEvent.AddEventParams("inventory_expand", openedSlots);
		amplitude.AddEvent(amplitudeEvent);
	}

	private void OnDestroy()
	{
		window.OnSlotBuy -= SendEvent;
	}
}
