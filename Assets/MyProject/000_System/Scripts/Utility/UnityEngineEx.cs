using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Utility.UnityEngineEx
{
	[Serializable]
	public class AudioSourceParam
	{
		[SerializeField] public  bool playOnAwake = false;
		[SerializeField, Range(0.0f, 1.0f)] public float volume = 1.0f;
		[SerializeField, Range(0.0f, 1.0f)] public float pitch = 1.0f;
		[SerializeField] public bool loop = false;

		void Set(in AudioSource source)
		{
			loop = source.loop;
			pitch = source.pitch;
			volume = source.volume;
			playOnAwake = source.playOnAwake;
		}

		void Get(ref AudioSource source)
		{
			source.loop=loop;
			source.pitch= pitch;
			source.volume= volume;
			source.playOnAwake= playOnAwake;
		}

	}


	class UnityEngineEx
	{
		public static void ColorToHSV(Color color, out Color hsv)
		{
			hsv = new();
			Color.RGBToHSV(color, out hsv.r, out hsv.g, out hsv.b);
		}

		

	}

	internal static class RectTransformEx
	{
		/// <summary>
		/// 左端の座標を返す
		/// </summary>
		public static Vector2 Left(this RectTransform self)
		{
			return new Vector2(self.anchoredPosition.x - self.sizeDelta.x * self.pivot.x * self.localScale.x, self.anchoredPosition.y);
		}

		/// <summary>
		/// 右端の座標を返す
		/// </summary>
		public static Vector2 Right(this RectTransform self)
		{
			return new Vector2(self.anchoredPosition.x + self.sizeDelta.x * (1 - self.pivot.x) * self.localScale.x, self.anchoredPosition.y);
		}

		/// <summary>
		/// 下端の座標を返す
		/// </summary>
		public static Vector2 Bottom(this RectTransform self)
		{
			return new Vector2(self.anchoredPosition.x, self.anchoredPosition.y - self.sizeDelta.y * self.pivot.y * self.localScale.y);
		}

		/// <summary>
		/// 上端の座標を返す
		/// </summary>
		public static Vector2 Top(this RectTransform self)
		{
			return new Vector2(self.anchoredPosition.x, self.anchoredPosition.y + self.sizeDelta.y * (1 - self.pivot.y) * self.localScale.y);
		}

		/// <summary>
		/// 端を表す矩形を返します
		/// </summary>
		public static Rect RectEdge(this RectTransform self)
		{
			var rect = new Rect
			{
				xMin = self.Left().x,
				xMax = self.Right().x,
				yMin = self.Bottom().y,
				yMax = self.Top().y,
			};

			return rect;
		}

		public static bool ContainsRect(this RectTransform self,RectTransform target)
		{
			Rect myRect = RectEdge(self);
			Rect rect= target.rect;

			return (rect.xMin >= myRect.xMin && rect.xMax <= myRect.xMax &&
					rect.yMin >= myRect.yMin && rect.yMax <= myRect.yMax);

		}
		public static bool ContainsRect(this RectTransform self,Vector2 target)
		{
			Rect myRect = RectEdge(self);

			return (target.x >= myRect.xMin && target.x <= myRect.xMax &&
					target.y >= myRect.yMin && target.y <= myRect.yMax);

		}
	}

	internal static class ImageEx
	{
		public static Vector2 GaugeFillRight(this Image gauge)
		{
			var gaugeRect = gauge.GetComponent<RectTransform>();

			return gaugeRect.Left() + new Vector2(gaugeRect.sizeDelta.x * gauge.fillAmount, 0f);

		}

		public static Vector2 GaugeAnchor(this Image gauge, Image parent)
		{
			Vector2 startPos;

			if (parent)
			{
				startPos = GaugeFillRight(parent);
			}
			else
			{
				var gaugeRect= gauge.GetComponent<RectTransform>();
				startPos = gaugeRect.Left();
			}

			return (GaugeFillRight(gauge) - startPos) / 2 + startPos;
		}
	}
	internal static class TransformEx
	{
		public static Transform PosX(this Transform self, float x)
		{
			self.position = new Vector3(x, self.position.y, self.position.z);
			return self;
		}
		public static Transform PosY(this Transform self, float y)
		{
			self.position = new Vector3(self.position.x, y, self.position.z);
			return self;
		}
		public static Transform PosZ(this Transform self, float z)
		{
			self.position = new Vector3(self.position.x, self.position.y, z);
			return self;
		}
	}

	[Serializable]
	public class ElapcedCoolTimer
	{
		[SerializeField] float coolTime=1.0f;
		float elapsedCoolTime = 0.0f;
		public bool isActive = false;

		[SerializeField] Image gaugeImage;

		public float ElapsedCoolTime => Math.Min(elapsedCoolTime,coolTime);

		/// <summary>
		/// IsActiveがtrueの時更新
		/// </summary>
		/// <returns>クールタイムが終了しているか</returns>
		public bool UpdateCoolTime()
		{
			if (isActive == false || IsEnd()) 
			{
				UpdateGauge();
				return IsEnd(); 
			}
			elapsedCoolTime += Time.deltaTime;
			UpdateGauge();
			return IsEnd();
		}
		/// <summary>
		/// クールタイムが終了しているか
		/// </summary>
		/// <returns></returns>
		public bool IsEnd()
		{
			return (elapsedCoolTime >= coolTime);
		}


		public float ElapcedPer()
		{
			return elapsedCoolTime / coolTime;
		}

		public void UpdateGauge()
		{
			if (gaugeImage)
			{
				gaugeImage.fillAmount = ElapcedPer();
			}
		}

		public void Reset()
		{
			elapsedCoolTime = 0.0f;
			isActive = false;
		}
	}
}