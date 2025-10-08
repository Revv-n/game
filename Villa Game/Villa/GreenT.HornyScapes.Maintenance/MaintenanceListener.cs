using System;
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

	public IObservable<MaintenanceInfo> NeedResetClient => needResetClient.AsObservable();

	public IObservable<Response<ConfigurationInfo>> Ping => ping.AsObservable();

	public MaintenanceListener(GetConfigsVersionRequest getConfigsVersionRequest, int intervalTime, User user, GameStopSignal gameStopSignal)
	{
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
		IConnectableObservable<Response<ConfigurationInfo>> connectableObservable = GetPlatformState().Publish();
		connectableObservable.Where(IsMTTime).Subscribe(MaintenanceTime).AddTo(streams);
		connectableObservable.Connect();
		IConnectableObservable<Response<ConfigurationInfo>> connectableObservable2 = connectableObservable.Where((Response<ConfigurationInfo> _response) => !IsMTTime(_response)).Do(delegate(Response<ConfigurationInfo> _response)
		{
			gameConfigVersion = _response.Data.game_config_version;
			currentVersion = _response.Data.curr_version;
		}).SelectMany(InfiniteIntervalObservable())
			.StartWith(0L)
			.SelectMany(GetPlatformState())
			.Debug("MT")
			.SelectMany(GetPlatformState())
			.Catch(delegate(Exception ex)
			{
				throw ex.LogException();
			})
			.OnErrorRetry<Response<ConfigurationInfo>, Exception>(delegate
			{
				ping.OnNext(null);
			}, intervalTime)
			.Publish();
		connectableObservable2.Where(NeedUpdateClient).Subscribe(UpdateClient).AddTo(streams);
		connectableObservable2.Where(IsOldConfigs).Subscribe(UpdateConfigs).AddTo(streams);
		connectableObservable2.Where(IsMTTime).Subscribe(MaintenanceTime).AddTo(streams);
		connectableObservable2.Connect();
	}

	private IObservable<long> InfiniteIntervalObservable()
	{
		return Observable.Interval(intervalTime, Scheduler.MainThreadIgnoreTimeScale).Repeat();
	}

	private IObservable<Response<ConfigurationInfo>> GetPlatformState()
	{
		return getConfigsVersionRequest.Get().Do(ping.OnNext);
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
