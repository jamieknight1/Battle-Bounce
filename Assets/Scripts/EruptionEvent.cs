using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EruptionEvent : MonoBehaviour
{
    [SerializeField] private GameObject volcanoVisual;
    [SerializeField] private GameObject lavaRock;

    private float nextTimeToErupt;
    private float eruptionDelay;

    [SerializeField] float minEruptTime;
    [SerializeField] float maxEruptTime;

    [SerializeField] int numberOfRocksToSpawn;
    [SerializeField] float minRockScale;
    [SerializeField] float maxRockScale;
    [SerializeField] GameObject ground;

    [SerializeField] float shakeSpeed;
    [SerializeField] float shakeAmount;
    [SerializeField] float volcanoShakeTime;

    private bool volcanoShake = false;

    private Animator animator;

    void Awake()
    {
        animator = volcanoVisual.GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= nextTimeToErupt)
        {
            eruptionDelay = Random.Range(minEruptTime, maxEruptTime);
            nextTimeToErupt = eruptionDelay + Time.time;

            Erupt();
        }

        if (volcanoShake) transform.position = new Vector3(Mathf.Sin(Time.time * shakeSpeed) * shakeAmount, transform.position.y);
    }

    private void Erupt()
    {
        StartCoroutine(VolcanoShake());
    }

    private IEnumerator VolcanoShake()
    {
        animator.SetTrigger("Squash");
        volcanoShake = true;
        yield return new WaitForSeconds(volcanoShakeTime);
        volcanoShake = false;
        animator.SetTrigger("Stretch");
        StartCoroutine(SpawnRocks());
    }

    private IEnumerator SpawnRocks()
    {
        for (int i = 0; i < numberOfRocksToSpawn; i++)
        {
            GameObject newRock = Instantiate(lavaRock, new Vector3(Random.Range(-7.5f, 7.5f), 10f), Quaternion.Euler(0 , 0, Random.Range(0f, 360f)));
            newRock.GetComponent<LavaRock>().SetGround(ground); 
            newRock.transform.localScale += new Vector3(Random.Range(minRockScale, maxRockScale), Random.Range(minRockScale, maxRockScale), Random.Range(minRockScale, maxRockScale));
            yield return new WaitForSeconds(1);
        }
    }
}
