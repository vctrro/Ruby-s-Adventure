using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float maxSpeed = 2f;
    bool isFixed;
    float fixTime;
    Rigidbody2D rb2d;
    Animator animator;
    Vector2 startPosition, moveDirection, rubyPos;
    GameObject ruby;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ruby = GameObject.Find("Ruby");
        startPosition = rb2d.position;
    }

    // Update is called once per frame
    void Update()
    {        
        if (isFixed) 
        {
            fixTime -= Time.deltaTime;
            if (fixTime > 0) {
                return;
            }
            isFixed = false;
            animator.SetBool("isFixed", false);           
        }
        rubyPos = ruby.transform.position;      

        if (Vector2.Distance(rb2d.position, rubyPos) < 6.0f)
        {            
            moveDirection = rubyPos - rb2d.position;
            moveDirection.Normalize();

            rb2d.position += moveDirection * maxSpeed * Time.deltaTime;
            //rb2d.MovePosition(position);
        }
        else 
        {
            if ((Vector2.Distance(startPosition, rb2d.position) > 0.2f)) //!rb2d.Equals(startPosition))
            {
                moveDirection = startPosition - rb2d.position;
                moveDirection.Normalize();

                rb2d.position += moveDirection * maxSpeed * Time.deltaTime;
                //rb2d.MovePosition(position);                
            }
        }
        animator.SetFloat("Move X", moveDirection.x);
        animator.SetFloat("Move Y", moveDirection.y);        
        
        moveDirection.Set(0,0);
        //Debug.Log($"{rb2d.position}  {moveDirection}");
        
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject == ruby) 
        {
            ruby.GetComponent<RubyController>().changeHealth(-1);
        }
        else
        {
            //обходить препятствия       
        }
    }

    

    public void Fix()
    {
        isFixed = true;
        animator.SetBool("isFixed", true);
        fixTime = 2.5f;
        //rb2d.simulated = false;
    }
}
