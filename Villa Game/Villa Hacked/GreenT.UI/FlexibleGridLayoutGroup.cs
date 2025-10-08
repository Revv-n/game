using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.UI;

public class FlexibleGridLayoutGroup : LayoutGroup
{
	public enum FitType
	{
		None,
		AxisX,
		AxisY,
		Both
	}

	public enum Alignement
	{
		Left,
		Center,
		Right
	}

	public Vector2 spacing;

	public bool restrictMaxCellSize;

	public Vector2 maxCellSize;

	public FitType fitType;

	public GridLayoutGroup.Constraint constraint;

	[SerializeField]
	private int constraintCount = 2;

	public Alignement subAlignement;

	[SerializeField]
	protected GridLayoutGroup.Corner m_StartCorner;

	[SerializeField]
	protected GridLayoutGroup.Axis m_StartAxis;

	private int columns;

	private int rows;

	private float aspect;

	public GridLayoutGroup.Corner startCorner
	{
		get
		{
			return m_StartCorner;
		}
		set
		{
			SetProperty(ref m_StartCorner, value);
		}
	}

	public GridLayoutGroup.Axis startAxis
	{
		get
		{
			return m_StartAxis;
		}
		set
		{
			SetProperty(ref m_StartAxis, value);
		}
	}

	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
		int num = 1;
		int num2 = 1;
		switch (constraint)
		{
		case GridLayoutGroup.Constraint.FixedColumnCount:
			num = (num2 = constraintCount);
			break;
		case GridLayoutGroup.Constraint.FixedRowCount:
			num = (num2 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)constraintCount));
			break;
		case GridLayoutGroup.Constraint.Flexible:
			num = 1;
			num2 = ((base.rectChildren.Count > 3) ? Mathf.CeilToInt(Mathf.Sqrt(base.rectChildren.Count)) : base.rectChildren.Count);
			break;
		}
		float rowMinWidth = GetRowMinWidth(num);
		float rowPrefferedWidth = GetRowPrefferedWidth(num2);
		SetLayoutInputForAxis(rowMinWidth, rowPrefferedWidth, -1f, 0);
	}

	public override void CalculateLayoutInputVertical()
	{
		float columnPrefferedHeight = GetColumnPrefferedHeight(columns);
		SetLayoutInputForAxis(0f, columnPrefferedHeight, -1f, 1);
	}

	public override void SetLayoutHorizontal()
	{
		GetCellCount(out columns, out rows);
	}

	public override void SetLayoutVertical()
	{
		int num = (int)startCorner % 2;
		int num2 = (int)startCorner / 2;
		SetChildrenSize();
		int num3;
		if (startAxis == GridLayoutGroup.Axis.Horizontal)
		{
			columns = Mathf.Clamp(columns, 1, base.rectChildren.Count);
			num3 = columns;
			if (num3 != 0)
			{
				rows = Mathf.Clamp(rows, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num3));
			}
			else
			{
				rows = 0;
			}
		}
		else
		{
			rows = Mathf.Clamp(rows, 1, base.rectChildren.Count);
			num3 = rows;
			if (num3 != 0)
			{
				columns = Mathf.Clamp(columns, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num3));
			}
			else
			{
				columns = 0;
			}
		}
		float num4 = GetContentWidth(columns, (RectTransform _rect) => _rect.sizeDelta.x) + spacing.x * (float)(columns - 1);
		float requiredSpaceWithoutPadding = GetContentHeight(columns) + spacing.y * (float)(CalcRowsCount(columns) - 1);
		Vector2 vector = new Vector2(GetStartOffset(0, num4), GetStartOffset(1, requiredSpaceWithoutPadding));
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		if ((subAlignement == Alignement.Center || subAlignement == Alignement.Right) && columns > 0)
		{
			int num9 = base.rectChildren.Count % columns;
			if (rows > 1 && num9 > 0 && num9 != columns)
			{
				num8 = num4;
				for (int i = columns * (rows - 1); i != base.rectChildren.Count; i++)
				{
					num8 -= base.rectChildren[i].rect.width;
				}
				num8 -= spacing.x * (float)(num9 - 1);
			}
		}
		for (int j = 0; j < base.rectChildren.Count; j++)
		{
			int num11;
			int num10;
			if (startAxis == GridLayoutGroup.Axis.Horizontal)
			{
				num10 = j % num3;
				num11 = j / num3;
			}
			else
			{
				num10 = j / num3;
				num11 = j % num3;
			}
			if (num == 1)
			{
				num10 = columns - 1 - num10;
			}
			if (num2 == 1)
			{
				num11 = rows - 1 - num11;
			}
			if (j % columns == 0)
			{
				num5 = 0f;
				if (j / columns + 1 == rows)
				{
					switch (subAlignement)
					{
					case Alignement.Center:
						num5 = num8 / 2f;
						break;
					case Alignement.Right:
						num5 = num8;
						break;
					}
				}
				num6 += num7;
				num7 = 0f;
			}
			SetChildAlongAxis(base.rectChildren[j], 0, vector.x + num5);
			SetChildAlongAxis(base.rectChildren[j], 1, vector.y + num6 + spacing[1] * (float)num11);
			num7 = Mathf.Max(num7, base.rectChildren[j].rect.height);
			num5 += base.rectChildren[j].rect.width + spacing[0];
		}
	}

	private void SetChildrenSize()
	{
		aspect = GetAspect(base.rectTransform.rect.size, columns);
		for (int i = 0; i < base.rectChildren.Count; i++)
		{
			RectTransform rectTransform = base.rectChildren[i];
			m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition);
			if (restrictMaxCellSize)
			{
				m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta);
			}
			rectTransform.anchorMin = Vector2.up;
			rectTransform.anchorMax = Vector2.up;
			if (restrictMaxCellSize)
			{
				aspect = Mathf.Min(aspect, 1f);
				float num = maxCellSize.x * aspect;
				float num2 = maxCellSize.y * aspect;
				if (rectTransform.rect.size[0] != num || rectTransform.rect.size[1] != num2)
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxCellSize.x * aspect);
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxCellSize.y * aspect);
				}
			}
			else if (aspect != 1f)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.size.x * aspect);
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.size.y * aspect);
			}
		}
	}

	private float GetAspect(Vector2 size, int columns)
	{
		float num = 1f;
		if (fitType == FitType.AxisX || fitType == FitType.Both)
		{
			float contentPrefferedWidth = GetContentPrefferedWidth(columns);
			num = (size[0] - (float)base.padding.horizontal - spacing[0] * (float)(columns - 1)) / contentPrefferedWidth;
		}
		float num2 = 1f;
		if (fitType == FitType.AxisY || fitType == FitType.Both)
		{
			float contentPrefferedHeight = GetContentPrefferedHeight(columns);
			num2 = (size[1] - (float)base.padding.vertical - spacing[1] * (float)(CalcRowsCount(columns) - 1)) / contentPrefferedHeight;
		}
		float num3 = 1f;
		switch (fitType)
		{
		case FitType.AxisX:
			num3 = num;
			break;
		case FitType.AxisY:
			num3 = num2;
			break;
		case FitType.Both:
			num3 = Mathf.Min(num, num2);
			break;
		}
		if (float.IsNaN(num3) || float.IsInfinity(num3))
		{
			num3 = 1f;
		}
		return num3;
	}

	private float GetYAspect(int columns)
	{
		float value = 1f;
		if (fitType == FitType.AxisY || fitType == FitType.Both)
		{
			float contentPrefferedHeight = GetContentPrefferedHeight(columns);
			if (contentPrefferedHeight > 0f)
			{
				float num = base.rectTransform.sizeDelta[1];
				if (num > 0f)
				{
					int cellsByAxis = CalcRowsCount(columns);
					float num2 = SizeOfEmptySpace(cellsByAxis, 1);
					value = (num - num2) / contentPrefferedHeight;
				}
				else
				{
					value = 0f;
				}
			}
			else
			{
				value = 0f;
			}
		}
		return Mathf.Clamp01(value);
	}

	private float SizeOfEmptySpace(int cellsByAxis, int axis)
	{
		return (float)((axis == 0) ? base.padding.horizontal : base.padding.vertical) + ((cellsByAxis > 0) ? (spacing[axis] * (float)(cellsByAxis - 1)) : 0f);
	}

	private float GetRowMinWidth(int columns)
	{
		return GetContentMinWidth(columns) * GetYAspect(columns) + SizeOfEmptySpace(columns, 0);
	}

	private float GetRowPrefferedWidth(int columns)
	{
		float yAspect = GetYAspect(columns);
		float contentPrefferedWidth = GetContentPrefferedWidth(columns);
		float num = SizeOfEmptySpace(columns, 0);
		return contentPrefferedWidth * yAspect + num;
	}

	private float GetContentPrefferedWidth(int columns)
	{
		if (!restrictMaxCellSize)
		{
			return GetContentWidth(columns, LayoutUtility.GetPreferredWidth);
		}
		return (float)columns * maxCellSize.x;
	}

	private float GetContentMinWidth(int columns)
	{
		float num = GetContentWidth(columns, LayoutUtility.GetMinWidth);
		if (restrictMaxCellSize)
		{
			num = Mathf.Min(num, (float)columns * maxCellSize.x);
		}
		return num;
	}

	private float GetContentWidth(int columns, Func<RectTransform, float> sizeCalculator)
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i != base.rectChildren.Count; i++)
		{
			if (i % columns == 0)
			{
				if (num2 > num)
				{
					num = num2;
				}
				num2 = 0f;
			}
			RectTransform arg = base.rectChildren[i];
			num2 += sizeCalculator(arg);
		}
		if (num2 > num)
		{
			num = num2;
		}
		return num;
	}

	private int CalcRowsCount(int columns)
	{
		if (columns <= 0)
		{
			return 0;
		}
		return Mathf.CeilToInt((float)base.rectChildren.Count / (float)columns);
	}

	private float GetContentPrefferedHeight(int columns)
	{
		if (!restrictMaxCellSize)
		{
			return GetContentHeight(columns);
		}
		return (float)CalcRowsCount(columns) * maxCellSize.y;
	}

	private float GetColumnPrefferedHeight(int columns)
	{
		return GetContentPrefferedHeight(columns) + SizeOfEmptySpace(CalcRowsCount(columns), 1);
	}

	private float GetContentHeight(int columns)
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i != base.rectChildren.Count; i++)
		{
			if (i % columns == 0)
			{
				num += num2;
				num2 = 0f;
			}
			num2 = Mathf.Max(num2, base.rectChildren[i].rect.height);
		}
		return num + num2;
	}

	private void GetCellCount(out int columns, out int rows)
	{
		columns = 1;
		rows = 1;
		int count = base.rectChildren.Count;
		float num = Mathf.Sqrt(count);
		int num2 = (int)num;
		if (num2 * num2 == count && num2 <= constraintCount && constraint != GridLayoutGroup.Constraint.FixedColumnCount && constraint != GridLayoutGroup.Constraint.FixedRowCount)
		{
			columns = (rows = num2);
		}
		else if (constraint == GridLayoutGroup.Constraint.FixedColumnCount)
		{
			columns = constraintCount;
			if (count > columns)
			{
				rows = count / columns + ((count % columns > 0) ? 1 : 0);
			}
		}
		else if (constraint == GridLayoutGroup.Constraint.FixedRowCount)
		{
			rows = constraintCount;
			if (count > rows)
			{
				columns = count / rows + ((count % rows > 0) ? 1 : 0);
			}
		}
		else
		{
			if (count == 0)
			{
				return;
			}
			if (restrictMaxCellSize)
			{
				GetCellsForFlexibleGrid(out columns, out rows);
				return;
			}
			float width = base.rectTransform.rect.width;
			float num3 = base.rectChildren.Max((RectTransform _child) => _child.rect.size.x);
			int b = Mathf.FloorToInt((width - (float)base.padding.horizontal + spacing.x + 0.0001f) / (num3 + spacing.x));
			int a = ((base.rectChildren.Count > 3) ? Mathf.CeilToInt(num) : base.rectChildren.Count);
			columns = Mathf.Min(a, b);
			rows = Mathf.CeilToInt((float)base.rectChildren.Count / (float)columns);
		}
	}

	private void GetCellsForFlexibleGrid(out int columns, out int rows)
	{
		int count = base.rectChildren.Count;
		float width = base.rectTransform.rect.width;
		float height = base.rectTransform.rect.height;
		float num = maxCellSize.x / maxCellSize.y;
		float num2 = 0f;
		for (int i = (columns = (rows = 1)); i <= count; i++)
		{
			int num3 = CalcRowsCount(i);
			float num4 = (width - SizeOfEmptySpace(i, 0)) / (float)i;
			float num5 = (height - SizeOfEmptySpace(num3, 1)) / (float)num3;
			float num6 = num4 / num;
			if (num5 >= num6)
			{
				num5 = num6;
			}
			else
			{
				num4 = num5 * num;
			}
			float num7 = num5 * num4;
			if (num7 > num2)
			{
				num2 = num7;
				columns = i;
				rows = num3;
			}
		}
	}
}
