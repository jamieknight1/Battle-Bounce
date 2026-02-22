using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{
    [SerializeField] float nextTimeToRetract;
    [SerializeField] GameObject retractorBeam;
    [SerializeField] float retractCooldown;
    [SerializeField] float speed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextTimeToRetract && retractorBeam.activeSelf)
        {
            nextTimeToRetract = Time.time + retractCooldown;
            retractorBeam.SetActive(false);
        }

        if (Time.time >= nextTimeToRetract && !retractorBeam.activeSelf)
        {
            nextTimeToRetract = Time.time + retractCooldown;
            retractorBeam.SetActive(true);
        }

        transform.Translate(Vector2.right * Time.deltaTime * speed);
        
        if (transform.position.x <= -6f || transform.position.x >= 6f) { speed *= -1; }
    }
}
