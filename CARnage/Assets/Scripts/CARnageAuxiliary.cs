using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CARnageAuxiliary : MonoBehaviour {

    public static float destroyAfterSec = 10;
    public static bool isPaused;

    public static void togglePause()
    {
        if (isPaused)
            pauseEnd();
        else
            pause();
    }

    public static void pause()
    {
        Time.timeScale = 0.0f;
        isPaused = true;
    }

    public static void pauseEnd()
    {
        Time.timeScale = 1.0f;
        isPaused = false;
    }

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
        AI,
        NONE
    }

    public static string colorMe(string s)
    {
        return s.Replace("Acid", "<color=lime>Acid</color>").Replace("Fire", "<color=red>Fire</color>").Replace("Leak", "<color=black>Leak</color>").Replace("HP", "<color=red>HP</color>").Replace("Shielded", "<color=cyan>Shielded</color>").Replace("Shield", "<color=cyan>Shield</color>").Replace("Gears", "<color=#303030>Gears</color>").Replace("Gear", "<color=#303030>Gear</color>").Replace("Explosion", "<color=yellow>Explosion</color>").Replace("Nitro", "<color=#F29200>Nitro</color>").Replace("Freeze", "<color=lightblue>Freeze</color>").Replace("Damage", "DMG");
    }

    public static int getPlayersPlayingCount()
    {
        int i = 0;
        if (!PlayerPrefs.GetString("Player0_controlledBy").Equals(""))
            i++;
        if (!PlayerPrefs.GetString("Player1_controlledBy").Equals(""))
            i++;
        if (!PlayerPrefs.GetString("Player2_controlledBy").Equals(""))
            i++;
        if (!PlayerPrefs.GetString("Player3_controlledBy").Equals(""))
            i++;

        return i;
    }

    //Breadth-first search
    public static Transform FindDeepChild(Transform parent, string name)
    {
        var result = parent.Find(name);
        if (result != null)
            return result;
        foreach (Transform child in parent)
        {
            result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
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
