using GreenT.HornyScapes.Constants;
using StripClub.Model;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.UI;

public class StatableToggleByLocker : MonoBehaviour
{
	[SerializeField]
	private string _lockerID;

	[SerializeField]
	private StatableComponent _affectedStatable;

	private ILocker _locker;

	[Inject]
	public void Construct(LockerFactory lockerFactory, IConstants<ILocker> lockerConstants)
	{
		_locker = lockerConstants[_lockerID];
	}

	private void Start()
	{
		_locker.IsOpen.Select((bool value) => value ? 1 : 0).Subscribe(_affectedStatable.Set).AddTo(this);
	}
}
