using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationPlay : MonoBehaviour {

    public Animation MyAnimation;
    public string AnimationName;

    
    public void PlayMyAnim(bool Reversed = false)
    {
        if (Reversed)
        {
            MyAnimation[AnimationName].speed = -1;
            MyAnimation[AnimationName].time = MyAnimation[AnimationName].length;
            
        }
        else
        {
            MyAnimation[AnimationName].speed = 1;
            MyAnimation[AnimationName].time = 0f;
            
        }
        MyAnimation.Play();
    }

}
