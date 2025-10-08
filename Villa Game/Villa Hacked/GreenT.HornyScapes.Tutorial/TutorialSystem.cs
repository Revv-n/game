using System;
using System.Collections.Generic;
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

	public IObservable<TutorialGroupSteps> OnStartGroup => Observable.AsObservable<TutorialGroupSteps>((IObservable<TutorialGroupSteps>)onStartGroup);

	public IObservable<TutorialStep> OnCompleteStep => Observable.AsObservable<TutorialStep>((IObservable<TutorialStep>)onCompleteStep);

	public TutorialSystem(TutorialGroupManager tutorManager)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		this.tutorManager = tutorManager;
	}

	public void StartListen()
	{
		lockerStream.Clear();
		IObservable<TutorialGroupSteps> uncompletedGroupObservable = tutorManager.GetUncompletedGroupObservable();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<TutorialGroupSteps>(Observable.SelectMany<TutorialGroupSteps, TutorialGroupSteps>(uncompletedGroupObservable, (Func<TutorialGroupSteps, IObservable<TutorialGroupSteps>>)EmitOnGroupUnlock), (Action<TutorialGroupSteps>)RunGroup, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)lockerStream);
		ObservableExtensions.Subscribe<bool>(Observable.SelectMany<TutorialGroupSteps, bool>(uncompletedGroupObservable, (Func<TutorialGroupSteps, IObservable<bool>>)OnUpdateGroupStart), (Action<bool>)delegate(bool _value)
		{
			IsActive.Value = _value;
		});
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<TutorialStep>(Observable.Where<TutorialStep>(Observable.Merge<TutorialStep>(from _step in tutorManager.Collection.SelectMany((TutorialGroupSteps _group) => _group.Steps)
			select _step.OnUpdate), (Func<TutorialStep, bool>)((TutorialStep _step) => _step.IsComplete.Value)), (Action<TutorialStep>)onCompleteStep.OnNext), (ICollection<IDisposable>)lockerStream);
	}

	private IObservable<TutorialGroupSteps> EmitOnGroupUnlock(TutorialGroupSteps group)
	{
		return Observable.Select<bool, TutorialGroupSteps>(Observable.Take<bool>(Observable.Where<bool>(Observable.TakeUntil<bool, bool>((IObservable<bool>)group.Lockers.IsOpen, Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)group.IsCompleted, (Func<bool, bool>)((bool x) => x)), 1)), (Func<bool, bool>)((bool _isOpen) => _isOpen)), 1), (Func<bool, TutorialGroupSteps>)((bool _) => group));
	}

	private IObservable<bool> OnUpdateGroupStart(TutorialGroupSteps group)
	{
		return Observable.Select<TutorialGroupSteps, bool>(group.OnUpdate, (Func<TutorialGroupSteps, bool>)((TutorialGroupSteps _group) => _group.IsStarted.Value || tutorManager.Collection.Any((TutorialGroupSteps __group) => __group.IsStarted.Value)));
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
		CompositeDisposable obj = lockerStream;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
