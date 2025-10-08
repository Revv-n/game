using System;
using System.Linq;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Meta.Navigation;
using GreenT.HornyScapes.StarShop;
using GreenT.HornyScapes.UI;
using Merge.Meta.RoomObjects;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta;

public class HouseView : MonoBehaviour
{
	private MainScreenIndicator _mainScreenIndicator;

	private HouseNavigationController navigationController;

	private RoomManager house;

	private IInputBlockerController clickBlockers;

	private IFactory<RoomConfig, Transform, Room> roomFactory;

	private RoomConfigManager roomConfigManager;

	private StarShopManager starManager;

	private DecorationManager decorationManager;

	private IHouseBuilder builder;

	private CharacterSettingsManager characterManager;

	private GameStarter gameStarter;

	private bool clickBlocked;

	private bool mainScreenIsVisible;

	private readonly CompositeDisposable disposables = new CompositeDisposable();

	private IDisposable skinTracker;

	[Inject]
	public void Init(HouseNavigationController navigationController, MainScreenIndicator mainScreenIndicator, RoomManager house, IInputBlockerController clickBlockers, IFactory<RoomConfig, Transform, Room> roomFactory, RoomConfigManager roomConfigManager, StarShopManager starManager, IHouseBuilder builder, CharacterSettingsManager cards, GameStarter gameStarter, DecorationManager decorationManager, IndicatorDisplayService displayService)
	{
		_mainScreenIndicator = mainScreenIndicator;
		this.navigationController = navigationController;
		this.house = house;
		this.clickBlockers = clickBlockers;
		this.roomFactory = roomFactory;
		this.roomConfigManager = roomConfigManager;
		this.starManager = starManager;
		this.builder = builder;
		characterManager = cards;
		this.gameStarter = gameStarter;
		this.decorationManager = decorationManager;
	}

	protected virtual void Start()
	{
		InitRooms();
		InitRoomObjects();
		InitSystems();
	}

	private void InitRooms()
	{
		house.OnNew.Where((Room room) => room.ID == 100).Take(1).Subscribe(DropFade, delegate(Exception exception)
		{
			exception.LogException();
		});
		foreach (RoomConfig item in roomConfigManager.Collection)
		{
			AddRoom(item);
		}
		roomConfigManager.OnNew.Subscribe(AddRoom).AddTo(this);
	}

	private void DropFade(Room room)
	{
		room.GetObjectOrDefault(0).SetView(1);
	}

	private void InitRoomObjects()
	{
		SetupOwnedRoomObjects();
		SetupUnlockedRoomObjects();
		SetupCharacterSkins();
	}

	private void SetupCharacterSkins()
	{
		skinTracker?.Dispose();
		skinTracker = gameStarter.IsGameActive.FirstOrDefault((bool x) => x).ContinueWith(characterManager.Collection.ToObservable().Merge(characterManager.OnNew)).SelectMany((CharacterSettings x) => x.OnUpdate)
			.StartWith(characterManager.Collection.Where((CharacterSettings x) => x.SkinID != 0))
			.Subscribe(UpdateSkin);
	}

	public void UpdateSkin(CharacterSettings character)
	{
		int iD = character.Public.ID;
		if (house.IsCharacterObjectSet(iD, out var characterObject))
		{
			characterObject.SetSkin(character.SkinID);
		}
	}

	private void SetupOwnedRoomObjects()
	{
		foreach (StarShopItem item in from _item in starManager.Collection
			where _item.State == EntityStatus.Rewarded
			join _config in roomConfigManager.Collection on _item.HouseObjectID[0] equals _config.RoomID
			where _config != null
			select _item)
		{
			builder.BuildRoomObject(item.HouseObjectID);
		}
		foreach (Decoration item2 in from decoration in decorationManager.Collection
			where (decoration.State == EntityStatus.Rewarded) & decoration.DisplayCondition.IsOpen.Value
			join config in roomConfigManager.Collection on decoration.HouseObjectID[0] equals config.RoomID
			where config != null
			select decoration)
		{
			builder.BuildRoomObject(item2.HouseObjectID);
		}
	}

	private void SetupUnlockedRoomObjects()
	{
		foreach (StarShopItem item in starManager.Collection.Where((StarShopItem _item) => _item.State == EntityStatus.InProgress || _item.State == EntityStatus.Complete))
		{
			house.GetObject(item.HouseObjectID).SetStatus(item.State);
		}
	}

	private void AddRoom(RoomConfig config)
	{
		if (!house.Collection.Any((Room _room) => _room.ID == config.RoomID))
		{
			Room entity = roomFactory.Create(config, base.transform);
			house.Add(entity);
		}
	}

	private void InitSystems()
	{
		disposables.Clear();
		bool active = IsActive();
		navigationController.Activate(active);
		clickBlockers.OnUpdate.Subscribe(delegate
		{
			clickBlocked = clickBlockers.ClickBlock;
			UpdateNavigationActivity();
		}).AddTo(disposables);
		_mainScreenIndicator.IsVisible.Subscribe(MainScreeIndicatorUpdate).AddTo(disposables);
	}

	private void UpdateNavigationActivity()
	{
		UpdateNavigationActivity(IsActive());
	}

	private void UpdateNavigationActivity(bool isActive)
	{
		navigationController.Activate(isActive);
		if (!isActive)
		{
			navigationController.SetDrag(hardState: false);
		}
	}

	private void MainScreeIndicatorUpdate(bool _isVisible)
	{
		mainScreenIsVisible = _isVisible;
		UpdateNavigationActivity();
	}

	private bool IsActive()
	{
		if (!clickBlocked)
		{
			return mainScreenIsVisible;
		}
		return false;
	}

	private void OnDisable()
	{
		disposables?.Dispose();
		skinTracker?.Dispose();
	}

	private void OnDestroy()
	{
		disposables?.Dispose();
		skinTracker?.Dispose();
		house.Init();
	}
}
