using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class SpeedUpStrategy : MonoBehaviour
{
	[Space]
	[Header("Animation")]
	[SerializeField]
	private float radius;

	[SerializeField]
	private int amountDrig;

	[SerializeField]
	private float timeDrig;

	[SerializeField]
	private float deltaPositionY;

	[SerializeField]
	private float endSize;

	private ModifyController modifyController;

	private GameItemController Field => Controller<GameItemController>.Instance;

	public void PlaySpeedUp(IEnumerable<GameItem> withActiveTimer, float value)
	{
		foreach (GIBox.ClickSpawn item in withActiveTimer.Select((GameItem x) => x.GetBox<GIBox.ClickSpawn>()).ToList())
		{
			RefTimer refTimer = new RefTimer(item.TweenTimer.Timer.TotalTime, item.TweenTimer.Timer.StartTime.AddSeconds(0f - value));
			int num = refTimer.RemovePeriods();
			int num2 = modifyController.CalcMaxAmount(item);
			item.AddAmount(item.Config.RestoreAmount * num, num2);
			item.Data.TimerActive = item.Data.Amount < num2;
			if (item.Data.TimerActive)
			{
				Debug.Log($"{item.Parent.Key} in {item.Parent.Coordinates} charges {item.Data.Amount}/{num2} time left {item.TweenTimer.Timer.TimeLeft}");
				item.SetTweenTimer(refTimer);
			}
			else
			{
				item.StopTimer();
				Debug.Log($"{item.Parent.Key} in {item.Parent.Coordinates} charges {item.Data.Amount}/{num2}");
			}
		}
	}

	[ContextMenu("Play_SpeedUp")]
	public void PlaySpeedUp()
	{
		List<GIBox.ClickSpawn> withActiveTimer = GetActiveTimerModulesOnField();
		Sequence sequence = DOTween.Sequence();
		List<Vector3> listPosition = new List<Vector3>();
		foreach (GIBox.ClickSpawn item in withActiveTimer)
		{
			Sequence sequence2 = DOTween.Sequence();
			listPosition.Add(item.Parent.transform.localPosition);
			for (int i = 0; i < amountDrig; i++)
			{
				Transform transform = item.Parent.transform;
				float y = Random.Range(transform.localPosition.y, transform.localPosition.y + radius * deltaPositionY);
				float x = Random.Range(transform.localPosition.x - radius, transform.localPosition.x + radius);
				sequence2.Append(item.Parent.transform.DOLocalMove(new Vector3(x, y, transform.localPosition.z), timeDrig));
			}
			sequence.Join(item.Parent.transform.DOScale(endSize, timeDrig * (float)amountDrig));
			sequence.Join(sequence2);
			item.Parent.AppendOuterTween(sequence);
			sequence.OnComplete(delegate
			{
				for (int j = 0; j < withActiveTimer.Count; j++)
				{
					withActiveTimer[j].Parent.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InBack);
					withActiveTimer[j].Parent.transform.DOLocalMove(listPosition[j], 0.3f).SetEase(Ease.InBack);
				}
			});
		}
	}

	public List<GIBox.ClickSpawn> GetActiveTimerModulesOnField()
	{
		List<GIBox.ClickSpawn> list = new List<GIBox.ClickSpawn>();
		if ((bool)Field)
		{
			IEnumerable<GIBox.ClickSpawn> collection = from x in Field.CurrentField.Field
				where x != null && x.Data.HasModule(GIModuleType.ClickSpawn)
				select x.GetBox<GIBox.ClickSpawn>() into x
				where x.Data.TimerActive
				select x;
			list.AddRange(collection);
		}
		return list;
	}

	private void OnDestroy()
	{
		GetActiveTimerModulesOnField().ForEach(delegate(GIBox.ClickSpawn x)
		{
			x.StopTimer();
		});
	}
}
