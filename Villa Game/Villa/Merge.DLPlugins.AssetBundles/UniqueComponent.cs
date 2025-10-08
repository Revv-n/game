using System.Collections.Generic;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

internal class UniqueComponent : MonoBehaviour
{
	[SerializeField]
	private string m_UniquenessTag;

	[SerializeField]
	private bool m_EnableLog;

	private static HashSet<string> s_uniquenessTags = new HashSet<string>();

	private bool m_Destroyed { get; set; }

	public string uniquenessTag => m_UniquenessTag;

	private void Awake()
	{
		DeleteIfNotUnique();
		AddTag(uniquenessTag);
	}

	private void OnDestroy()
	{
		RemoveTag(uniquenessTag);
	}

	public void SetTag(string tag)
	{
		RemoveTag(m_UniquenessTag);
		m_UniquenessTag = tag;
		DeleteIfNotUnique();
		AddTag(tag);
		Log("Set tag " + tag);
	}

	private void DeleteIfNotUnique()
	{
		if (s_uniquenessTags.Contains(uniquenessTag))
		{
			m_Destroyed = true;
			Object.DestroyImmediate(base.gameObject);
			Log("Destroyed");
		}
	}

	private void AddTag(string tag)
	{
		if (!m_Destroyed && !s_uniquenessTags.Contains(tag))
		{
			s_uniquenessTags.Add(tag);
			Log("Add tag " + tag);
		}
	}

	private void RemoveTag(string tag)
	{
		if (!m_Destroyed && s_uniquenessTags.Contains(tag))
		{
			s_uniquenessTags.Remove(tag);
			Log("Remove tag " + tag);
		}
	}

	private void Log(string message)
	{
		if (m_EnableLog)
		{
			Debug.Log("Uniqie component: " + message);
		}
	}
}
