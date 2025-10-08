using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.Cheats;

public class EnumDropdownSelector<T> : MonoBehaviour where T : Enum
{
	[SerializeField]
	private TMP_Dropdown _dropdown;

	public T SelectedValue { get; private set; }

	public event Action<T> OnValueChanged;

	private void Awake()
	{
		string[] names = Enum.GetNames(typeof(T));
		_dropdown.ClearOptions();
		_dropdown.AddOptions(names.ToList());
		_dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
		OnDropdownValueChanged(_dropdown.value);
	}

	private void OnDropdownValueChanged(int index)
	{
		T[] array = (T[])Enum.GetValues(typeof(T));
		SelectedValue = array[index];
		this.OnValueChanged?.Invoke(SelectedValue);
	}

	public void SetValue(T value)
	{
		int num = Array.IndexOf(Enum.GetValues(typeof(T)), value);
		if (num >= 0)
		{
			_dropdown.SetValueWithoutNotify(num);
			SelectedValue = value;
		}
	}
}
