using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Net;
using GreenT.Settings.Data;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Maintenance;

public class MaintenanceListener : IDisposable
{
	private readonly Subject<MaintenanceInfo> needResetClient = new Subject<MaintenanceInfo>();

	private readonly Subject<Response<ConfigurationInfo>> ping = new Subject<Response<ConfigurationInfo>>();

	private readonly GetConfigsVersionRequest getConfigsVersionRequest;

	private readonly TimeSpan intervalTime;

	private readonly CompositeDisposable streams = new CompositeDisposable();

	private string currentVersion = string.Empty;

	private string gameConfigVersion = string.Empty;

	private GameStopSignal _gameStopSignal;

	private User user;

	public IObservable<MaintenanceInfo> NeedResetClient => Observable.AsObservable<MaintenanceInfo>((IObservable<MaintenanceInfo>)needResetClient);

	public IObservable<Response<ConfigurationInfo>> Ping => Observable.AsObservable<Response<ConfigurationInfo>>((IObservable<Response<ConfigurationInfo>>)ping);

	public MaintenanceListener(GetConfigsVersionRequest getConfigsVersionRequest, int intervalTime, User user, GameStopSignal gameStopSignal)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		this.getConfigsVersionRequest = getConfigsVersionRequest;
		this.intervalTime = TimeSpan.FromSeconds(intervalTime);
		this.user = user;
		_gameStopSignal = gameStopSignal;
	}

	public void Track()
	{
		TrackMaintenance();
	}

	private void TrackMaintenance()
	{
		IConnectableObservable<Response<ConfigurationInfo>> obj = Observable.Publish<Response<ConfigurationInfo>>(GetPlatformState());
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response<ConfigurationInfo>>(Observable.Where<Response<ConfigurationInfo>>((IObservable<Response<ConfigurationInfo>>)obj, (Func<Response<ConfigurationInfo>, bool>)IsMTTime), (Action<Response<ConfigurationInfo>>)MaintenanceTime), (ICollection<IDisposable>)streams);
		obj.Connect();
		IConnectableObservable<Response<ConfigurationInfo>> obj2 = Observable.Publish<Response<ConfigurationInfo>>(Observable.OnErrorRetry<Response<ConfigurationInfo>, Exception>(Observable.Catch<Response<ConfigurationInfo>, Exception>(Observable.SelectMany<Response<ConfigurationInfo>, Response<ConfigurationInfo>>(Observable.SelectMany<long, Response<ConfigurationInfo>>(Observable.StartWith<long>(Observable.SelectMany<Response<ConfigurationInfo>, long>(Observable.Do<Response<ConfigurationInfo>>(Observable.Where<Response<ConfigurationInfo>>((IObservable<Response<ConfigurationInfo>>)obj, (Func<Response<ConfigurationInfo>, bool>)((Response<ConfigurationInfo> _response) => !IsMTTime(_response))), (Action<Response<ConfigurationInfo>>)delegate(Response<ConfigurationInfo> _response)
		{
			gameConfigVersion = _response.Data.game_config_version;
			currentVersion = _response.Data.curr_version;
		}), InfiniteIntervalObservable()), 0L), GetPlatformState()).Debug("MT"), GetPlatformState()), (Func<Exception, IObservable<Response<ConfigurationInfo>>>)delegate(Exception ex)
		{
			throw ex.LogException();
		}), (Action<Exception>)delegate
		{
			ping.OnNext((Response<ConfigurationInfo>)null);
		}, intervalTime));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response<ConfigurationInfo>>(Observable.Where<Response<ConfigurationInfo>>((IObservable<Response<ConfigurationInfo>>)obj2, (Func<Response<ConfigurationInfo>, bool>)NeedUpdateClient), (Action<Response<ConfigurationInfo>>)UpdateClient), (ICollection<IDisposable>)streams);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response<ConfigurationInfo>>(Observable.Where<Response<ConfigurationInfo>>((IObservable<Response<ConfigurationInfo>>)obj2, (Func<Response<ConfigurationInfo>, bool>)IsOldConfigs), (Action<Response<ConfigurationInfo>>)UpdateConfigs), (ICollection<IDisposable>)streams);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response<ConfigurationInfo>>(Observable.Where<Response<ConfigurationInfo>>((IObservable<Response<ConfigurationInfo>>)obj2, (Func<Response<ConfigurationInfo>, bool>)IsMTTime), (Action<Response<ConfigurationInfo>>)MaintenanceTime), (ICollection<IDisposable>)streams);
		obj2.Connect();
	}

	private IObservable<long> InfiniteIntervalObservable()
	{
		return Observable.Repeat<long>(Observable.Interval(intervalTime, Scheduler.MainThreadIgnoreTimeScale));
	}

	private IObservable<Response<ConfigurationInfo>> GetPlatformState()
	{
		return Observable.Do<Response<ConfigurationInfo>>(getConfigsVersionRequest.Get(), (Action<Response<ConfigurationInfo>>)ping.OnNext);
	}

	private bool NeedUpdateClient(Response<ConfigurationInfo> response)
	{
		return !response.Data.curr_version_client.Equals(Application.version);
	}

	private bool IsOldConfigs(Response<ConfigurationInfo> _response)
	{
		return !_response.Data.game_config_version.Equals(gameConfigVersion);
	}

	private bool IsMTTime(Response<ConfigurationInfo> _response)
	{
		if (!_response.Data.maintenance)
		{
			return _response.Data.players_mt?.Contains(user.PlayerID) ?? false;
		}
		return true;
	}

	private void UpdateClient(Response<ConfigurationInfo> response)
	{
		try
		{
			_gameStopSignal.Stop();
			StopTrack();
			needResetClient.OnNext(CreateUpdateClient());
		}
		catch (Exception exception)
		{
			exception.LogException();
		}
	}

	private void UpdateConfigs(Response<ConfigurationInfo> response)
	{
		try
		{
			_gameStopSignal.Stop();
			StopTrack();
			needResetClient.OnNext(CreateConfigIsOld());
		}
		catch (Exception exception)
		{
			exception.LogException();
		}
	}

	private void MaintenanceTime(Response<ConfigurationInfo> response)
	{
		try
		{
			_gameStopSignal.Stop();
			StopTrack();
			needResetClient.OnNext(CreateMaintenanceTime());
		}
		catch (Exception exception)
		{
			exception.LogException();
		}
	}

	private void StopTrack()
	{
		streams.Clear();
	}

	public void Dispose()
	{
		streams.Dispose();
	}

	private MaintenanceInfo CreateUpdateClient()
	{
		return new MaintenanceInfo(needUpdateClient: true, configIsOld: false, maintenanceTime: false);
	}

	private MaintenanceInfo CreateConfigIsOld()
	{
		return new MaintenanceInfo(needUpdateClient: false, configIsOld: true, maintenanceTime: false);
	}

	private MaintenanceInfo CreateMaintenanceTime()
	{
		return new MaintenanceInfo(needUpdateClient: false, configIsOld: false, maintenanceTime: true);
	}
}
