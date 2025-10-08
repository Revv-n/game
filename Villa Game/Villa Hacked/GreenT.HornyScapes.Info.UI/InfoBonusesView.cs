using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.Info.UI;

public class InfoBonusesView : MonoBehaviour
{
	[SerializeField]
	private Transform _holder;

	public GameItemConfigView[] giConfigViews;

	public void Init(GIConfig config)
	{
		if (!config.TryGetModule<ModuleConfigs.ClickSpawn>(out var _))
		{
			_holder.gameObject.SetActive(value: false);
			return;
		}
		_holder.gameObject.SetActive(value: true);
		GameItemConfigView[] array = giConfigViews;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Set(config);
		}
	}
}
