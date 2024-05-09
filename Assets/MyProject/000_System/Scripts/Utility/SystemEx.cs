using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.SystemEx
{
	public static class SystemEx
	{

		public static int ClampRangeIndex<L>(int idx, List<L> list)
		{
			return Mathf.Clamp(idx, 0, list.Count - 1);
		}

		/// <summary>
		/// 範囲外に出た場合は反対端を返す
		/// </summary>
		/// <typeparam name="L"></typeparam>
		/// <param name="idx"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static int RangeLoopIndex<L>(int idx, ref List<L> list)
		{
			if (idx < 0)
			{
				idx = list.Count - 1;
			}
			if (idx >= list.Count)
			{
				idx = 0;
			}

			return idx;
		}
		/// <summary>
		/// 範囲外に出た場合は反対端を返す
		/// </summary>
		/// <typeparam name="L"></typeparam>
		/// <param name="idxSideL"></param>
		/// <param name="idxSideR"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static List<int> RangeLoopIndex<L>(int idxSideL, int idxSideR, ref List<L> list)
		{
			List<int> resultList = new();
			int currentIdx = idxSideL;
			while (currentIdx != idxSideR)
			{
				resultList.Add(currentIdx);
				++currentIdx;
				currentIdx = RangeLoopIndex(currentIdx, ref list);
			}

			return resultList;
		}

		public static bool IsEdge<L>(int idx, ref List<L> list)
		{
			return (idx == 0 || (idx + 1) == list.Count);
		}
	}
}
