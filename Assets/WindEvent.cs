using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEvent : MonoBehaviour
{
    private Vector2 windDir;
    [SerializeField] private float windSpeed;

    private float nextTimeToWind;
    private float windDelay;

    [SerializeField] float minWindTime;
    [SerializeField] float maxWindTime;

    private bool blowWind = false;

    [SerializeField] float minWindDuration;
    [SerializeField] float maxWindDuration;

    [SerializeField] ParticleSystem windParticle;
    [SerializeField] float windParticleHeadstart;

    void Start()
    {
        
    }

    void Update()
    {
        if (Time.time >= nextTimeToWind)
        {
            windDelay = Random.Range(minWindTime, maxWindTime);
            nextTimeToWind = windDelay + Time.time;

           StartCoroutine(WindBlow());
        }

        if (blowWind)
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                player.GetComponent<Rigidbody2D>().AddForce(windDir * windSpeed * Time.deltaTime);
            }
        }
    }

    private IEnumerator WindBlow()
    {
        //windDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        WindAngle();
        windParticle.Play();
        windParticle.transform.rotation = Quaternion.LookRotation(windDir);
        yield return new WaitForSeconds(windParticleHeadstart);
        blowWind = true;
        yield return new WaitForSeconds(Random.Range(minWindDuration, maxWindDuration));
        blowWind = false;
        //windParticle.Stop();
    }

    private void WindAngle()
    {
        float windAngle;
        if (Random.value < 0.5) windAngle = Random.Range(-45f, 45f) * Mathf.Deg2Rad;
        else windAngle = Random.Range(135f, 225f) * Mathf.Deg2Rad;

        Vector2 newDir = new Vector2(Mathf.Cos(windAngle), Mathf.Sin(windAngle));
        windDir = newDir.normalized;
    }
}
