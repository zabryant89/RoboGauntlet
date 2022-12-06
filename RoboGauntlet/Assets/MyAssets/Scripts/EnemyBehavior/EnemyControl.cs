using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float speed;
    public float time;
    private float initialTime; //time for the initial charge, will 4x faster
    private bool firstAttack; //true until first charge initiates, see Attack()
    public float attackDamage;
    public float attackRate;
    private float canAttack;
    private Transform target;
    private Vector2 attackPos;
    private bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        isAttacking = false;
        firstAttack = true;
        initialTime = time / 4;
        target = FindObjectOfType<PlayerControl>().transform;
        StartCoroutine(PauseTime());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking)
        {
            Vector2 direction = new Vector2(//this block to to constantly face the player - vector determines facing direction
                target.position.x - transform.position.x,
                target.position.y - transform.position.y);
            transform.up = direction; //forces unit to face player
            attackPos = target.position; //determines attack path (constantly updated until attack initiated)
        }
        else
        {
            Attack();
            if (Vector3.Distance(transform.position, attackPos) == 0)
            {
                isAttacking = false;
                StartCoroutine(PauseTime());
            }
        }
        if (attackRate > canAttack)
        {
            canAttack += Time.deltaTime;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (attackRate <= canAttack)
            {
                collision.gameObject.GetComponent<PlayerHealth>().UpdateHealth(attackDamage);
                canAttack = 0f;
            }
        }
    }

    private void Attack()
    {
        transform.position = Vector2.MoveTowards(transform.position, attackPos, speed * Time.deltaTime);
        
    }

    IEnumerator PauseTime()
    {
        if (firstAttack == true)
        {
            yield return new WaitForSeconds(initialTime);
            isAttacking = true;
            firstAttack = false;
        }
        else
        {
            yield return new WaitForSeconds(time);
            isAttacking = true;
        }
    }
}