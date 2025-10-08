using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Exceptions;
using GreenT.HornyScapes.StarShop;
using GreenT.Localizations;
using StripClub;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.NewEvent.Data;
using UniRx;

namespace GreenT.HornyScapes.Saves;

public class SaveController : IDisposable
{
	public ReactiveCommand OnStartSavingEvent = new ReactiveCommand();

	private readonly Saver saver;

	private readonly User user;

	private readonly SaveProviderFacade saveProviderFacade;

	private readonly SaveNotifier _saveNotifier;

	private readonly IReadOnlyReactiveProperty<bool> isGameActive;

	private readonly IReadOnlyReactiveProperty<bool> isGameDataLoaded;

	private readonly ReactiveProperty<bool> isUserDataLoaded = new ReactiveProperty<bool>(initialValue: false);

	private readonly RelativeProgress loadingProgress;

	private readonly IExceptionHandler exceptionHandler;

	private readonly IClock _clock;

	private readonly PostLoadingState _postLoadingState;

	private readonly InitializeState _initializeState;

	private readonly SimpleCurrencyFactory _currencyFactory;

	private readonly MigrateToNewBalance _migrateToNewBalance;

	private readonly LocalizationProvider _localizationProvider;

	private readonly LocalizationState _localizationState;

	private readonly PlayerPaymentsStats playerPaymentStats;

	private IDisposable delayedSaveStream;

	private readonly CompositeDisposable disposables = new CompositeDisposable();

	private GameStopSignal _gameStopSignal;

	public IReadOnlyReactiveProperty<bool> IsUserDataLoaded => isUserDataLoaded;

	public SaveController(ISaver saver, User user, SaveProviderFacade saveProviderFacade, GameStarter gameStarter, RelativeProgress loadingProgress, IExceptionHandler exceptionHandler, IClock clock, PostLoadingState postLoadingState, InitializeState initializeState, SimpleCurrencyFactory currencyFactory, PlayerPaymentsStats playerPaymentStats, MigrateToNewBalance migrateToNewBalance, LocalizationProvider localizationProvider, LocalizationState localizationState, SaveNotifier saveNotifier, GameStopSignal gameStopSignal)
	{
		gameStarter.SetConfigsLoaded(IsUserDataLoaded);
		this.saver = saver as Saver;
		this.user = user;
		this.saveProviderFacade = saveProviderFacade;
		isGameActive = gameStarter.IsGameActive;
		isGameDataLoaded = gameStarter.IsDataLoaded;
		this.loadingProgress = loadingProgress;
		_postLoadingState = postLoadingState;
		_initializeState = initializeState;
		_currencyFactory = currencyFactory;
		this.exceptionHandler = exceptionHandler;
		_clock = clock;
		this.playerPaymentStats = playerPaymentStats;
		_migrateToNewBalance = migrateToNewBalance;
		_localizationProvider = localizationProvider;
		_localizationState = localizationState;
		_saveNotifier = saveNotifier;
		_gameStopSignal = gameStopSignal;
	}

	public void Launch()
	{
		PreparePlayersDataOnEvent();
	}

	private void PreparePlayersDataOnEvent()
	{
		disposables.Clear();
		IObservable<bool> right = isGameDataLoaded.CombineLatest(isGameActive, (bool y, bool z) => y && !z);
		IConnectableObservable<User> connectableObservable = (from _pair in (from _pair in user.OnLogin.TakeUntil(_gameStopSignal.IsStopped.Where((bool x) => x)).CombineLatest(right, (User x, bool y) => (user: x, readyToInitialize: y))
				where _pair.readyToInitialize
				select _pair).Do(delegate
			{
				isUserDataLoaded.Value = false;
				saver.SetSuppressState(state: true);
			})
			select _pair.user).Publish();
		(from _ in connectableObservable.Where((User _user) => _user.Type.Equals(User.State.Unknown)).TakeUntil(_gameStopSignal.IsStopped.Where((bool x) => x)).Do(delegate
			{
				_localizationState.Initialize();
			})
			select _localizationProvider.OnLocalizationChange).Debug("Initialize game for UNKNOWN user").Subscribe(delegate
		{
			try
			{
				saver.ClearStates();
				_initializeState.InitializeGameStructures();
				playerPaymentStats.Init();
				_migrateToNewBalance.Migrate();
				SetAsLoaded();
			}
			catch (Exception ex2)
			{
				exceptionHandler.Handle(ex2);
			}
		}).AddTo(disposables);
		(from _ in connectableObservable.Where((User _user) => !_user.Type.Equals(User.State.Unknown)).TakeUntil(_gameStopSignal.IsStopped.Where((bool x) => x)).Do(delegate
			{
				_initializeState.InitializeGameStructures();
			})
				.SelectMany((User _) => saveProviderFacade.Deserialize<SavedData>())
				.Select(PrepareState)
				.Select(NewBalanceMigrateEmptyProgressAccount)
				.Do(saver.LoadState)
				.Do(delegate
				{
					_localizationState.Initialize();
				})
			select _localizationProvider.OnLocalizationChange).Do(delegate
		{
			_currencyFactory.MigrateCurrencies();
		}).Do(delegate
		{
			loadingProgress.Set(0.97f);
		}).SelectMany(_postLoadingState.PostLoading())
			.Subscribe(delegate
			{
				SetAsLoaded();
			}, delegate(Exception ex)
			{
				exceptionHandler.Handle(ex);
				throw ex.LogException();
			})
			.AddTo(disposables);
		connectableObservable.Connect().AddTo(disposables);
	}

	private SavedData PrepareState(SavedData arg)
	{
		if (arg != null)
		{
			MigrateFromOldToNewTutorial(arg.Data);
			MigrateToNewbalance(arg);
		}
		return arg;
	}

	private SavedData NewBalanceMigrateEmptyProgressAccount(SavedData savedData)
	{
		if (savedData?.Data == null)
		{
			_migrateToNewBalance.Migrate();
		}
		return savedData;
	}

	private static void MigrateFromOldToNewTutorial(List<Memento> arg)
	{
		if (arg != null && arg.OfType<StarShopItem.Memento>().Any((StarShopItem.Memento _memento) => _memento.ID == 105))
		{
			bool num = arg.OfType<StarShopItem.Memento>().Any((StarShopItem.Memento _memeto) => _memeto.ID == 201 && _memeto.State != 8);
			bool flag = arg.OfType<PlayerStats.Memento>().Any((PlayerStats.Memento _memento) => _memento.PaymentCount == 0);
			if (num && flag)
			{
				arg.Clear();
			}
		}
	}

	private void MigrateToNewbalance(SavedData savedData)
	{
		if (savedData.Data == null)
		{
			_migrateToNewBalance.Migrate();
			return;
		}
		MigrateToNewbalanceMemento migrateToNewbalanceMemento = null;
		bool flag = false;
		List<MigrateToNewbalanceMemento> list = savedData.Data.OfType<MigrateToNewbalanceMemento>().ToList();
		if (list.Any())
		{
			migrateToNewbalanceMemento = list?.First();
			flag = migrateToNewbalanceMemento?.WasTotalMigrated() ?? false;
		}
		if (!flag)
		{
			migrateToNewbalanceMemento?.Migrate();
			_migrateToNewBalance.Migrate();
			if (savedData.Data.OfType<StarShopItem.Memento>().Any((StarShopItem.Memento _memento) => _memento.ID == 205 && _memento.State != 8))
			{
				savedData.Data = new List<Memento>();
				savedData.Data.Add(_migrateToNewBalance.SaveState());
			}
		}
	}

	private void SetAsLoaded()
	{
		isUserDataLoaded.Value = true;
		saver.SetSuppressState(state: false);
		loadingProgress.Set(1f);
	}

	public void OnStartSaving()
	{
		OnStartSavingEvent.Execute();
	}

	public void SaveToLocal()
	{
		delayedSaveStream?.Dispose();
		if (!saver.SuppressSaving)
		{
			delayedSaveStream = (from _ in Observable.TimerFrame(1, FrameCountType.EndOfFrame)
				select PrepareSaveData()).Do(saveProviderFacade.SaveToLocal).Subscribe(delegate
			{
				_saveNotifier.NotifyLocalSave();
			});
		}
	}

	public void SaveToServer()
	{
		if (!saver.SuppressSaving)
		{
			SavedData data = PrepareSaveData();
			if (saveProviderFacade.SaveToServer(data))
			{
				_saveNotifier.NotifyServerSave();
			}
		}
	}

	private SavedData PrepareSaveData()
	{
		return new SavedData
		{
			UpdatedAt = _clock.GetTime().ConvertToUnixTimestamp(),
			Data = saver.GetState()
		};
	}

	public void Dispose()
	{
		disposables.Dispose();
		delayedSaveStream?.Dispose();
	}
}
