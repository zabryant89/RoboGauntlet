using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEnemy : MonoBehaviour
{
    private bool isAwake;
    public float speed;
    public float attackDamage;
    public float time;
    public GameObject explosion;
    private Transform target;
    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray = new Sprite[2];

    // Start is called before the first frame update
    void Start()
    {
        isAwake = false;
        target = FindObjectOfType<PlayerControl>().transform;
        StartCoroutine(Countdown());
        spriteRenderer.sprite = spriteArray[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (isAwake)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        if (isAwake == true)
        {
            explosion = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(explosion, 0.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //explodes
        if (collision.gameObject.CompareTag("Player") && isAwake == true)
        {
            collision.gameObject.GetComponent<PlayerHealth>().UpdateHealth(attackDamage);
            Destroy(this.gameObject);
            Destroy(collision.gameObject);
        }
    }

    IEnumerator Countdown()
    {
        //idle for this period, doesn't do damage but can be collided with (safely)
        //can also be destroyed during this time (wise for the player)
        yield return new WaitForSeconds(time);
        isAwake = true;
        spriteRenderer.sprite = spriteArray[1];
    }
}
