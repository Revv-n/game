using Merge;
using UnityEngine;

public class AbstractTile : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer sr;

	public Point Coordinates { get; set; }

	public Sprite Background
	{
		get
		{
			return sr.sprite;
		}
		set
		{
			sr.sprite = value;
		}
	}

	public bool IsBackEnabled
	{
		get
		{
			return sr.enabled;
		}
		set
		{
			sr.enabled = value;
		}
	}

	public SpriteRenderer SpriteRen
	{
		get
		{
			return sr;
		}
		set
		{
			sr = value;
		}
	}

	public BackTile BackSetting { get; private set; }

	public void SetBackSetting(BackTile set)
	{
		BackSetting = set;
	}
}
