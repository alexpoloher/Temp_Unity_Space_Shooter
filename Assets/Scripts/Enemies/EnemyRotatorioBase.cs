using System.Collections;
using UnityEngine;

public class EnemyRotatorioBase : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float velocidadDesplazamiento = 2.0f;
    [SerializeField] float velocidadRotacion = 45f;

    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float delayDisparo = 1f;

    private float damageToDeal;
    private bool puedeDisparar = true;
    private EnemyParameters enemyParameters;
    Vector3 direccionMovimiento = Vector3.left;
    Vector3 dirreccionRotacion = new Vector3(0,0,1);

    private Vector2 minCamara;
    private Vector2 maxCamara;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyParameters = GetComponent<EnemyParameters>();
        damageToDeal = enemyParameters.damageToDeal;

        //Se obtienen las coordenadas máximas y minimas de la cámara
        minCamara = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));    //(el 0,0 es el borde inferior izquierdo)
        maxCamara = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));    //(el 1,1 es el borde superior derecho de la cámara)

    }

    // Update is called once per frame
    void Update()
    {
        //A la vez que se desplaza en X, rotará sobre sí mismo
        transform.Translate(direccionMovimiento * velocidadDesplazamiento * Time.deltaTime, Space.World);
        transform.Rotate(dirreccionRotacion * velocidadRotacion * Time.deltaTime, Space.World);

        if (transform.GetChild(0).gameObject.transform.position.y > maxCamara.y || transform.GetChild(0).gameObject.transform.position.y < minCamara.y) {
            velocidadRotacion = velocidadRotacion * -1;
        }


        //Si el hijo (que es el que lleva el sprite, pasa de ese punto en x, se elimina el GameObject porque ha llegado al límite
        if (transform.GetChild(0).gameObject.transform.position.x < minCamara.x) {
            Destroy(gameObject);
        }

        if (puedeDisparar) {
            Disparar();
        }
    }

    private void Disparar()
    {
        Vector3 posicionDisparo = transform.GetChild(1).position;  //Se coge la posición del cañon, que es de donde va a salir la bala
        GameObject bulletCreada = Instantiate(projectilePrefab, posicionDisparo, transform.GetChild(1).rotation);
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
        if (elOtro.gameObject.CompareTag("PlayerShot") || elOtro.gameObject.CompareTag("Player") || elOtro.gameObject.CompareTag("Saw"))
        {
            Morir();
           
        }
    }

    void Morir()
    {
        Vector3 posicionExplosion = transform.GetChild(0).position;  //Se coge la posición del hijo que lleva el sprite
        Instantiate(explosionPrefab, posicionExplosion, Quaternion.identity);

        //Se coge el único spawner de power ups de la escena y él calculará si debe spawnear un PowerUp
        PowerUpSpawner spawner = FindAnyObjectByType<PowerUpSpawner>();
        spawner.SpawnearPowerUp(transform.GetChild(0).position);

        Destroy(gameObject);
        //animator.SetTrigger("death");

    }

}
