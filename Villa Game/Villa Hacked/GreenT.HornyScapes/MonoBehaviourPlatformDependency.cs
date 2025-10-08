using UnityEngine;

namespace GreenT.HornyScapes;

public class MonoBehaviourPlatformDependency : PlatformDataChanger<MonoBehaviour>
{
	public GameObject[] entitiesShowEpocha;

	public GameObject[] entitiesShowNutaku;

	public GameObject[] entitiesShowSteam;

	public GameObject[] entitiesShowHarem;

	public GameObject[] entitiesShowErolabs;

	public SettingsWindowSizeSetter settingsWindowSizeSetter;

	protected override void SetNutaku()
	{
		GameObject[] array = entitiesShowEpocha;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowSteam;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowHarem;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowErolabs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowNutaku;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		settingsWindowSizeSetter.SetDefaultSize();
	}

	protected override void SetEpocha()
	{
		GameObject[] array = entitiesShowNutaku;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowSteam;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowHarem;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowErolabs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowEpocha;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		settingsWindowSizeSetter.SetDefaultSize();
	}

	protected override void SetSteam()
	{
		GameObject[] array = entitiesShowNutaku;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowEpocha;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowHarem;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowErolabs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowSteam;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		settingsWindowSizeSetter.SetDefaultSize();
	}

	protected override void SetHarem()
	{
		GameObject[] array = entitiesShowNutaku;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowEpocha;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowSteam;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowErolabs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowHarem;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		settingsWindowSizeSetter.SetDefaultSize();
	}

	protected override void SetErolabs()
	{
		GameObject[] array = entitiesShowNutaku;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowEpocha;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowSteam;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowHarem;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = entitiesShowErolabs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		settingsWindowSizeSetter.SetBigSize();
	}
}
