using System;
using System.Linq;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.HornyScapes.UI;
using GreenT.Types;
using Merge.Meta.RoomObjects;
using StripClub.Model.Cards;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.StarShop.SubSystems;

public class StarShopGirlFlow : IDisposable
{
	private readonly RoomManager roomManager;

	private readonly StartFlow startFlow;

	private readonly CardsCollection cardsCollection;

	private readonly IndicatorDisplayService _displayService;

	private readonly SignalBus _signalBus;

	private readonly CompositeDisposable showCharacterStream = new CompositeDisposable();

	public StarShopGirlFlow(RoomManager roomManager, StartFlow startFlow, CardsCollection cardsCollection, IndicatorDisplayService displayService, SignalBus signalBus)
	{
		_signalBus = signalBus;
		this.startFlow = startFlow;
		this.roomManager = roomManager;
		_displayService = displayService;
		this.cardsCollection = cardsCollection;
	}

	public void Activate(bool isOn)
	{
		showCharacterStream.Clear();
		if (isOn)
		{
			TrackDisplayConditionCharacters();
		}
	}

	private void TrackDisplayConditionCharacters()
	{
		ShowUnlockedGirl();
		ShowLockedGirl();
	}

	private void ShowUnlockedGirl()
	{
		try
		{
			(from _character in cardsCollection.Collection.OfType<ICharacter>()
				where _character.DisplayCondition.IsOpen.Value
				select roomManager.GetCharacterObject(_character.ID)).ToObservable().Subscribe(delegate(CharacterObject _character)
			{
				_character.SetStatus(EntityStatus.Rewarded);
			}, delegate(Exception ex)
			{
				throw ex.LogException();
			}).AddTo(showCharacterStream);
		}
		catch (Exception)
		{
		}
	}

	private void ShowLockedGirl()
	{
		try
		{
			IConnectableObservable<ICharacter> connectableObservable = (from _character in cardsCollection.Collection.OfType<ICharacter>().ToObservable()
				where !_character.DisplayCondition.IsOpen.Value
				select _character).SelectMany((Func<ICharacter, IObservable<ICharacter>>)EmitUnlockedCharacter).Publish();
			IObservable<ICharacter> observable = connectableObservable.Where((ICharacter _character) => !roomManager.IsObjectSet(new CompositeIdentificator(_character.ID), isGirlId: true)).SelectMany((Func<ICharacter, IObservable<ICharacter>>)EmitCharacterOnRoomCreated);
			IObservable<CharacterObject> source = (from _character in connectableObservable.Where((ICharacter _character) => roomManager.IsObjectSet(new CompositeIdentificator(_character.ID), isGirlId: true)).Merge(observable)
				select roomManager.GetCharacterObject(_character.ID)).Share();
			IObservable<CharacterObject> observable2 = source.Where((CharacterObject _) => !startFlow.IsLaunched).Merge(source.Where((CharacterObject _) => startFlow.IsLaunched));
			RequestToShow(observable2);
			(from _pair in observable2.CombineLatest(_displayService.OnIndicatorPush(FilteredIndicatorType.StarShop), (CharacterObject _object, bool _indicator) => (_object: _object, _indicator: _indicator))
				where _pair._indicator
				select _pair).Subscribe(delegate
			{
				_signalBus.TryFire(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.StarShop));
				startFlow.TryShowFlow();
			}, delegate(Exception ex)
			{
				throw ex.LogException();
			}).AddTo(showCharacterStream);
			connectableObservable.Connect().AddTo(showCharacterStream);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException(GetType().Name ?? "");
		}
		IObservable<ICharacter> EmitCharacterOnRoomCreated(ICharacter character)
		{
			return from _characterObject in roomManager.OnNew.SelectMany((Room _room) => _room.RoomObjects).OfType<IRoomObject<BaseObjectConfig>, CharacterObject>()
				where _characterObject.Config.CharacterID == character.ID
				select _characterObject into _
				select character;
		}
		static IObservable<ICharacter> EmitUnlockedCharacter(ICharacter character)
		{
			return from x in character.DisplayCondition.IsOpen.Skip(1)
				where x
				select x into _
				select character;
		}
	}

	private void RequestToShow(IObservable<CharacterObject> notLaunchedFlow)
	{
		notLaunchedFlow.Subscribe(delegate(CharacterObject roomObject)
		{
			startFlow.AddObjectToFlow(roomObject);
			_signalBus.TryFire(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.StarShop));
		}).AddTo(showCharacterStream);
	}

	public void Dispose()
	{
		showCharacterStream.Dispose();
	}
}
