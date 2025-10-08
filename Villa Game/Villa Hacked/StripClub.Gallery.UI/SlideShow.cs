using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Gallery.UI;

public class SlideShow : MonoBehaviour
{
	[Serializable]
	public enum State
	{
		Freeze,
		Slow,
		Fast
	}

	[Serializable]
	public class StateSettingsDictionary : SerializableDictionary<State, Settings>
	{
	}

	[Serializable]
	public struct Settings
	{
		[field: SerializeField]
		public Sprite Icon { get; private set; }

		[field: SerializeField]
		public float DelayTime { get; private set; }
	}

	[SerializeField]
	private Image slideShowIcon;

	[SerializeField]
	private StateSettingsDictionary statesDict = new StateSettingsDictionary();

	private State currentState;

	private int statesCount = Enum.GetValues(typeof(State)).Length;

	private IDisposable slideShow;

	public event Action OnSwitch;

	public event Action<float, float> Progress;

	public void Switch()
	{
		currentState++;
		if ((int)currentState >= statesCount)
		{
			currentState = State.Freeze;
		}
		slideShow?.Dispose();
		float delayTime = statesDict[currentState].DelayTime;
		slideShowIcon.sprite = statesDict[currentState].Icon;
		switch (currentState)
		{
		case State.Slow:
		case State.Fast:
			slideShow = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<float>(PeriodicalEmission(delayTime), (Action<float>)delegate
			{
				this.OnSwitch?.Invoke();
			}), (Component)this);
			break;
		case State.Freeze:
			this.Progress?.Invoke(0f, 1f);
			break;
		default:
			throw new ArgumentOutOfRangeException("No behaviour for state " + currentState);
		}
	}

	private IObservable<float> PeriodicalEmission(float delayTime)
	{
		return Observable.TakeUntilDisable<float>(Observable.Where<float>(Observable.Skip<float>(Observable.Do<float>(Observable.Select<long, float>(Observable.Timer(TimeSpan.FromTicks(0L), TimeSpan.FromSeconds(1.0)), (Func<long, float>)((long timePassed) => (float)timePassed % delayTime)), (Action<float>)delegate(float cycleTime)
		{
			this.Progress?.Invoke(cycleTime, delayTime);
		}), 1), (Func<float, bool>)((float cycleTime) => (int)cycleTime == 0)), (Component)this);
	}
}
