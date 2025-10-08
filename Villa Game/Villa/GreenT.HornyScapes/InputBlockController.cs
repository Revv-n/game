using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.HornyScapes;

public class InputBlockController : IInputBlockerController, IDisposable
{
	private List<IInputBlocker> inputBlockers = new List<IInputBlocker>();

	private CompositeDisposable trackStreams = new CompositeDisposable();

	private Dictionary<IInputBlocker, IDisposable> disposablesDict = new Dictionary<IInputBlocker, IDisposable>();

	protected Subject<IInputBlocker> onUpdateBlocker = new Subject<IInputBlocker>();

	public bool ClickBlock => inputBlockers.Any((IInputBlocker block) => block.IsLaunched);

	public IObservable<IInputBlocker> OnUpdate => onUpdateBlocker.AsObservable();

	public void AddBlocker(IInputBlocker inputBlocker)
	{
		inputBlockers.Add(inputBlocker);
		IDisposable value = inputBlocker.OnUpdate.Subscribe(onUpdateBlocker.OnNext).AddTo(trackStreams);
		disposablesDict.Add(inputBlocker, value);
	}

	public void Dispose()
	{
		trackStreams.Dispose();
		onUpdateBlocker.OnCompleted();
		onUpdateBlocker.Dispose();
	}

	public void Remove(IInputBlocker inputBlocker)
	{
		disposablesDict[inputBlocker].Dispose();
		disposablesDict.Remove(inputBlocker);
		inputBlockers.Remove(inputBlocker);
	}
}
