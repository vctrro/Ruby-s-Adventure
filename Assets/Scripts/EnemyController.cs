using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D), typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    public float maxSpeed = 2f;
    bool isFixed, persecution;
    float fixTime;
    Rigidbody2D rb2d;
    Animator animator;
    Vector2 startPosition, moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = rb2d.position;
        print("Starting " + Time.time);
        StartCoroutine(WaitAndPrint(2.0F));
        print($"<color=green>Before WaitAndPrint Finishes {Time.time}</color>");
    }

    // Update is called once per frame
    void Update()
    {        
        if (isFixed) 
        {
            fixTime -= Time.deltaTime;
            if (fixTime > 0) return;
            
            isFixed = false;
            animator.SetBool("isFixed", false);           
        }

        if (!persecution)
        {
            if ((Vector2.Distance(startPosition, rb2d.position) > 0.2f)) //!rb2d.Equals(startPosition))
            {
                MoveTo(startPosition);
            }
            else
            {
                moveDirection.Set(0,0); 
            }
        }

        animator.SetFloat("Move X", moveDirection.x);
        animator.SetFloat("Move Y", moveDirection.y);        
        //Debug.Log($"{rb2d.position}  {moveDirection}");
    }

    IEnumerator WaitAndPrint(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        Debug.Log($"<color=red>WaitAndPrint {Time.time}</color>");
    }

    private void MoveTo(Vector2 direction)
    {
        moveDirection = direction - rb2d.position;
        moveDirection.Normalize();
        rb2d.position += moveDirection * maxSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.name == "Ruby") 
        {
            other.collider.GetComponent<RubyController>().changeHealth(-1);
        }
        else
        {
            //обходить препятствия       
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (other.gameObject.name == "Ruby") 
        {
            MoveTo(other.transform.position);
            persecution = true;
        }        
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.name == "Ruby") 
            persecution = false;
    }

    public void Fix()
    {
        isFixed = true;
        animator.SetBool("isFixed", true);
        fixTime = 2.5f;
        //rb2d.simulated = false;
    }
}
