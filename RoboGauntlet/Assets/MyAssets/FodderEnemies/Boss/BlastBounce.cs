using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastBounce : MonoBehaviour
{
    //this class is specifically for the bouncing projectile the boss shoots
    //projectile visuals
    public GameObject hitEffect;
    public GameObject proj;
    private int momBlastBounceCount = 0;
    public int momBlastBounceMax;
    public int attackDamage;
    private Vector3 lastVelocity;
    private Rigidbody2D rb;
    public bool isTelegraphing = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AutoDestroy());
        momBlastBounceCount = 0;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        lastVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var speed = lastVelocity.magnitude;
        var direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);

        if (collision.gameObject.CompareTag("Player") && isTelegraphing == false)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            effect.transform.up = transform.up;
            Destroy(effect, 0.1f);
            Destroy(gameObject);
            collision.gameObject.GetComponent<PlayerHealth>().UpdateHealth(attackDamage);
        }
        else if (momBlastBounceCount < momBlastBounceMax)
        {
            rb.velocity = direction * Mathf.Max(speed, 1f);
            transform.up = direction;
            momBlastBounceCount++;
        }
        else
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            effect.transform.up = transform.up;
            Destroy(effect, 0.1f);
            Destroy(gameObject);
        }
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(10);
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        effect.transform.up = transform.up;
        Destroy(effect, 0.1f);
        Destroy(gameObject);
    }
}
