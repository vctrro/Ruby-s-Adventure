using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Damageable", 0, Random.Range(0.0f, 1.1f));
    }
    private void OnTriggerStay2D(Collider2D other) {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null) 
        {            
            controller.changeHealth(-1);            
        }
    }
}
