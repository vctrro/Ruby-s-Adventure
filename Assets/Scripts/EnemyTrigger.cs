using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTrigger : MonoBehaviour
{
    [HideInInspector] public FindRubyEvent OnFindRuby;
    [HideInInspector] public UnityEvent OnLostRuby;
    private void OnTriggerStay2D(Collider2D other)
    {
        OnFindRuby.Invoke(other.transform.position);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnLostRuby.Invoke();
    }

    [System.Serializable]
    public class FindRubyEvent : UnityEvent<Vector2>
    {

    }
}
