using System;
using System.Collections.Generic;
using GreenT.HornyScapes.UI;
using Merge.Core.Masters;
using UniRx;
using UnityEngine;
using Zenject;

namespace Merge;

public class InputController : Controller<InputController>, IMasterController, IDisposable
{
	private enum PointerState
	{
		None,
		Up,
		Down,
		InitDrag,
		Drag,
		EndDrag
	}

	[SerializeField]
	private float initDragDistance = 0.1f;

	[SerializeField]
	private Camera gameCamera;

	private List<IClickController> _clickListeners = new List<IClickController>();

	private List<IPointerController> _pointerListeners = new List<IPointerController>();

	private List<IDragController> _dragListeners = new List<IDragController>();

	private Vector3 _mouseDownPosition;

	private Vector3 _prevPosition;

	private bool _isDragging;

	private bool _isMouseDown;

	private bool _isBlocked;

	private CompositeDisposable _disposables = new CompositeDisposable();

	private IDisposable _activityStream;

	private UserInputDetector _inputDetector;

	private MergeScreenIndicator _mergeScreenIndicator;

	private EventMergeScreenIndicator _eventMergeScreenIndicator;

	[Inject]
	public void Init(UserInputDetector inputDetector, MergeScreenIndicator mergeScreenIndicator, EventMergeScreenIndicator eventMergeScreenIndicator)
	{
		_inputDetector = inputDetector;
		_mergeScreenIndicator = mergeScreenIndicator;
		_eventMergeScreenIndicator = eventMergeScreenIndicator;
	}

	public override void Init()
	{
		_activityStream = (from _ in _mergeScreenIndicator.IsVisible.Merge(_eventMergeScreenIndicator.IsVisible)
			select _eventMergeScreenIndicator.IsVisible.Value || _mergeScreenIndicator.IsVisible.Value).Subscribe(SetInputActivity);
	}

	private void OnValidate()
	{
		if (!gameCamera)
		{
			Debug.LogError("InputController doesn't have a camera");
		}
	}

	public void LinkControllers(IList<BaseController> controllers)
	{
		foreach (BaseController controller in controllers)
		{
			if (controller is IClickController)
			{
				_clickListeners.Add(controller as IClickController);
			}
			if (controller is IPointerController)
			{
				_pointerListeners.Add(controller as IPointerController);
			}
			if (controller is IDragController)
			{
				_dragListeners.Add(controller as IDragController);
			}
		}
	}

	private PointerState UpdateMouse()
	{
		Vector2 vector = gameCamera.ScreenToWorldPoint(Input.mousePosition);
		if (Input.GetMouseButtonUp(0))
		{
			return AtMouseUp(vector);
		}
		if (Input.GetMouseButtonDown(0))
		{
			return AtMouseDown(vector);
		}
		if (!_isMouseDown)
		{
			return PointerState.None;
		}
		if (_isDragging)
		{
			return AtDrag(vector);
		}
		if (IsDragInited(vector))
		{
			return AtDragInited(vector);
		}
		return PointerState.None;
	}

	private PointerState UpdateTouch()
	{
		Vector3 position = gameCamera.ScreenToWorldPoint(Input.mousePosition);
		position.z = 0f;
		if (Input.touchCount == 0)
		{
			if (_isMouseDown || _isDragging)
			{
				return AtMouseUp(position);
			}
			return PointerState.None;
		}
		Touch touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Ended)
		{
			return AtMouseUp(position);
		}
		if (touch.phase == TouchPhase.Began)
		{
			return AtMouseDown(position);
		}
		if (!_isMouseDown)
		{
			return PointerState.None;
		}
		if (_isDragging)
		{
			return AtDrag(position);
		}
		if (touch.phase == TouchPhase.Moved)
		{
			return AtDragInited(position);
		}
		return PointerState.None;
	}

	private PointerState AtMouseDown(Vector3 position)
	{
		_prevPosition = position;
		_mouseDownPosition = position;
		_isMouseDown = true;
		_pointerListeners.ForEach(delegate(IPointerController x)
		{
			x.AtMouseDown(position);
		});
		return PointerState.Down;
	}

	private PointerState AtMouseUp(Vector3 position)
	{
		_isMouseDown = false;
		_pointerListeners.ForEach(delegate(IPointerController x)
		{
			x.AtMouseUp(position);
		});
		if (!_isDragging)
		{
			_clickListeners.ForEach(delegate(IClickController x)
			{
				x.AtClick(position, _mouseDownPosition);
			});
			return PointerState.Up;
		}
		_isDragging = false;
		foreach (IDragController dragListener in _dragListeners)
		{
			dragListener.AtEndDrag(position);
		}
		return PointerState.EndDrag;
	}

	private PointerState AtDragInited(Vector3 position)
	{
		_isDragging = true;
		_dragListeners.ForEach(delegate(IDragController x)
		{
			x.AtStartDrag(position, _mouseDownPosition);
		});
		return PointerState.InitDrag;
	}

	private PointerState AtDrag(Vector3 position)
	{
		Vector3 delta = position - _prevPosition;
		_dragListeners.ForEach(delegate(IDragController x)
		{
			x.AtDrag(position, delta);
		});
		_prevPosition = position;
		return PointerState.Drag;
	}

	private bool IsDragInited(Vector3 position)
	{
		return (position - _mouseDownPosition).sqrMagnitude > initDragDistance;
	}

	public void SetBlockedByTutorial(bool blocked)
	{
		if (_isBlocked != blocked)
		{
			_isBlocked = blocked;
			_disposables.Clear();
			if (!_isBlocked)
			{
				ListenUserInput();
			}
		}
	}

	public void ListenUserInput()
	{
		_inputDetector.OnPointerDown().Subscribe(delegate(UserInputDetector _input)
		{
			Vector2 worldPosition = gameCamera.ScreenToWorldPoint(_input.PointerScreenPosition);
			_pointerListeners.ForEach(delegate(IPointerController x)
			{
				x.AtMouseDown(worldPosition);
			});
		}).AddTo(_disposables);
		_inputDetector.OnPointerUp().Subscribe(delegate(UserInputDetector _input)
		{
			Vector2 worldPosition = gameCamera.ScreenToWorldPoint(_input.PointerScreenPosition);
			_pointerListeners.ForEach(delegate(IPointerController x)
			{
				x.AtMouseUp(worldPosition);
			});
		}).AddTo(_disposables);
		_inputDetector.OnClick().Subscribe(delegate(UserInputDetector _input)
		{
			Vector2 worldPosition = gameCamera.ScreenToWorldPoint(_input.PointerScreenPosition);
			Vector2 downPosition = gameCamera.ScreenToWorldPoint(_input.PointerDownScreenPosition);
			_clickListeners.ForEach(delegate(IClickController x)
			{
				x.AtClick(worldPosition, downPosition);
			});
		}).AddTo(_disposables);
		_inputDetector.OnDragBegin().Subscribe(delegate(UserInputDetector _input)
		{
			Vector2 worldPosition = gameCamera.ScreenToWorldPoint(_input.PointerScreenPosition);
			Vector2 downPosition = gameCamera.ScreenToWorldPoint(_input.PointerDownScreenPosition);
			_dragListeners.ForEach(delegate(IDragController x)
			{
				x.AtStartDrag(worldPosition, downPosition);
			});
		}).AddTo(_disposables);
		_inputDetector.OnDragging().Subscribe(delegate(UserInputDetector _input)
		{
			Vector2 worldPosition = gameCamera.ScreenToWorldPoint(_input.PointerScreenPosition);
			_dragListeners.ForEach(delegate(IDragController x)
			{
				x.AtDrag(worldPosition, _input.PointerShift);
			});
		}).AddTo(_disposables);
		_inputDetector.OnDragEnd().Subscribe(delegate(UserInputDetector _input)
		{
			Vector3 worldPosition = gameCamera.ScreenToWorldPoint(_input.PointerScreenPosition);
			_dragListeners.ForEach(delegate(IDragController x)
			{
				x.AtEndDrag(worldPosition);
			});
		}).AddTo(_disposables);
	}

	protected override void OnDestroy()
	{
		Dispose();
	}

	private void SetInputActivity(bool state)
	{
		if (!_isBlocked)
		{
			if (state)
			{
				ListenUserInput();
			}
			else
			{
				_disposables.Clear();
			}
		}
	}

	public void Dispose()
	{
		base.OnDestroy();
		_disposables?.Dispose();
		_activityStream.Dispose();
	}
}
