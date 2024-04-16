using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.UIEx
{
	public class UIEx
	{
		public static Vector2 CanvasSizeDelta(Canvas canvas)
		{
			RectTransform rect = canvas.GetComponent<RectTransform>();
			return rect.sizeDelta * canvas.transform.localScale.z;
		}
	}
}
