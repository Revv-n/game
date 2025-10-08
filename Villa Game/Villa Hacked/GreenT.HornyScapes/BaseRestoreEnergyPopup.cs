using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Constants;
using TMPro;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public abstract class BaseRestoreEnergyPopup : PopupWindow
{
	[SerializeField]
	private TMP_Text _energyText;

	private int _energyAmount;

	protected abstract string AmountKey { get; }

	[Inject]
	private void SetConstantsInfo(IConstants<int> constants)
	{
		_energyAmount = constants[AmountKey];
	}

	protected override void Awake()
	{
		base.Awake();
		_energyText.text = _energyAmount.ToString();
	}
}
