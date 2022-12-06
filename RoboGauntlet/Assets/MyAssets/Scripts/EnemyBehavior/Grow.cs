using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grow : MonoBehaviour
{
    //public GameObject projectile;
    private float curTime;
    public float time;
    public float maxSize; //note: this is for both X and Y values
    public GameObject hitEffect;

    private void Start()
    {
        StartCoroutine(ScaleOverTime(time));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ScaleOverTime(float time)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 originalHitScale = hitEffect.transform.localScale;
        Vector3 maxScale = new Vector3(maxSize, maxSize, 1f);

        do
        {
            transform.localScale = Vector3.Lerp(originalScale, maxScale, curTime / time);
            hitEffect.transform.localScale = transform.localScale/1.5f;
            curTime += Time.deltaTime;
            yield return null;
        } while (curTime <= time);
    }
}
