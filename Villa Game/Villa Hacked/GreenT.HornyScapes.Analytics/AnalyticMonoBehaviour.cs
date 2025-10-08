using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public abstract class AnalyticMonoBehaviour : MonoBehaviour
{
	[Inject]
	protected IAmplitudeSender<AmplitudeEvent> amplitude;
}
