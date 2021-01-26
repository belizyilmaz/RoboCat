using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] float health = 100f;

    [Header("Shooting")]
    float shotCounter;
    [SerializeField] float minTime = 0.2f;
    [SerializeField] float maxTime = 3f;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float laserSpeed = 10f;
    [SerializeField] GameObject particleVFX;

    [Header("Sound")]
    [SerializeField] AudioClip dyingSFX;
    [SerializeField] AudioClip shootingSFX;
    [SerializeField] [Range(0,1)] float dyingSoundVol = 0.7f;
    [SerializeField] [Range(0, 1)] float shootingSoundVol = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(minTime, maxTime);
        }
    }

    private void Fire()
    {
        GameObject laser =
                     Instantiate(
                         laserPrefab,
                         transform.position,
                         Quaternion.Euler(new Vector3(0, 0, 90))) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(-laserSpeed, 0);
        AudioSource.PlayClipAtPoint(shootingSFX, Camera.main.transform.position, shootingSoundVol);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            TriggerExplosion();
        }
    }

    private void TriggerExplosion()
    {
        FindObjectOfType<GameSession>().AddToScore();
        PlayDieSFX();
        Destroy(gameObject);
        GameObject explosion = Instantiate(particleVFX, transform.position, transform.rotation);
        Destroy(explosion, 1f);
    }

    private void PlayDieSFX()
    {
        AudioSource.PlayClipAtPoint(dyingSFX, Camera.main.transform.position, dyingSoundVol);
    }
}
