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

	private readonly ReactiveProperty<bool> isUserDataLoaded = new ReactiveProperty<bool>(false);

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

	public IReadOnlyReactiveProperty<bool> IsUserDataLoaded => (IReadOnlyReactiveProperty<bool>)(object)isUserDataLoaded;

	public SaveController(ISaver saver, User user, SaveProviderFacade saveProviderFacade, GameStarter gameStarter, RelativeProgress loadingProgress, IExceptionHandler exceptionHandler, IClock clock, PostLoadingState postLoadingState, InitializeState initializeState, SimpleCurrencyFactory currencyFactory, PlayerPaymentsStats playerPaymentStats, MigrateToNewBalance migrateToNewBalance, LocalizationProvider localizationProvider, LocalizationState localizationState, SaveNotifier saveNotifier, GameStopSignal gameStopSignal)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
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
		IObservable<bool> observable = Observable.CombineLatest<bool, bool, bool>((IObservable<bool>)isGameDataLoaded, (IObservable<bool>)isGameActive, (Func<bool, bool, bool>)((bool y, bool z) => y && !z));
		IConnectableObservable<User> obj = Observable.Publish<User>(Observable.Select<(User, bool), User>(Observable.Do<(User, bool)>(Observable.Where<(User, bool)>(Observable.CombineLatest<User, bool, (User, bool)>(Observable.TakeUntil<User, bool>((IObservable<User>)user.OnLogin, Observable.Where<bool>((IObservable<bool>)_gameStopSignal.IsStopped, (Func<bool, bool>)((bool x) => x))), observable, (Func<User, bool, (User, bool)>)((User x, bool y) => (user: x, readyToInitialize: y))), (Func<(User, bool), bool>)(((User user, bool readyToInitialize) _pair) => _pair.readyToInitialize)), (Action<(User, bool)>)delegate
		{
			isUserDataLoaded.Value = false;
			saver.SetSuppressState(state: true);
		}), (Func<(User, bool), User>)(((User user, bool readyToInitialize) _pair) => _pair.user)));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IObservable<LocalizationDictionary>>(Observable.Select<User, IObservable<LocalizationDictionary>>(Observable.Do<User>(Observable.TakeUntil<User, bool>(Observable.Where<User>((IObservable<User>)obj, (Func<User, bool>)((User _user) => _user.Type.Equals(User.State.Unknown))), Observable.Where<bool>((IObservable<bool>)_gameStopSignal.IsStopped, (Func<bool, bool>)((bool x) => x))), (Action<User>)delegate
		{
			_localizationState.Initialize();
		}), (Func<User, IObservable<LocalizationDictionary>>)((User _) => _localizationProvider.OnLocalizationChange)).Debug("Initialize game for UNKNOWN user"), (Action<IObservable<LocalizationDictionary>>)delegate
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
		}), (ICollection<IDisposable>)disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.SelectMany<IObservable<LocalizationDictionary>, Unit>(Observable.Do<IObservable<LocalizationDictionary>>(Observable.Do<IObservable<LocalizationDictionary>>(Observable.Select<SavedData, IObservable<LocalizationDictionary>>(Observable.Do<SavedData>(Observable.Do<SavedData>(Observable.Select<SavedData, SavedData>(Observable.Select<SavedData, SavedData>(Observable.SelectMany<User, SavedData>(Observable.Do<User>(Observable.TakeUntil<User, bool>(Observable.Where<User>((IObservable<User>)obj, (Func<User, bool>)((User _user) => !_user.Type.Equals(User.State.Unknown))), Observable.Where<bool>((IObservable<bool>)_gameStopSignal.IsStopped, (Func<bool, bool>)((bool x) => x))), (Action<User>)delegate
		{
			_initializeState.InitializeGameStructures();
		}), (Func<User, IObservable<SavedData>>)((User _) => saveProviderFacade.Deserialize<SavedData>())), (Func<SavedData, SavedData>)PrepareState), (Func<SavedData, SavedData>)NewBalanceMigrateEmptyProgressAccount), (Action<SavedData>)saver.LoadState), (Action<SavedData>)delegate
		{
			_localizationState.Initialize();
		}), (Func<SavedData, IObservable<LocalizationDictionary>>)((SavedData _) => _localizationProvider.OnLocalizationChange)), (Action<IObservable<LocalizationDictionary>>)delegate
		{
			_currencyFactory.MigrateCurrencies();
		}), (Action<IObservable<LocalizationDictionary>>)delegate
		{
			loadingProgress.Set(0.97f);
		}), _postLoadingState.PostLoading()), (Action<Unit>)delegate
		{
			SetAsLoaded();
		}, (Action<Exception>)delegate(Exception ex)
		{
			exceptionHandler.Handle(ex);
			throw ex.LogException();
		}), (ICollection<IDisposable>)disposables);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)disposables);
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
			delayedSaveStream = ObservableExtensions.Subscribe<SavedData>(Observable.Do<SavedData>(Observable.Select<long, SavedData>(Observable.TimerFrame(1, (FrameCountType)2), (Func<long, SavedData>)((long _) => PrepareSaveData())), (Action<SavedData>)saveProviderFacade.SaveToLocal), (Action<SavedData>)delegate
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
