using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CARnageAuxiliary : MonoBehaviour {

    public static float destroyAfterSec = 10;

    public static GameObject getCarFromHitbox(GameObject hitbox)
    {
        if (hitbox.transform.parent == null)
            return null;
        if (hitbox.GetComponent<RCC_CarControllerV3>() != null)
            return hitbox;

        return getCarFromHitbox(hitbox.transform.parent.gameObject);
    }

    static float getAnimationLength(Animator animator, string animationName)
    {
        foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
        {
            if(ac.name.Contains(animationName))
                return ac.length;
        }

        return -1;
    }

    public static void playAnimationTimeScaled(GameObject target, string animationName, float actualValue)
    {        
        foreach(Animator animator in target.GetComponentsInChildren<Animator>())
        {
            float animTime = getAnimationLength(animator, animationName);
            if(animTime > 0)
            {
                animator.SetTrigger(animationName);
                animator.speed = animTime / actualValue;
            }
        }

    }
    
    public enum ControllerType
    {
        MouseKeyboard,
        Controller1,
        Controller2,
        Controller3,
        AI
    }

    //public static void setAnimationSpeed(GameObject target, string animationName, float actualValue)
    //{
    //    float speed = 1;
    //    foreach (AnimationClip ac in target.GetComponentInChildren<Animator>().runtimeAnimatorController.animationClips)
    //    {
    //        //Debug.Log(ac.name + ": " + ac.length);
    //        if (ac.name.Contains(animationName))
    //        {
    //            speed = ac.length;

    //        }
    //    }

    //    float animTime = getAnimationLength(target, animationName);
    //    float speed = animTime / actualValue;
    //    return speed;
    //}
}
