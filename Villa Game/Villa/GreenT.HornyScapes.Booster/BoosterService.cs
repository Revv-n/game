using System;
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
		_saver = saver;
		_storage = storage;
		_mapperManager = mapperManager;
	}

	public void Initialize()
	{
		_saver.Add(_storage);
		_storage.OnNew.SelectMany((Func<BoosterModel, IObservable<BoosterModel>>)WaitForTimer).Subscribe(delegate(BoosterModel model)
		{
			_storage.Remove(model);
			model.Bonus.Undo();
		}).AddTo(_compositeDisposable);
	}

	private IObservable<BoosterModel> WaitForTimer(BoosterModel model)
	{
		return from _ in model.Timer.OnTimeIsUp.Take(1)
			select model;
	}

	public void ApplyBooster(int id)
	{
		BoosterMapper mapper2 = _mapperManager.Collection.First((BoosterMapper mapper) => mapper.booster_id == id);
		_storage.Setup(mapper2);
	}

	public void Dispose()
	{
		_compositeDisposable?.Dispose();
	}
}
