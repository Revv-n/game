using System;
using UniRx;

namespace GreenT.HornyScapes.Tutorial;

public abstract class BaseTutorialComponent<KStep> : IDisposable where KStep : TutorialStepSO
{
	protected KStep stepSO;

	protected Subject<BaseTutorialComponent<KStep>> onActivate = (Subject<BaseTutorialComponent<KStep>>)(object)new Subject<BaseTutorialComponent<BaseTutorialComponent<KStep>>>();

	protected CompositeDisposable stepUpdateStream = new CompositeDisposable();

	public ReactiveProperty<bool> IsInited = new ReactiveProperty<bool>();

	protected TutorialGroupManager groupManager;

	public int StepID { get; private set; } = -1;


	public int GroupID { get; private set; } = -1;


	public ITutorialModel StepModel { get; protected set; }

	public IObservable<BaseTutorialComponent<KStep>> OnActivate => Observable.AsObservable<BaseTutorialComponent<KStep>>((IObservable<BaseTutorialComponent<KStep>>)onActivate);

	public bool IsActive { get; protected set; }

	public bool IsLight { get; protected set; }

	public bool BlockScreen { get; protected set; }

	protected BaseTutorialComponent(KStep stepSO, TutorialGroupManager groupManager)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		this.stepSO = stepSO;
		this.groupManager = groupManager;
	}

	protected abstract void GetStep(int groupId, int stepId);

	protected virtual void InnerInit(int groupID, int stepID, bool blockScreen, bool isLight)
	{
		if (StepModel != null && !StepModel.IsComplete.Value)
		{
			InitFields();
			if (!IsActive)
			{
				stepUpdateStream.Add(SubscribeOnStepStart());
			}
			stepUpdateStream.Add(SubscribeOnStepComplete());
			IsInited.Value = true;
		}
		void InitFields()
		{
			StepID = stepID;
			GroupID = groupID;
			IsLight = isLight;
			BlockScreen = blockScreen;
			IsActive = StepModel.IsInProgressStep.Value;
		}
	}

	private IDisposable SubscribeOnStepStart()
	{
		return ObservableExtensions.Subscribe<bool>(Observable.Where<bool>(Observable.Catch<bool, Exception>(Observable.Skip<bool>((IObservable<bool>)StepModel.IsInProgressStep, 1), (Func<Exception, IObservable<bool>>)delegate(Exception innerEx)
		{
			throw innerEx.SendException($"{GetType().Name}: Start {StepID} is broken");
		}), (Func<bool, bool>)((bool _val) => _val)), (Action<bool>)delegate
		{
			OnStart();
		}, (Action<Exception>)delegate(Exception ex)
		{
			throw ex;
		});
	}

	private IDisposable SubscribeOnStepComplete()
	{
		return ObservableExtensions.Subscribe<bool>(Observable.Where<bool>(Observable.Catch<bool, Exception>(Observable.Where<bool>((IObservable<bool>)StepModel.IsComplete, (Func<bool, bool>)((bool _value) => _value)), (Func<Exception, IObservable<bool>>)delegate(Exception innerEx)
		{
			throw innerEx.SendException($"{GetType().Name}: Complete {StepID} is broken");
		}), (Func<bool, bool>)((bool _val) => _val)), (Action<bool>)delegate
		{
			Complete();
		}, (Action<Exception>)delegate(Exception ex)
		{
			throw ex;
		});
	}

	protected virtual void OnStart()
	{
		IsActive = true;
		((Subject<BaseTutorialComponent<BaseTutorialComponent<KStep>>>)(object)onActivate).OnNext((BaseTutorialComponent<BaseTutorialComponent<KStep>>)(object)this);
	}

	public void CompleteStep()
	{
		StepModel.SetComplete(state: true);
		IsActive = false;
	}

	protected virtual void Complete()
	{
		Dispose();
	}

	public void Dispose()
	{
		stepUpdateStream.Dispose();
	}
}
