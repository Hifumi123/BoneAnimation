﻿using UnityEngine;

public class UpperReloadingState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("UpperState", 0);
    }
}
