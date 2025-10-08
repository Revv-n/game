using System;
using System.Collections.Generic;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.HornyScapes.Relationships.Providers;
using GreenT.HornyScapes.Relationships.Views;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Animations;

public class LevelUpAnimationService : MonoView
{
	[SerializeField]
	private TMP_Text _title;

	[SerializeField]
	private ChainAnimationGroup _animation;

	[SerializeField]
	private RelationshipStatusView _statusView;

	[SerializeField]
	private StatableComponentGroup _states;

	[SerializeField]
	private ParticleSystem _particleSystem;

	[SerializeField]
	private float _particleSystemDelay;

	[SerializeField]
	private float _closeDelay;

	private RelationshipMapperProvider _relationshipMapperProvider;

	private RelationshipRewardMapperProvider _relationshipRewardMapperProvider;

	private IDisposable _closeStream;

	private Sequence _particleSystemSequence;

	private readonly int _kiraRelationshipId = 1001;

	private readonly int _kiraFirstRewardIndex;

	[Inject]
	public void Init(RelationshipMapperProvider relationshipMapperProvider, RelationshipRewardMapperProvider relationshipRewardMapperProvider)
	{
		_relationshipMapperProvider = relationshipMapperProvider;
		_relationshipRewardMapperProvider = relationshipRewardMapperProvider;
	}

	public void PlayAnimation(int relationshipId, int index)
	{
		_closeStream?.Dispose();
		if (relationshipId == _kiraRelationshipId && index == _kiraFirstRewardIndex)
		{
			StopAnimation();
			return;
		}
		_statusView.ForceSetStatus(relationshipId, index);
		int currentStatus = GetCurrentStatus(relationshipId, index);
		_states.Set(currentStatus);
		Display(display: true);
		StopAnimation();
		_animation.Play();
		_particleSystemSequence = DOTween.Sequence().AppendInterval(_particleSystemDelay).AppendCallback(delegate
		{
			_particleSystem.Play();
		});
		_closeStream = Observable.Timer(TimeSpan.FromSeconds(_closeDelay)).ObserveOnMainThread().Subscribe(delegate
		{
			SetCloseStream();
		});
	}

	private void Awake()
	{
		Display(display: false);
		_animation.Init();
	}

	private void StopAnimation()
	{
		if (_particleSystemSequence != null)
		{
			_particleSystemSequence.Kill();
			_particleSystemSequence = null;
		}
		_animation.Stop();
		_particleSystem.Stop();
	}

	private void SetCloseStream()
	{
		_closeStream?.Dispose();
		_closeStream = (from _ in Observable.EveryUpdate()
			where Input.GetKeyDown(KeyCode.Mouse0)
			select _).Subscribe(delegate
		{
			CloseWindow();
		});
	}

	private void CloseWindow()
	{
		Display(display: false);
		_closeStream?.Dispose();
		_animation.Stop();
	}

	private int GetCurrentStatus(int relationshipId, int index)
	{
		int num = 1;
		List<int> rewardStatuses = GetRewardStatuses(relationshipId);
		int num2 = int.MaxValue;
		for (int i = 0; i < index + 1; i++)
		{
			int num3 = rewardStatuses[i];
			if (num3 < num2)
			{
				num++;
			}
			num2 = num3;
		}
		return num;
	}

	private List<int> GetRewardStatuses(int relationshipId)
	{
		int[] rewards = _relationshipMapperProvider.Get(relationshipId).rewards;
		List<int> list = new List<int>(rewards.Length);
		int[] array = rewards;
		foreach (int id in array)
		{
			RelationshipRewardMapper relationshipRewardMapper = _relationshipRewardMapperProvider.Get(id);
			list.Add(relationshipRewardMapper.status_number);
		}
		return list;
	}
}
