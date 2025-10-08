using System;
using System.Collections.Generic;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
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
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CharacterObject>(Observable.ToObservable<CharacterObject>(from _character in cardsCollection.Collection.OfType<ICharacter>()
				where _character.DisplayCondition.IsOpen.Value
				select roomManager.GetCharacterObject(_character.ID)), (Action<CharacterObject>)delegate(CharacterObject _character)
			{
				_character.SetStatus(EntityStatus.Rewarded);
			}, (Action<Exception>)delegate(Exception ex)
			{
				throw ex.LogException();
			}), (ICollection<IDisposable>)showCharacterStream);
		}
		catch (Exception)
		{
		}
	}

	private void ShowLockedGirl()
	{
		try
		{
			IConnectableObservable<ICharacter> val = Observable.Publish<ICharacter>(Observable.SelectMany<ICharacter, ICharacter>(Observable.Where<ICharacter>(Observable.ToObservable<ICharacter>(cardsCollection.Collection.OfType<ICharacter>()), (Func<ICharacter, bool>)((ICharacter _character) => !_character.DisplayCondition.IsOpen.Value)), (Func<ICharacter, IObservable<ICharacter>>)EmitUnlockedCharacter));
			IObservable<ICharacter> observable = Observable.SelectMany<ICharacter, ICharacter>(Observable.Where<ICharacter>((IObservable<ICharacter>)val, (Func<ICharacter, bool>)((ICharacter _character) => !roomManager.IsObjectSet(new CompositeIdentificator(_character.ID), isGirlId: true))), (Func<ICharacter, IObservable<ICharacter>>)EmitCharacterOnRoomCreated);
			IObservable<CharacterObject> observable2 = Observable.Share<CharacterObject>(Observable.Select<ICharacter, CharacterObject>(Observable.Merge<ICharacter>(Observable.Where<ICharacter>((IObservable<ICharacter>)val, (Func<ICharacter, bool>)((ICharacter _character) => roomManager.IsObjectSet(new CompositeIdentificator(_character.ID), isGirlId: true))), new IObservable<ICharacter>[1] { observable }), (Func<ICharacter, CharacterObject>)((ICharacter _character) => roomManager.GetCharacterObject(_character.ID))));
			IObservable<CharacterObject> observable3 = Observable.Merge<CharacterObject>(Observable.Where<CharacterObject>(observable2, (Func<CharacterObject, bool>)((CharacterObject _) => !startFlow.IsLaunched)), new IObservable<CharacterObject>[1] { Observable.Where<CharacterObject>(observable2, (Func<CharacterObject, bool>)((CharacterObject _) => startFlow.IsLaunched)) });
			RequestToShow(observable3);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(CharacterObject, bool)>(Observable.Where<(CharacterObject, bool)>(Observable.CombineLatest<CharacterObject, bool, (CharacterObject, bool)>(observable3, _displayService.OnIndicatorPush(FilteredIndicatorType.StarShop), (Func<CharacterObject, bool, (CharacterObject, bool)>)((CharacterObject _object, bool _indicator) => (_object: _object, _indicator: _indicator))), (Func<(CharacterObject, bool), bool>)(((CharacterObject _object, bool _indicator) _pair) => _pair._indicator)), (Action<(CharacterObject, bool)>)delegate
			{
				_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.StarShop));
				startFlow.TryShowFlow();
			}, (Action<Exception>)delegate(Exception ex)
			{
				throw ex.LogException();
			}), (ICollection<IDisposable>)showCharacterStream);
			DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)showCharacterStream);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException(GetType().Name ?? "");
		}
		IObservable<ICharacter> EmitCharacterOnRoomCreated(ICharacter character)
		{
			return Observable.Select<CharacterObject, ICharacter>(Observable.Where<CharacterObject>(Observable.OfType<IRoomObject<BaseObjectConfig>, CharacterObject>(Observable.SelectMany<Room, IRoomObject<BaseObjectConfig>>(roomManager.OnNew, (Func<Room, IEnumerable<IRoomObject<BaseObjectConfig>>>)((Room _room) => _room.RoomObjects))), (Func<CharacterObject, bool>)((CharacterObject _characterObject) => _characterObject.Config.CharacterID == character.ID)), (Func<CharacterObject, ICharacter>)((CharacterObject _) => character));
		}
		static IObservable<ICharacter> EmitUnlockedCharacter(ICharacter character)
		{
			return Observable.Select<bool, ICharacter>(Observable.Where<bool>(Observable.Skip<bool>((IObservable<bool>)character.DisplayCondition.IsOpen, 1), (Func<bool, bool>)((bool x) => x)), (Func<bool, ICharacter>)((bool _) => character));
		}
	}

	private void RequestToShow(IObservable<CharacterObject> notLaunchedFlow)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CharacterObject>(notLaunchedFlow, (Action<CharacterObject>)delegate(CharacterObject roomObject)
		{
			startFlow.AddObjectToFlow(roomObject);
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.StarShop));
		}), (ICollection<IDisposable>)showCharacterStream);
	}

	public void Dispose()
	{
		showCharacterStream.Dispose();
	}
}
