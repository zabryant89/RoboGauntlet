using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeEnemyControl : MonoBehaviour
{
    /* NOTE: I need a sprite for the projectiles of this enemy
     */
    public float speed; //movement speed
    public float time; //cooldown period between shots
    public int powerShot; //ticker value for when triple shot is fired
    public int powerShotTicker;// current value
    public float quickTime; //time between each shot of the triple shot
    private Transform target; //player
    public float attackRate; //attack interval (in seconds)
    private float canAttack; //attack interval counter
    public float attackDamage; //damage when colliding
    public Transform firingPoint; //where the projectile will come out
    public GameObject bullet; //projectile
    public float bulletForce; //projectile speed

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerControl>().transform;
        StartCoroutine(NormalPause());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector2(
            target.position.x - transform.position.x,
            target.position.y - transform.position.y
            );
        transform.up = direction;

        if (Vector2.Distance(target.position, transform.position) > 4)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            Vector3 angle = new Vector2(transform.position.x, transform.position.y + 1);
            transform.position = Vector2.MoveTowards(transform.position, target.position + angle, speed * Time.deltaTime);
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

    IEnumerator QuickFire()
    {
        yield return new WaitForSeconds(quickTime);
        Shoot();
        if (powerShotTicker != 0)
        {
            powerShotTicker--;
            StartCoroutine(QuickFire());
        }
        else
        {
            StartCoroutine(NormalPause());
        }
    }

    IEnumerator NormalPause()
    {
        yield return new WaitForSeconds(time);
        Shoot();
        if (powerShotTicker < powerShot)
        {
            powerShotTicker++;
            StartCoroutine(NormalPause());
        }
        else
        {
            StartCoroutine(QuickFire());
        }
    }

    public void Shoot()
    {
        GameObject projectile = Instantiate(bullet, firingPoint.position, firingPoint.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.AddForce(firingPoint.up * bulletForce, ForceMode2D.Impulse);
    }
}
