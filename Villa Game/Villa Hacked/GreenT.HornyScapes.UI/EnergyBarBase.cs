using StripClub.Extensions;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.UI;

public abstract class EnergyBarBase : MonoBehaviour
{
	[SerializeField]
	protected Button _plus;

	[SerializeField]
	protected TMProTimer _timerView;

	protected RestorableValue<int> _energy;

	protected TimeHelper _timeHelper;

	[Inject]
	public virtual void Init(TimeHelper timeHelper)
	{
		_timeHelper = timeHelper;
	}

	protected virtual void Awake()
	{
		_plus.onClick.AddListener(ShowRestoreEnergyPopup);
		_timerView.Init(_energy.Timer, _timeHelper.UseCombineFormat);
	}

	protected abstract void ShowRestoreEnergyPopup();

	protected virtual void OnDestroy()
	{
		_plus.onClick.RemoveListener(ShowRestoreEnergyPopup);
	}
}
