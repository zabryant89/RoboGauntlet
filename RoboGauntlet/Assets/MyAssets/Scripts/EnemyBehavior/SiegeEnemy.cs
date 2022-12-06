using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegeEnemy : MonoBehaviour
{
    public float speed;
    public float time;
    public float attackDamage;
    public float attackRate;
    private float canAttack;
    private Transform target;
    private bool isMoving;
    public Transform firingPoint;
    public GameObject bullet;
    public float bulletForce;

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerControl>().transform;
        isMoving = false;
        canAttack = 0f;
        StartCoroutine(ShootEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        if (attackRate > canAttack)
        {
            canAttack += Time.deltaTime;
        }

        Vector2 direction = new Vector2(
            target.position.x - transform.position.x,
            target.position.y - transform.position.y);
        transform.up = direction;

        if (Vector3.Distance(transform.position, target.position) < 5)
        {
            transform.up = -transform.up;
            Vector3 dirToTarget = transform.position - target.transform.position;
            Vector3 newPos = transform.position + dirToTarget;
            transform.position = Vector2.MoveTowards(transform.position, newPos, speed * Time.deltaTime);
            isMoving = true;
        }
        else
        {
            //transform.position = transform.position;
            isMoving = false;
        }
    }

    private void OnCollisionStay(Collision collision)
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
    IEnumerator ShootEnemy()
    {
        yield return new WaitForSeconds(time);
        if (isMoving == false)
        {
            Shoot();
        }
        else
        {

        }

        StartCoroutine(ShootEnemy());
    }

    public void Shoot()
    {
        GameObject projectile = Instantiate(bullet, firingPoint.position, firingPoint.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.AddForce(firingPoint.up * bulletForce, ForceMode2D.Impulse);
    }
}
