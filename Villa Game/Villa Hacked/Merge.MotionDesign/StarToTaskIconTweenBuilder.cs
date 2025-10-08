using DG.Tweening;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Tasks;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.HornyScapes.UI;
using StripClub.Model;
using StripClub.Model.Quest;
using UnityEngine;
using Zenject;

namespace Merge.MotionDesign;

public class StarToTaskIconTweenBuilder : MonoBehaviour
{
	public SpriteBezierAnimate BezierAnimate;

	private MainUiWindow mainUiWindow;

	private TaskRewardsMainFactory mainFactory;

	private ICameraChanger cameraChanger;

	private MergeTaskViewManager mergeTaskViewManager;

	[Inject]
	private void InnerInit(TaskRewardsMainFactory mainFactory, ICameraChanger cameraChanger, MergeTaskViewManager mergeTaskViewManager)
	{
		this.mainFactory = mainFactory;
		this.cameraChanger = cameraChanger;
		this.mergeTaskViewManager = mergeTaskViewManager;
	}

	public void FlyStar((Task task, GameItem gameItem) group)
	{
		if (group.task.State != StateType.Complete && mergeTaskViewManager.TryGetItem(group.task, out var view))
		{
			FlyingCurrency flyingCurrency;
			if (group.task.Reward is CurrencyLinkedContent currencyLinkedContent)
			{
				flyingCurrency = mainFactory.Create(currencyLinkedContent.Currency);
			}
			else
			{
				flyingCurrency = mainFactory.Create(CurrencyType.Star);
			}
			Vector3 onCanvasPosition = GetOnCanvasPosition(group.gameItem.transform.position);
			Vector3 position = view.TaskViewStateMachine.RewardState.Star.transform.position;
			BuildBezierTween(flyingCurrency.transform, onCanvasPosition, position).OnComplete(delegate
			{
				flyingCurrency.Display(display: false);
			});
		}
	}

	public Vector3 GetOnCanvasPosition(Vector3 objectTransformPosition)
	{
		return RectTransformUtility.WorldToScreenPoint(cameraChanger.MergeCamera, objectTransformPosition);
	}

	private Vector3 GetScreenTargetPosition(Vector3 position)
	{
		return cameraChanger.MergeCamera.ScreenToWorldPoint(position);
	}

	public Tween BuildBezierTween(Transform flyingObject, Vector3 from, Vector3 to)
	{
		return DOTween.Sequence().Join(BezierAnimate.Launch(from, to, flyingObject));
	}
}
