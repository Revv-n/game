using System;
using GreenT.HornyScapes.Messenger;
using StripClub.Messenger;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class GetConcreteRarityGirlPhotoObjective : GainObjective
{
	public readonly Rarity Rarity;

	protected readonly DialoguesTracker _dialoguesTracker;

	protected IDisposable _photoStream;

	public GetConcreteRarityGirlPhotoObjective(Func<Sprite> iconProvider, SavableObjectiveData data, DialoguesTracker dialoguesTracker, Rarity rarity)
		: base(iconProvider, data)
	{
		Rarity = rarity;
		_dialoguesTracker = dialoguesTracker;
	}

	public override void Track()
	{
		base.Track();
		_photoStream?.Dispose();
		_photoStream = ObservableExtensions.Subscribe<ChatMessage>(Observable.Where<ChatMessage>(_dialoguesTracker.GetConcreteNewPhotoStream(), (Func<ChatMessage, bool>)((ChatMessage message) => _dialoguesTracker.IsNeededrarity(message, Rarity))), (Action<ChatMessage>)delegate
		{
			AddProgress();
		});
	}

	public override void OnRewardTask()
	{
		base.OnRewardTask();
		_photoStream?.Dispose();
	}

	protected void AddProgress()
	{
		Data.Progress++;
		onUpdate.OnNext((IObjective)this);
	}
}
