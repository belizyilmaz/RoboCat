using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // config params
    [Header("Player")]
    [SerializeField] float playerSpeed = 10f;
    [SerializeField] float padding = 1f;
    [SerializeField] int health = 300;

    [Header("Sound")]
    [SerializeField] AudioClip dyingSFX;
    [SerializeField] AudioClip shootingSFX;
    [SerializeField] [Range(0, 1)] float dyingSoundVol = 0.7f;
    [SerializeField] [Range(0, 1)] float shootingSoundVol = 0.25f;

    [Header("Laser")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float laserSpeed = 10f;
    [SerializeField] float firingPeriod = 0.1f;

    Coroutine firingCoroutine;

    float minX;
    float maxX;
    float minY;
    float maxY;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
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
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<Level>().LoadGameOver();
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(dyingSFX, Camera.main.transform.position, dyingSoundVol);
    }

    public int GetHealth()
    {
        return health;
    }

    private void Fire()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
          
        } else if(Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously()
    {
        while(true)
        {
            GameObject laser =
              Instantiate(
                  laserPrefab,
                  transform.position,
                  Quaternion.Euler(new Vector3(0, 0, 90))) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(laserSpeed, 0);
            AudioSource.PlayClipAtPoint(shootingSFX, Camera.main.transform.position, shootingSoundVol);
            yield return new WaitForSeconds(firingPeriod);
        }
        
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * playerSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * playerSpeed;
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, minX, maxX);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, minY, maxY);
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCam = Camera.main;
        minX = gameCam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        maxX = gameCam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        minY = gameCam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        maxY = gameCam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

   
}

