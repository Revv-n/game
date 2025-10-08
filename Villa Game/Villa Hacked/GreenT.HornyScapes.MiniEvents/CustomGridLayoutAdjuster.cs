using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.MiniEvents;

[RequireComponent(typeof(GridLayoutGroup))]
public sealed class CustomGridLayoutAdjuster : MonoBehaviour
{
	[SerializeField]
	private GridLayoutGroup _gridLayoutGroup;

	[SerializeField]
	private int _currentElementCount;

	[Header("Settings")]
	[SerializeField]
	private int _maxElementsPerRow = 4;

	[SerializeField]
	private int _defaultElementsPerRow = 3;

	[SerializeField]
	private Vector2 _defaultCellSize = new Vector2(328f, 100f);

	[SerializeField]
	private Vector2 _minimizedCellSize = new Vector2(256f, 78f);

	public void OnAddElement()
	{
		_currentElementCount++;
		UpdateCellSize(_currentElementCount);
	}

	public void OnRemoveElement()
	{
		_currentElementCount--;
		_currentElementCount = Mathf.Clamp(_currentElementCount, 0, int.MaxValue);
		UpdateCellSize(_currentElementCount);
	}

	private void UpdateCellSize(int currentElementsAmount)
	{
		_gridLayoutGroup.constraintCount = ((currentElementsAmount >= _maxElementsPerRow) ? _maxElementsPerRow : _defaultElementsPerRow);
		_gridLayoutGroup.cellSize = ((currentElementsAmount >= _maxElementsPerRow) ? _minimizedCellSize : _defaultCellSize);
	}
}
