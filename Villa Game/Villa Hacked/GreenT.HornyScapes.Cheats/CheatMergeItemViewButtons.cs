using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Cheats;

public class CheatMergeItemViewButtons : MonoBehaviour
{
	public Button Info;

	public Button Create;

	public CheatMergeItemView view;

	public InputSettingCheats inputSetting;

	private void Awake()
	{
		Info.onClick.AddListener(view.PrintInfo);
		Create.onClick.AddListener(AddItemToPocket);
	}

	private void AddItemToPocket()
	{
		if (inputSetting.IsWhilePressed(inputSetting.AndCreateItem))
		{
			view.AddItemToPocket();
		}
	}

	private void OnDestroy()
	{
		Info.onClick.RemoveAllListeners();
		Create.onClick.RemoveAllListeners();
	}
}
