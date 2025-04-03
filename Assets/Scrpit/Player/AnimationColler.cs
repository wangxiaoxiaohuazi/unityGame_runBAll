using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationColler : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;

    void Start()
    {
         if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator.Play("run");
    }
}
