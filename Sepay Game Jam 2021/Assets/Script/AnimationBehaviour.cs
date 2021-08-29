using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBehaviour : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.gameObject, stateInfo.length - 0.5f);
    }
}
