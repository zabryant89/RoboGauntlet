using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBomb : MonoBehaviour
{
    public float speed;
    public float attackDamage;
    public float timeToExplode; //in seconds, note this is divided by 3, as the enemy "counts down" visually
    private int explodeCount = 0; //just counts the iterations to ensure boom boom
    private int explodeMax = 3; //tied to above
    public GameObject explosion; //explosion effect
    private Transform target; //player
    public SpriteRenderer spriteRenderer; //insert the sprite renderer 
    public Sprite[] spriteArray = new Sprite[3]; //countdown until boom

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerControl>().transform;
        spriteRenderer.sprite = spriteArray[0];
        StartCoroutine(Countdown());
        speed *= 3;
        StartCoroutine(SpeedBoost());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        explosion = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(explosion, 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //explodes
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().UpdateHealth(attackDamage);
            Destroy(this.gameObject);
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator SpeedBoost()
    {
        yield return new WaitForSeconds(0.5f);
        speed /= 3;
    }

    private IEnumerator Countdown()
    {
        if (explodeCount < explodeMax)
        {
            explodeCount++;
            yield return new WaitForSeconds(timeToExplode / 3);
            if (explodeCount != 3)
            {
                spriteRenderer.sprite = spriteArray[explodeCount];
            }
            StartCoroutine(Countdown());
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
