using System;
using UnityEngine;

namespace AnimatorEvents
{
	[Serializable]
	public class EventNodeBase
	{
		public virtual void OnEvent(Animator animator) { }
		public virtual void OnExit(Animator animator) { }
	}

	public class AnimatorEventSMB : StateMachineBehaviourExtended
	{
		public TimedEvent[] onNormalizedTimeReached;

		[Serializable]
		public struct TimedEvent
		{
			[Range(0, 1)]
			public float normalizedTime;
			public bool repeat;
			public bool atLeastOnce;
			public bool neverWhileExit;
			[NonSerialized] public int nextNormalizedTime;

			[SerializeReference, SubclassSelector] EventNodeBase _argParam;
			public EventNodeBase ArgParam => _argParam;
		}

		public override void StateExit_TransitionStarts(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			for (int i = 0; i < onNormalizedTimeReached.Length; i++)
            {
				onNormalizedTimeReached[i].ArgParam?.OnExit(animator);
			}
		}

		public override void StateExit_TransitionEnds(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			// 終了時に実行したいイベントを確定し、後でリセットする。
			for (int i = 0; i < onNormalizedTimeReached.Length; i++)
			{
				if (onNormalizedTimeReached[i].atLeastOnce && onNormalizedTimeReached[i].nextNormalizedTime == 0)
				{
					onNormalizedTimeReached[i].ArgParam?.OnEvent(animator);
				}
				onNormalizedTimeReached[i].nextNormalizedTime = 0;
			}
		}

		public override void StateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			for (int i = 0; i < onNormalizedTimeReached.Length; i++)
			{
				if (onNormalizedTimeReached[i].neverWhileExit && currentState == State.ExitTransitioning) continue;

				if (stateInfo.normalizedTime >= onNormalizedTimeReached[i].normalizedTime + onNormalizedTimeReached[i].nextNormalizedTime)
				{
					onNormalizedTimeReached[i].ArgParam?.OnEvent(animator);

					if (onNormalizedTimeReached[i].repeat)
					{
						onNormalizedTimeReached[i].nextNormalizedTime++;
					}
					else
					{
						onNormalizedTimeReached[i].nextNormalizedTime = int.MaxValue;
					}
				}
			}
		}

	}
}
