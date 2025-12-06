using System.Collections;
using UnityEngine;

public class EnemyExtra : MonoBehaviour
{

    Vector3 direccionMovimiento = Vector3.left;

    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float delayDisparo = 1.5f;

    [Header("Movement")]
    [SerializeField] float speed = 2.0f;
    [SerializeField] float rotationSpeed = 45f;

    private float damageToDeal;
    private bool puedeDisparar = true;
    private Animator animator;
    private EnemyParameters enemyParameters;

    //private bool vaASegundaPosicion = false;
    Vector3 direccionMovimientoVertical = Vector3.up;
    private bool haEntradoEnPantalla = false;
    private bool empiezaHaciaArriba;
    private bool haRebotadoYa = false;  //Este enemigo solo rebotará una vez en pantalla y después se irá
    private int numCaniones = 4;

    private Vector2 minCamara;
    private Vector2 maxCamara;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        enemyParameters = GetComponent<EnemyParameters>();
        damageToDeal = enemyParameters.damageToDeal;

        //Se obtienen las coordenadas máximas y minimas de la cámara
        minCamara = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));    //(el 0,0 es el borde inferior izquierdo)
        maxCamara = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));    //(el 1,1 es el borde superior derecho de la cámara)


        if (transform.position.y > maxCamara.y)
        {
            direccionMovimiento = Vector3.down;
            empiezaHaciaArriba = false;
        }
        else
        {
            direccionMovimiento = Vector3.up;
            empiezaHaciaArriba = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direccionMovimiento * speed * Time.deltaTime, Space.World);  //Para que si están rotados, igualmente vayan a la izquierda en X
        transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime, Space.World);


        //Si llega a un límite de la pantalla, se empieza a mover hacia el otro lado
        if ((transform.position.y > maxCamara.y - 0.5f || transform.position.y < minCamara.y + 0.5f) && haEntradoEnPantalla && !haRebotadoYa)
        {
            direccionMovimiento *= -1;
            haRebotadoYa = true;    //Solo rebotará una vez

        }

        if (!haEntradoEnPantalla && ((transform.position.y < maxCamara.y - 0.5f && !empiezaHaciaArriba) || (transform.position.y > minCamara.y + 0.5f && empiezaHaciaArriba)))
        {
            haEntradoEnPantalla = true;
        }

        if (haRebotadoYa && (transform.position.y > maxCamara.y + 1f || transform.position.y < minCamara.y - 1f)) {
            Destroy(gameObject);
        }


        if (puedeDisparar)
        {
            Disparar();
        }
    }

    private void Disparar()
    {
        //Este dispara por 4 cañones
        /*Vector3 posicionDisparo1 = transform.GetChild(0).position;  //Se coge la posición del cañon, que es de donde va a salir la bala
        GameObject bulletCreada1 = Instantiate(projectilePrefab, posicionDisparo1, transform.GetChild(0).rotation);
        Vector3 posicionDisparo2 = transform.GetChild(1).position;
        GameObject bulletCreada2 = Instantiate(projectilePrefab, posicionDisparo2, transform.GetChild(1).rotation);
        Vector3 posicionDisparo3 = transform.GetChild(2).position;
        GameObject bulletCreada3 = Instantiate(projectilePrefab, posicionDisparo3, transform.GetChild(2).rotation);
        Vector3 posicionDisparo4 = transform.GetChild(3).position;
        GameObject bulletCreada4 = Instantiate(projectilePrefab, posicionDisparo4, transform.GetChild(3).rotation);

        bulletCreada1.GetComponent<EnemyParameters>().damageToDeal = damageToDeal;   //La bala creada hará el mismo daño que el enemigo que la dispara
        bulletCreada2.GetComponent<EnemyParameters>().damageToDeal = damageToDeal;
        bulletCreada3.GetComponent<EnemyParameters>().damageToDeal = damageToDeal;
        bulletCreada4.GetComponent<EnemyParameters>().damageToDeal = damageToDeal;*/

        for (int i = 0; i < numCaniones; i++) {
            Vector3 posicionDisparo = transform.GetChild(i).position;  //Se coge la posición del cañon, que es de donde va a salir la bala
            GameObject bulletCreada = Instantiate(projectilePrefab, posicionDisparo, transform.GetChild(i).rotation);
            bulletCreada.GetComponent<EnemyParameters>().damageToDeal = damageToDeal;   //La bala creada hará el mismo daño que el enemigo que la dispara
        }
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
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        //Se coge el único spawner de power ups de la escena y él calculará si debe spawnear un PowerUp
        PowerUpSpawner spawner = FindAnyObjectByType<PowerUpSpawner>();
        spawner.SpawnearPowerUp(transform.position);

        Destroy(gameObject);
        //animator.SetTrigger("death");

    }
}
