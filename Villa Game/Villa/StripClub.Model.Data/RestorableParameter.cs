using System;
using UnityEngine;

namespace StripClub.Model.Data;

[Serializable]
public class RestorableParameter
{
	[Tooltip("Shows how often value restores")]
	[SerializeField]
	private float restorePeriodInSeconds = 1f;

	[Tooltip("Defines how often engine will check if time to restore energy is already come")]
	[SerializeField]
	private float checkFrequencyInSeconds = 1f;

	[field: SerializeField]
	public int Basic { get; private set; }

	[field: SerializeField]
	public int Max { get; private set; }

	[field: SerializeField]
	public int RestoreAmountPerTick { get; internal set; }

	public TimeSpan RestorePeriod => TimeSpan.FromSeconds(restorePeriodInSeconds);

	public TimeSpan CheckFrequency => TimeSpan.FromSeconds(checkFrequencyInSeconds);
}
