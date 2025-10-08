using System;
using System.Linq;
using UniRx;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialSystem : IDisposable
{
	private readonly Subject<TutorialGroupSteps> onStartGroup = new Subject<TutorialGroupSteps>();

	private readonly Subject<TutorialStep> onCompleteStep = new Subject<TutorialStep>();

	private readonly TutorialGroupManager tutorManager;

	private readonly CompositeDisposable lockerStream = new CompositeDisposable();

	public readonly ReactiveProperty<bool> IsActive = new ReactiveProperty<bool>();

	public IObservable<TutorialGroupSteps> OnStartGroup => onStartGroup.AsObservable();

	public IObservable<TutorialStep> OnCompleteStep => onCompleteStep.AsObservable();

	public TutorialSystem(TutorialGroupManager tutorManager)
	{
		this.tutorManager = tutorManager;
	}

	public void StartListen()
	{
		lockerStream.Clear();
		IObservable<TutorialGroupSteps> uncompletedGroupObservable = tutorManager.GetUncompletedGroupObservable();
		uncompletedGroupObservable.SelectMany((Func<TutorialGroupSteps, IObservable<TutorialGroupSteps>>)EmitOnGroupUnlock).Subscribe(RunGroup, delegate(Exception ex)
		{
			ex.LogException();
		}).AddTo(lockerStream);
		uncompletedGroupObservable.SelectMany((Func<TutorialGroupSteps, IObservable<bool>>)OnUpdateGroupStart).Subscribe(delegate(bool _value)
		{
			IsActive.Value = _value;
		});
		(from _step in (from _step in tutorManager.Collection.SelectMany((TutorialGroupSteps _group) => _group.Steps)
				select _step.OnUpdate).Merge()
			where _step.IsComplete.Value
			select _step).Subscribe(onCompleteStep.OnNext).AddTo(lockerStream);
	}

	private IObservable<TutorialGroupSteps> EmitOnGroupUnlock(TutorialGroupSteps group)
	{
		return from _ in (from _isOpen in @group.Lockers.IsOpen.TakeUntil(@group.IsCompleted.Where((bool x) => x).Take(1))
				where _isOpen
				select _isOpen).Take(1)
			select @group;
	}

	private IObservable<bool> OnUpdateGroupStart(TutorialGroupSteps group)
	{
		return group.OnUpdate.Select((TutorialGroupSteps _group) => _group.IsStarted.Value || tutorManager.Collection.Any((TutorialGroupSteps __group) => __group.IsStarted.Value));
	}

	private void RunGroup(TutorialGroupSteps group)
	{
		IsActive.Value = true;
		group.Play();
		onStartGroup.OnNext(group);
	}

	public void Dispose()
	{
		onStartGroup?.Dispose();
		onCompleteStep?.Dispose();
		lockerStream?.Dispose();
	}
}
