using System;
using UniRx;

namespace GreenT.HornyScapes.Tutorial;

public abstract class BaseTutorialComponent<KStep> : IDisposable where KStep : TutorialStepSO
{
	protected KStep stepSO;

	protected Subject<BaseTutorialComponent<KStep>> onActivate = new Subject<BaseTutorialComponent<KStep>>();

	protected CompositeDisposable stepUpdateStream = new CompositeDisposable();

	public ReactiveProperty<bool> IsInited = new ReactiveProperty<bool>();

	protected TutorialGroupManager groupManager;

	public int StepID { get; private set; } = -1;


	public int GroupID { get; private set; } = -1;


	public ITutorialModel StepModel { get; protected set; }

	public IObservable<BaseTutorialComponent<KStep>> OnActivate => onActivate.AsObservable();

	public bool IsActive { get; protected set; }

	public bool IsLight { get; protected set; }

	public bool BlockScreen { get; protected set; }

	protected BaseTutorialComponent(KStep stepSO, TutorialGroupManager groupManager)
	{
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
		return (from _val in StepModel.IsInProgressStep.Skip(1).Catch(delegate(Exception innerEx)
			{
				throw innerEx.SendException($"{GetType().Name}: Start {StepID} is broken");
			})
			where _val
			select _val).Subscribe(delegate
		{
			OnStart();
		}, delegate(Exception ex)
		{
			throw ex;
		});
	}

	private IDisposable SubscribeOnStepComplete()
	{
		return (from _val in StepModel.IsComplete.Where((bool _value) => _value).Catch(delegate(Exception innerEx)
			{
				throw innerEx.SendException($"{GetType().Name}: Complete {StepID} is broken");
			})
			where _val
			select _val).Subscribe(delegate
		{
			Complete();
		}, delegate(Exception ex)
		{
			throw ex;
		});
	}

	protected virtual void OnStart()
	{
		IsActive = true;
		onActivate.OnNext(this);
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
