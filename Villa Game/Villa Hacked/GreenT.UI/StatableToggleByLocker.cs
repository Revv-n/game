using System;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Select<bool, int>((IObservable<bool>)_locker.IsOpen, (Func<bool, int>)((bool value) => value ? 1 : 0)), (Action<int>)_affectedStatable.Set), (Component)this);
	}
}
