using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

public sealed class ScrollPool : MonoView
{
	[SerializeField]
	private int _spacing;

	[SerializeField]
	private int _topPadding;

	[SerializeField]
	private int _bottomPadding;

	[SerializeField]
	private int _minTotalAmount = 5;

	[SerializeField]
	private RatingPlayerView[] _views;

	[SerializeField]
	private RatingPlayerView _playerView;

	[SerializeField]
	private RectTransform _content;

	[SerializeField]
	private ScrollRect _scrollRect;

	[SerializeField]
	private Transform _topSnap;

	[SerializeField]
	private Transform _bottomSnap;

	[SerializeField]
	private RatingEmptyView _topSnapPoint;

	[SerializeField]
	private RatingEmptyView _bottomSnapPoint;

	[SerializeField]
	private float _verticalNormalizedPosition = 1f;

	private RatingPlayerView _currentPlayerParent;

	private int _realPlayerIndex;

	private int _playerIndex;

	private int _oldIndex = -1;

	private int _totalAmount;

	private float _currentContentPosition;

	private bool _isInRange;

	private IDisposable _playerViewTrackDisposable;

	private IDisposable _disposable;

	private CompositeDisposable _compositeDisposable = new CompositeDisposable();

	private const int ITEM_HEIGHT = 120;

	private const int ZERO = 0;

	private const int ONE = 1;

	private const float FLOAT_ONE = 1f;

	private Subject<RatingPlayerView> _onOpponentViewPositionUpdate = new Subject<RatingPlayerView>();

	public IObservable<RatingPlayerView> OnOpponentViewPositionUpdate => (IObservable<RatingPlayerView>)_onOpponentViewPositionUpdate;

	private void Update()
	{
		if (_scrollRect.verticalNormalizedPosition >= _verticalNormalizedPosition)
		{
			TryMoveToTop(0);
		}
		ValidateInRange();
		if (_totalAmount <= _minTotalAmount || _playerIndex == -1)
		{
			return;
		}
		FastScrollValidate();
		if (_playerIndex == 6 && _scrollRect.verticalNormalizedPosition == 1f)
		{
			if (_currentPlayerParent != null)
			{
				_currentPlayerParent.SwapVisibleState(isActive: true);
			}
			_playerViewTrackDisposable?.Dispose();
			_playerView.transform.parent = _bottomSnap;
			_playerView.transform.localPosition = Vector3.zero;
			_playerView.PlayBigAnimation();
		}
	}

	private void OnDestroy()
	{
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Clear();
		}
		CompositeDisposable compositeDisposable2 = _compositeDisposable;
		if (compositeDisposable2 != null)
		{
			compositeDisposable2.Dispose();
		}
		_playerViewTrackDisposable?.Dispose();
		_disposable?.Dispose();
	}

	public void Init(int totalAmount)
	{
		_disposable?.Dispose();
		_disposable = ObservableExtensions.Subscribe<Vector2>(UnityUIComponentExtensions.OnValueChangedAsObservable(_scrollRect), (Action<Vector2>)delegate
		{
			OnValueChanged();
		});
		InitRange(totalAmount);
	}

	public void InitRange(int totalAmount)
	{
		if (_realPlayerIndex + 1 > totalAmount)
		{
			_bottomPadding = 50;
		}
		else
		{
			_bottomPadding = -60;
		}
		_totalAmount = totalAmount;
		SetData();
	}

	public void SetupPlayerName(string name)
	{
		_playerView.SetupName(name);
	}

	public void SetupPlayers(int playerIndex, int realPlayerIndex)
	{
		_playerIndex = playerIndex;
		_realPlayerIndex = realPlayerIndex;
		if (playerIndex != -1)
		{
			HandlePlayer();
			OnValueChanged();
		}
	}

	public void ClearState()
	{
		_playerIndex = -1;
		for (int i = 0; i < _views.Length; i++)
		{
			_views[i].SwapVisibleState(isActive: true);
		}
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Clear();
		}
		_playerViewTrackDisposable?.Dispose();
		SetData();
		_playerView.transform.parent = _bottomSnap;
		_playerView.transform.localPosition = Vector3.zero;
		_playerView.PlayBigAnimation();
	}

	private void Validate()
	{
		if (_playerView.transform.parent != null && _playerView.transform.parent != _bottomSnap && _playerView.transform.parent != _topSnap && _currentPlayerParent.Index != _playerIndex)
		{
			_currentPlayerParent.SwapVisibleState(isActive: true);
			_currentPlayerParent = _views.FirstOrDefault((RatingPlayerView view) => view.Index == _playerIndex);
			if (_currentPlayerParent != null)
			{
				_playerView.transform.parent = _currentPlayerParent.ParentRoot;
				_playerView.transform.localPosition = Vector3.zero;
				_playerView.PlaySmallAnimation();
			}
		}
		_playerView.RectTransform.sizeDelta = _views[0].RectTransform.sizeDelta;
	}

	private void ValidateInRange()
	{
		if (!(_playerView.transform.parent != null) || _playerIndex != _minTotalAmount || !(_playerView.transform.parent == _bottomSnap))
		{
			return;
		}
		RatingPlayerView ratingPlayerView = _views.FirstOrDefault((RatingPlayerView view) => view.Index == _playerIndex);
		if (ratingPlayerView != null && ratingPlayerView.transform.position.y >= _bottomSnapPoint.transform.position.y)
		{
			if (_currentPlayerParent != null)
			{
				_currentPlayerParent.SwapVisibleState(isActive: true);
			}
			_currentPlayerParent = ratingPlayerView;
			_currentPlayerParent.SwapVisibleState(isActive: false);
			_playerView.transform.parent = _currentPlayerParent.ParentRoot;
			_playerView.transform.localPosition = Vector3.zero;
			_playerView.PlaySmallAnimation();
		}
	}

	private void FastScrollValidate()
	{
		int num = _views.Min((RatingPlayerView view) => view.Index);
		int num2 = _views.Max((RatingPlayerView view) => view.Index);
		if (_playerIndex > num2)
		{
			if (_currentPlayerParent != null)
			{
				_currentPlayerParent.SwapVisibleState(isActive: true);
			}
			_playerViewTrackDisposable?.Dispose();
			_playerView.transform.parent = _bottomSnap;
			_playerView.transform.localPosition = Vector3.zero;
			_playerView.PlayBigAnimation();
		}
		else if (_playerIndex < num)
		{
			if (_currentPlayerParent != null)
			{
				_currentPlayerParent.SwapVisibleState(isActive: true);
			}
			_playerViewTrackDisposable?.Dispose();
			_playerView.transform.parent = _topSnap;
			_playerView.transform.localPosition = Vector3.zero;
			_playerView.PlayBigAnimation();
		}
	}

	private void OnValueChanged()
	{
		_currentContentPosition = _content.anchoredPosition.y - (float)_spacing;
		if (_currentContentPosition < 0f)
		{
			return;
		}
		int num = Mathf.FloorToInt(_currentContentPosition / (float)(120 + _spacing));
		if (_oldIndex != num)
		{
			if (num > _oldIndex)
			{
				TryMoveToBottom(num);
			}
			else
			{
				TryMoveToTop(num);
			}
			_oldIndex = num;
			Validate();
		}
	}

	private void TryMoveToTop(int newIndex)
	{
		int num = Mathf.Min(_views.Length, _totalAmount);
		int num2 = newIndex % _views.Length;
		List<int> list = new List<int>();
		for (int i = 0; i < num; i++)
		{
			int num3 = (num2 + i) % _views.Length;
			int num4 = newIndex + i;
			if (num4 >= _totalAmount)
			{
				continue;
			}
			list.Add(num4);
			RectTransform rectTransform = _views[num3].RectTransform;
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			anchoredPosition.y = -(_topPadding + num4 * _spacing + num4 * 120);
			rectTransform.anchoredPosition = anchoredPosition;
			_views[num3].SetupPlace(num4);
			_onOpponentViewPositionUpdate.OnNext(_views[num3]);
			if (num4 != _playerIndex)
			{
				continue;
			}
			_playerViewTrackDisposable?.Dispose();
			if (_currentPlayerParent != null)
			{
				_currentPlayerParent.SwapVisibleState(isActive: true);
			}
			if (_views.Where((RatingPlayerView view) => view.Index == _playerIndex).Count() >= 2)
			{
				if (_playerIndex == _totalAmount - 1)
				{
					IEnumerable<RatingPlayerView> source = _views.Where((RatingPlayerView view) => view.Index == _playerIndex);
					if (source.First().transform.position.y < source.Last().transform.position.y)
					{
						_currentPlayerParent = source.First();
					}
					else
					{
						_currentPlayerParent = source.Last();
					}
				}
				else if (_playerIndex == 0)
				{
					IEnumerable<RatingPlayerView> source2 = _views.Where((RatingPlayerView view) => view.Index == _playerIndex);
					if (source2.First().transform.position.y > source2.Last().transform.position.y)
					{
						_currentPlayerParent = source2.First();
					}
					else
					{
						_currentPlayerParent = source2.Last();
					}
				}
				else
				{
					_currentPlayerParent = _views[num3];
				}
			}
			else
			{
				_currentPlayerParent = _views[num3];
			}
			_currentPlayerParent.SwapVisibleState(isActive: false);
			_playerViewTrackDisposable = ObservableExtensions.Subscribe<float>(ObserveExtensions.ObserveEveryValueChanged<Transform, float>(_currentPlayerParent.transform, (Func<Transform, float>)((Transform value) => value.position.y), (FrameCountType)0, false), (Action<float>)delegate(float currentYPosition)
			{
				if (currentYPosition <= _playerView.transform.position.y)
				{
					_playerView.transform.parent = _currentPlayerParent.ParentRoot;
					_playerView.transform.localPosition = Vector3.zero;
					_playerView.PlaySmallAnimation();
				}
			});
		}
		_isInRange = list.Any((int id) => id == _playerIndex);
		if (!_isInRange)
		{
			CheckSnap(_playerView.transform.position.y);
		}
	}

	private void TryMoveToBottom(int newIndex)
	{
		int num = _views.Length - 1;
		int num2 = newIndex % _views.Length;
		List<int> list = new List<int>();
		for (int i = 0; i < _views.Length; i++)
		{
			int num3 = (num2 - i + _views.Length) % _views.Length;
			int num4 = newIndex + num - i;
			if (num4 >= _totalAmount)
			{
				continue;
			}
			list.Add(num4);
			RectTransform rectTransform = _views[num3].RectTransform;
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			anchoredPosition.y = -(_topPadding + num4 * _spacing + num4 * 120);
			rectTransform.anchoredPosition = anchoredPosition;
			_views[num3].SetupPlace(num4);
			_onOpponentViewPositionUpdate.OnNext(_views[num3]);
			if (num4 != _playerIndex)
			{
				continue;
			}
			_playerViewTrackDisposable?.Dispose();
			if (_currentPlayerParent != null)
			{
				_currentPlayerParent.SwapVisibleState(isActive: true);
			}
			if (_views.Where((RatingPlayerView view) => view.Index == _playerIndex).Count() >= 2)
			{
				if (_playerIndex == _totalAmount - 1)
				{
					IEnumerable<RatingPlayerView> source = _views.Where((RatingPlayerView view) => view.Index == _playerIndex);
					if (source.First().transform.position.y < source.Last().transform.position.y)
					{
						_currentPlayerParent = source.First();
					}
					else
					{
						_currentPlayerParent = source.Last();
					}
				}
				else if (_playerIndex == 0)
				{
					IEnumerable<RatingPlayerView> source2 = _views.Where((RatingPlayerView view) => view.Index == _playerIndex);
					if (source2.First().transform.position.y > source2.Last().transform.position.y)
					{
						_currentPlayerParent = source2.First();
					}
					else
					{
						_currentPlayerParent = source2.Last();
					}
				}
				else
				{
					_currentPlayerParent = _views[num3];
				}
			}
			else
			{
				_currentPlayerParent = _views[num3];
			}
			_currentPlayerParent.SwapVisibleState(isActive: false);
			_playerViewTrackDisposable = ObservableExtensions.Subscribe<float>(ObserveExtensions.ObserveEveryValueChanged<Transform, float>(_currentPlayerParent.transform, (Func<Transform, float>)((Transform value) => value.position.y), (FrameCountType)0, false), (Action<float>)delegate(float currentYPosition)
			{
				if (currentYPosition >= _playerView.transform.position.y)
				{
					_playerView.transform.parent = _currentPlayerParent.ParentRoot;
					_playerView.transform.localPosition = Vector3.zero;
					_playerView.PlaySmallAnimation();
				}
			});
		}
		_isInRange = list.Any((int id) => id == _playerIndex);
		if (!_isInRange)
		{
			CheckSnap(_playerView.transform.position.y);
		}
	}

	private void SetData()
	{
		_oldIndex = 0;
		float y = (float)(120 * _totalAmount) * 1f + (float)_topPadding + (float)_bottomPadding + (float)((_totalAmount != 0) ? ((_totalAmount - 1) * _spacing) : 0);
		_content.sizeDelta = new Vector2(_content.sizeDelta.x, y);
		Vector2 anchoredPosition = _content.anchoredPosition;
		anchoredPosition.y = 0f;
		_content.anchoredPosition = anchoredPosition;
		int num = _topPadding;
		for (int i = 0; i < _views.Length; i++)
		{
			bool flag = i < _totalAmount;
			_views[i].gameObject.SetActive(flag);
			_views[i].SetupPlace(i);
			if (flag)
			{
				anchoredPosition = _views[i].RectTransform.anchoredPosition;
				anchoredPosition.y = -num;
				_views[i].RectTransform.anchoredPosition = anchoredPosition;
				num += _spacing + 120;
				_onOpponentViewPositionUpdate.OnNext(_views[i]);
			}
		}
	}

	private void HandlePlayer()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<float>(ObserveExtensions.ObserveEveryValueChanged<Transform, float>(_playerView.transform, (Func<Transform, float>)((Transform value) => value.position.y), (FrameCountType)0, false), (Action<float>)delegate(float currentYPosition)
		{
			CheckFirstPlace(currentYPosition);
			CheckMiddlePlace(currentYPosition);
			if (_isInRange)
			{
				CheckSnap(currentYPosition);
			}
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	private void CheckSnap(float currentYPosition)
	{
		if (currentYPosition <= _bottomSnapPoint.transform.position.y)
		{
			if (_currentPlayerParent != null)
			{
				_currentPlayerParent.SwapVisibleState(isActive: true);
			}
			_playerViewTrackDisposable?.Dispose();
			_playerView.transform.parent = _bottomSnap;
			_playerView.transform.localPosition = Vector3.zero;
			_playerView.PlayBigAnimation();
		}
		else if (currentYPosition >= _topSnapPoint.transform.position.y)
		{
			if (_currentPlayerParent != null)
			{
				_currentPlayerParent.SwapVisibleState(isActive: true);
			}
			_playerViewTrackDisposable?.Dispose();
			_playerView.transform.parent = _topSnap;
			_playerView.transform.localPosition = Vector3.zero;
			_playerView.PlayBigAnimation();
		}
	}

	private void CheckFirstPlace(float currentYPosition)
	{
		if (_playerIndex == 0 && currentYPosition > _topSnapPoint.transform.position.y)
		{
			_playerView.transform.parent = _topSnap;
			_playerView.transform.localPosition = Vector3.zero;
			_playerView.PlayBigAnimation();
		}
	}

	private void CheckMiddlePlace(float currentYPosition)
	{
		if (_playerIndex != 0)
		{
			if (currentYPosition > _topSnapPoint.transform.position.y)
			{
				_playerView.transform.parent = _topSnap;
				_playerView.transform.localPosition = Vector3.zero;
				_playerView.PlayBigAnimation();
			}
			else if (currentYPosition < _bottomSnapPoint.transform.position.y)
			{
				_playerView.transform.parent = _bottomSnap;
				_playerView.transform.localPosition = Vector3.zero;
				_playerView.PlayBigAnimation();
			}
		}
	}

	public void ForceUpdate()
	{
		bool flag = false;
		for (int i = 0; i < _views.Length; i++)
		{
			if (i >= _totalAmount || _views[i].Index != _playerIndex)
			{
				continue;
			}
			flag = true;
			_playerViewTrackDisposable?.Dispose();
			if (_currentPlayerParent != null)
			{
				_currentPlayerParent.SwapVisibleState(isActive: true);
			}
			if (_views.Where((RatingPlayerView view) => view.Index == _playerIndex).Count() >= 2)
			{
				if (_playerIndex == _totalAmount - 1)
				{
					IEnumerable<RatingPlayerView> source = _views.Where((RatingPlayerView view) => view.Index == _playerIndex);
					if (source.First().transform.position.y < source.Last().transform.position.y)
					{
						_currentPlayerParent = source.First();
					}
					else
					{
						_currentPlayerParent = source.Last();
					}
				}
				else if (_playerIndex == 0)
				{
					IEnumerable<RatingPlayerView> source2 = _views.Where((RatingPlayerView view) => view.Index == _playerIndex);
					if (source2.First().transform.position.y > source2.Last().transform.position.y)
					{
						_currentPlayerParent = source2.First();
					}
					else
					{
						_currentPlayerParent = source2.Last();
					}
				}
			}
			else
			{
				_currentPlayerParent = _views[i];
			}
			_currentPlayerParent.SwapVisibleState(isActive: false);
			_playerView.transform.parent = _currentPlayerParent.ParentRoot;
			_playerView.transform.localPosition = Vector3.zero;
			_playerView.PlaySmallAnimation();
		}
		if (!flag && _playerIndex != -1)
		{
			int num = _views.Min((RatingPlayerView view) => view.Index);
			int num2 = _views.Max((RatingPlayerView view) => view.Index);
			if (_playerIndex > num2)
			{
				_playerView.transform.parent = _bottomSnap;
				_playerView.transform.localPosition = Vector3.zero;
				_playerView.PlayBigAnimation();
			}
			else if (_playerIndex < num)
			{
				_playerView.transform.parent = _topSnap;
				_playerView.transform.localPosition = Vector3.zero;
				_playerView.PlayBigAnimation();
			}
		}
		if (!flag && _playerIndex != -1 && _totalAmount <= _minTotalAmount && _playerIndex == _totalAmount)
		{
			_playerViewTrackDisposable?.Dispose();
			_views[_playerIndex].gameObject.SetActive(value: true);
			_currentPlayerParent = _views[_playerIndex];
			_currentPlayerParent.SwapVisibleState(isActive: false);
			_playerView.transform.parent = _currentPlayerParent.ParentRoot;
			_playerView.transform.localPosition = Vector3.zero;
			_playerView.PlaySmallAnimation();
		}
	}
}
