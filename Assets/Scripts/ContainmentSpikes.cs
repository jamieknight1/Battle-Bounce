using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainmentSpikes : MonoBehaviour
{
    Vector2 startPos;
    [SerializeField] float spikeSpeed;
    [SerializeField] float stopPos;
    [HideInInspector] public bool playerDead = false;
    [SerializeField] int inequalityNumber;
    [SerializeField] float damage;
    [SerializeField] float knockbackForce;

    private List<GameObject> players = new List<GameObject>();
    private int playerCount;

    void Awake()
    {
        startPos = transform.position;
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(player);
        }

        playerCount = players.Count;
    }

    void OnEnable()
    {
        Health.LostLife += ResetSpikes;
    }

    void OnDisable()
    {
        Health.LostLife -= ResetSpikes;
    }

    void Update()
    {
        if (inequalityNumber * transform.position.y > stopPos * inequalityNumber) { transform.Translate(Time.deltaTime * Vector2.down * spikeSpeed); }

        if (playerDead)
        {
            transform.position = startPos;
            playerDead = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Player")
        {
            other.gameObject.GetComponent<Health>().TakeDamage(damage);

            Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector2 knockbackDir = other.transform.position - transform.position;
            playerRb.AddForce(knockbackDir * knockbackForce);
        }
    }

    private void ResetSpikes()
    {
        playerDead = true;
    }
}
