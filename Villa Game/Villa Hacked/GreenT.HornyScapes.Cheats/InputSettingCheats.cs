using UnityEngine;

namespace GreenT.HornyScapes.Cheats;

[CreateAssetMenu(fileName = "InputSettingCheats", menuName = "GreenT/Cheats/Settings/InputSettings")]
public class InputSettingCheats : ScriptableObject
{
	public KeyCode OpenConsole = KeyCode.Tab;

	public CompositeKeys NextLine = new CompositeKeys(KeyCode.LeftControl, KeyCode.Alpha1);

	public CompositeKeys CopyMergeItemId = new CompositeKeys(KeyCode.LeftControl, KeyCode.Q);

	public KeyCode AddItemsForCurrentTasks = KeyCode.T;

	public KeyCode ShowBankTabID = KeyCode.LeftAlt;

	public KeyCode Roulette = KeyCode.LeftShift;

	[Header("Delete items in pocket")]
	public CompositeKeys DeleteAllItemsInPocket = new CompositeKeys(KeyCode.LeftControl, KeyCode.R);

	[Header("Delete merge items")]
	public CompositeKeys ClearCurrentField = new CompositeKeys(KeyCode.LeftControl, KeyCode.D);

	public KeyCode DeleteSelectedItem = KeyCode.D;

	[Header("Spawn items")]
	public CompositeKeys DischargeAnySpawner = new CompositeKeys(KeyCode.LeftControl, KeyCode.F);

	public KeyCode DischargeSelectedSpawner = KeyCode.F;

	[Header("Merge items")]
	public CompositeKeys MergeAllAvailableItems = new CompositeKeys(KeyCode.LeftControl, KeyCode.A);

	public KeyCode FindAndMergeAvailableItem = KeyCode.A;

	[Header("Create Item in Panel")]
	public KeyCode AndCreateItem = KeyCode.LeftShift;

	[Header("And Use LeftMouseButton")]
	public KeyCode CopyTaskId = KeyCode.LeftControl;

	public bool IsPressed(KeyCode key)
	{
		return Input.GetKeyDown(key);
	}

	public bool IsWhilePressed(KeyCode key)
	{
		return Input.GetKey(key);
	}
}
