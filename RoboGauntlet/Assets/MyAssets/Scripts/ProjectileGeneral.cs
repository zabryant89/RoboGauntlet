using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGeneral : MonoBehaviour
{
    public GameObject hitEffect;
    public GameObject proj;

    private void Start()
    {
        StartCoroutine(AutoDestroy());
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        effect.transform.up = transform.up;
        Destroy(effect, 0.1f);
        Destroy(gameObject);

        if (this.CompareTag("Player") && coll.gameObject.layer == 6)
        {
            coll.gameObject.GetComponent<EnemyHealth>().UpdateHealth(-1);
        }

        if (this.gameObject.layer == 6 && coll.gameObject.CompareTag("Player"))
        {
            coll.gameObject.GetComponent<PlayerHealth>().UpdateHealth(-1);
        }

        if (this.CompareTag("Player") && coll.gameObject.layer == 7)
        {
            coll.gameObject.GetComponent<Boss>().UpdateHealth(-1);
        }
    }

    IEnumerator AutoDestroy() //deal with potential clipping through walls, don't want infinite missiles!
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
