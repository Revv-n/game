using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DialogField : MonoBehaviour
{
	public struct StringPart
	{
		public List<string> Tags { get; private set; }

		public string Normalized { get; private set; }

		public int LengthNormalized => Normalized.Length;

		public int LengthTagged => GetFullString().Length;

		public StringPart(List<string> tags, string original)
		{
			Tags = tags;
			Normalized = original;
		}

		public StringPart(string original)
		{
			Tags = new List<string>();
			Normalized = original;
		}

		public string GetFullString()
		{
			return GetSubstring(Normalized.Length);
		}

		public string GetSubstring(int count)
		{
			string text = "";
			for (int i = 0; i < Tags.Count; i++)
			{
				text = text + "<" + Tags[i] + ">";
			}
			text += Normalized.Substring(0, count);
			for (int num = Tags.Count - 1; num >= 0; num--)
			{
				text = text + "</" + Tags[num] + ">";
			}
			return text;
		}

		public bool GetTagsCount(string original)
		{
			if (original.Contains("<") && original.Contains("</"))
			{
				return original.Contains(">");
			}
			return false;
		}
	}

	public struct StringTagInfo
	{
		public string Original { get; private set; }

		public string Normalized { get; private set; }

		public List<StringPart> Parts { get; private set; }

		public string GetSubstring(int length)
		{
			string text = "";
			int num = length;
			for (int i = 0; i < Parts.Count; i++)
			{
				int lengthNormalized = Parts[i].LengthNormalized;
				if (num < lengthNormalized)
				{
					return text + Parts[i].GetSubstring(num);
				}
				num -= lengthNormalized;
				text += Parts[i].GetFullString();
			}
			return text;
		}

		public StringTagInfo(string original)
		{
			Original = original;
			int position = 0;
			Parts = new List<StringPart>();
			Normalized = "";
			while (position < original.Length)
			{
				StringPart item = GetNextPart();
				Parts.Add(item);
				Normalized += item.Normalized;
			}
			StringPart GetNextPart()
			{
				if (original[position] != '<')
				{
					return GetNotTaggedPart();
				}
				return GetTaggedPart();
				StringPart GetNotTaggedPart()
				{
					string text3 = "";
					for (int i = position; i < original.Length && original[i] != '<'; i++)
					{
						text3 += original[i];
						position++;
					}
					return new StringPart(text3);
				}
				StringPart GetTaggedPart()
				{
					List<string> tags = new List<string>();
					int completedTags = 0;
					string text = "";
					while (position < original.Length)
					{
						if (original[position] == '<')
						{
							OperateTag();
							if (completedTags == 0)
							{
								return new StringPart(tags, text);
							}
						}
						else
						{
							text += original[position];
							position++;
						}
					}
					return new StringPart(tags, text);
					void OperateTag()
					{
						string text2 = "";
						bool flag = original[position + 1] == '/';
						completedTags += ((!flag) ? 1 : (-1));
						position += ((!flag) ? 1 : 2);
						for (; position < original.Length; position++)
						{
							if (original[position] == '>')
							{
								position++;
								if (!flag)
								{
									tags.Add(text2);
								}
								break;
							}
							text2 += original[position];
						}
					}
				}
			}
		}
	}

	[SerializeField]
	private TextMeshProUGUI label;

	[SerializeField]
	private float timeForChar = 0.05f;

	[SerializeField]
	private float afterTime = 5f;

	private string targetString;

	private Tween scrumbleTween;

	private Tween waitTween;

	private StringTagInfo taggedString;

	private int showdChars;

	private bool isScrumbling;

	private Action completeCallback;

	public Tween DoScrumble(string str, Action completeCallback)
	{
		scrumbleTween?.Kill();
		this.completeCallback = completeCallback;
		targetString = str;
		showdChars = 0;
		taggedString = new StringTagInfo(str);
		label.text = "";
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			isScrumbling = true;
		});
		sequence.Append(DoText(str));
		sequence.AppendCallback(delegate
		{
			isScrumbling = false;
		});
		sequence.SetEase(Ease.Linear);
		sequence.onComplete = (TweenCallback)Delegate.Combine(sequence.onComplete, (TweenCallback)delegate
		{
			waitTween.Play();
		});
		waitTween = DOVirtual.Float(0f, afterTime, afterTime, delegate
		{
		});
		waitTween.Pause();
		Tween tween = waitTween;
		tween.onComplete = (TweenCallback)Delegate.Combine(tween.onComplete, (TweenCallback)delegate
		{
			completeCallback();
		});
		scrumbleTween = sequence;
		return scrumbleTween;
	}

	private Tween DoText(string str)
	{
		scrumbleTween = DOVirtual.Float(0f, str.Length, (float)str.Length * timeForChar, TweenUpdate).SetEase(Ease.Linear);
		return scrumbleTween;
		void TweenUpdate(float value)
		{
			int num = Mathf.RoundToInt(value);
			if (num != showdChars)
			{
				showdChars = num;
				label.text = taggedString.GetSubstring(showdChars);
			}
		}
	}

	public Tween DoShow()
	{
		base.gameObject.SetActive(value: true);
		label.text = "";
		base.transform.localScale = Vector3.zero;
		return base.transform.DOScale(1f, 0.4f);
	}

	public Tween DoHide()
	{
		label.text = "";
		base.transform.localScale = Vector3.one;
		return base.transform.DOScale(0f, 0.4f).OnComplete(delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}

	public void DoKill()
	{
		scrumbleTween?.Kill(complete: true);
		waitTween?.Kill(complete: true);
		isScrumbling = false;
	}

	public void AtSkip()
	{
		if (isScrumbling)
		{
			scrumbleTween?.Kill(complete: true);
			isScrumbling = false;
			waitTween.Play();
		}
		else
		{
			waitTween?.Kill();
			completeCallback();
		}
	}
}
