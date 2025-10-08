using System.Collections.Generic;
using GreenT.HornyScapes.Relationships.Views;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Windows;

public sealed class RelationshipBackgroundController : MonoBehaviour
{
	[SerializeField]
	private RelationshipBackground _backgroundPrefab;

	[SerializeField]
	private RelationshipBackground _gradientPrefab;

	[SerializeField]
	private BlockedRelationshipBackground _blockedBackground;

	[SerializeField]
	private RectTransform _container;

	[SerializeField]
	private RectTransform _pattern;

	private SmartPool<RelationshipBackground> _backgroundPool;

	private SmartPool<RelationshipBackground> _gradientPool;

	private readonly Dictionary<int, Dictionary<int, RelationshipBackground>> _backgrounds = new Dictionary<int, Dictionary<int, RelationshipBackground>>(4);

	private readonly Dictionary<int, Dictionary<int, RelationshipBackground>> _gradients = new Dictionary<int, Dictionary<int, RelationshipBackground>>(4);

	public void Set(int relationshipId, BaseRewardView rewardView, int status, bool isNewStatus)
	{
		TryInitPools();
		float width = rewardView.RectTransform.rect.width;
		Vector2 position = Vector2.zero;
		if (!_backgrounds.ContainsKey(relationshipId))
		{
			_backgrounds[relationshipId] = new Dictionary<int, RelationshipBackground>();
		}
		if (!_gradients.ContainsKey(relationshipId))
		{
			_gradients[relationshipId] = new Dictionary<int, RelationshipBackground>();
		}
		Dictionary<int, RelationshipBackground> dictionary = _backgrounds[relationshipId];
		Dictionary<int, RelationshipBackground> dictionary2 = _gradients[relationshipId];
		if (isNewStatus)
		{
			int key = status - 1;
			if (dictionary.ContainsKey(key))
			{
				RelationshipBackground relationshipBackground = dictionary[key];
				position.x = relationshipBackground.Position.x + relationshipBackground.Width;
			}
			RelationshipBackground relationshipBackground2 = _gradientPool.Pop();
			relationshipBackground2.SetScale(Vector3.one);
			relationshipBackground2.AddWidth(width);
			relationshipBackground2.SetPosition(in position);
			relationshipBackground2.SetStatus(status);
			dictionary2.Add(status, relationshipBackground2);
		}
		else if (dictionary.ContainsKey(status))
		{
			dictionary[status].AddWidth(width);
		}
		else
		{
			if (dictionary2.ContainsKey(status))
			{
				RelationshipBackground relationshipBackground3 = dictionary2[status];
				position.x = relationshipBackground3.Position.x + relationshipBackground3.Width;
			}
			RelationshipBackground relationshipBackground4 = _backgroundPool.Pop();
			relationshipBackground4.SetScale(Vector3.one);
			relationshipBackground4.AddWidth(width);
			relationshipBackground4.SetPosition(in position);
			relationshipBackground4.SetStatus(status);
			dictionary.Add(status, relationshipBackground4);
		}
		_blockedBackground.AddReward(relationshipId, rewardView);
		_pattern.SetAsLastSibling();
	}

	public void SetNeedAnimate(int relationshipId)
	{
		_blockedBackground.SetNeedAnimate(relationshipId);
	}

	public void TryStartAnimation(int relationshipId)
	{
		_blockedBackground.TryStartAnimation(relationshipId);
	}

	public void Clear(int relationshipId)
	{
		if (!_backgrounds.ContainsKey(relationshipId))
		{
			return;
		}
		Dictionary<int, RelationshipBackground> dictionary = _backgrounds[relationshipId];
		Dictionary<int, RelationshipBackground> dictionary2 = _gradients[relationshipId];
		foreach (RelationshipBackground value in dictionary.Values)
		{
			value.Clear();
			_backgroundPool.ReturnItemInPull(value);
		}
		foreach (RelationshipBackground value2 in dictionary2.Values)
		{
			value2.Clear();
			_gradientPool.ReturnItemInPull(value2);
		}
		dictionary.Clear();
		dictionary2.Clear();
		_blockedBackground.Clear(relationshipId);
	}

	private void TryInitPools()
	{
		if (_backgroundPool == null)
		{
			_backgroundPool = new SmartPool<RelationshipBackground>(_backgroundPrefab, _container);
		}
		if (_gradientPool == null)
		{
			_gradientPool = new SmartPool<RelationshipBackground>(_gradientPrefab, _container);
		}
	}
}
