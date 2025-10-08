using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Booster;

public class BoosterService : IInitializable, IDisposable
{
	private readonly BoosterStorage _storage;

	private readonly BoosterMapperManager _mapperManager;

	private readonly ISaver _saver;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public BoosterService(BoosterMapperManager mapperManager, BoosterStorage storage, ISaver saver)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_saver = saver;
		_storage = storage;
		_mapperManager = mapperManager;
	}

	public void Initialize()
	{
		_saver.Add(_storage);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<BoosterModel>(Observable.SelectMany<BoosterModel, BoosterModel>(_storage.OnNew, (Func<BoosterModel, IObservable<BoosterModel>>)WaitForTimer), (Action<BoosterModel>)delegate(BoosterModel model)
		{
			_storage.Remove(model);
			model.Bonus.Undo();
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	private IObservable<BoosterModel> WaitForTimer(BoosterModel model)
	{
		return Observable.Select<GenericTimer, BoosterModel>(Observable.Take<GenericTimer>(model.Timer.OnTimeIsUp, 1), (Func<GenericTimer, BoosterModel>)((GenericTimer _) => model));
	}

	public void ApplyBooster(int id)
	{
		BoosterMapper mapper2 = _mapperManager.Collection.First((BoosterMapper mapper) => mapper.booster_id == id);
		_storage.Setup(mapper2);
	}

	public void Dispose()
	{
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Dispose();
		}
	}
}
