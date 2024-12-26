using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyShake : MonoBehaviour
{
    [SerializeField] private float shakeAmount;
    [SerializeField] private float shakeDuration;

    public void ShakeCandy(GameObject candy)
    {
        StartCoroutine(Shake(candy));
    }
    private IEnumerator Shake(GameObject candy)
    {
        Vector3 originalPosition = candy.transform.position; 

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration && Time.timeScale == 1f)
        {
            Vector3 randomOffset = new Vector3
                (Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount), 0);

            candy.transform.position = originalPosition + randomOffset;

            elapsedTime += Time.deltaTime; 
            yield return null; 
        }

        candy.transform.position = originalPosition;
    }

}
