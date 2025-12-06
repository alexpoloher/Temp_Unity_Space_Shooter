using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float speed = 2.0f;
    Vector3 linearVelocity = Vector3.left;

    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float delayDisparo = 1f;

    private float damageToDeal;
    private bool puedeDisparar = true;
    private Animator animator;
    private EnemyParameters enemyParameters;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        enemyParameters = GetComponent<EnemyParameters>();
        damageToDeal = enemyParameters.damageToDeal;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(linearVelocity * speed * Time.deltaTime, Space.World);  //Para que si están rotados, igualmente vayan a la izquierda en X
        if (transform.position.x < -10.41) { 
            linearVelocity = Vector3.right; //Si llega hasta ese punto, empezará a ir hacia la derecha
        }
        if (transform.position.x > 22) { 
            Destroy(gameObject);
        }

        if (puedeDisparar)
        {
            Disparar();
        }

    }

    private void Disparar()
    {
        Vector3 posicionDisparo = transform.GetChild(0).position;  //Se coge la posición del cañon, que es de donde va a salir la bala
        animator.SetTrigger("disparar");
        GameObject bulletCreada = Instantiate(projectilePrefab, posicionDisparo, Quaternion.identity);
        bulletCreada.GetComponent<EnemyParameters>().damageToDeal = damageToDeal;   //La bala creada hará el mismo daño que el enemigo que la dispara
        StartCoroutine(RecargaDisparo());
    }

    IEnumerator RecargaDisparo()
    {
        puedeDisparar = false;
        yield return new WaitForSeconds(delayDisparo);
        puedeDisparar = true;
    }

    private void OnTriggerEnter2D(Collider2D elOtro)
    {
        if (elOtro.gameObject.CompareTag("PlayerShot") || elOtro.gameObject.CompareTag("Player") || elOtro.gameObject.CompareTag("Saw")) {
            Morir();
        }
    }

    void Morir() {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        //Se coge el único spawner de power ups de la escena y él calculará si debe spawnear un PowerUp
        PowerUpSpawner spawner = FindAnyObjectByType<PowerUpSpawner>();
        spawner.SpawnearPowerUp(transform.position);

        Destroy(gameObject);
        //animator.SetTrigger("death");

    }

    private void DestruirEnemigo()
    {
        Destroy(gameObject);

    }

}
