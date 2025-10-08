using UnityEngine;

namespace GreenT.HornyScapes;

public abstract class SimpleStateMachine<TStateType, TState> : MonoBehaviour where TState : IState
{
	protected TState _currentState;

	public abstract void SetState(TStateType stateType);

	protected virtual void SetState(TState nextState)
	{
		if (_currentState != null)
		{
			_currentState.Exit();
		}
		_currentState = nextState;
		nextState.Enter();
	}
}
