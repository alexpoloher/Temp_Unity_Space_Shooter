using System.Collections;
using UnityEngine;

public class EnemigoBVertical : MonoBehaviour
{

    [SerializeField] float speed = 2.0f;
    Vector3 direccionMovimiento = Vector3.left;

    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float delayDisparo = 1.5f;

    private float damageToDeal;
    private bool puedeDisparar = true;
    private Animator animator;
    private EnemyParameters enemyParameters;

    private bool vaASegundaPosicion = false;
    Vector3 direccionMovimientoVertical = Vector3.up;
    private bool haIniciadoMovimientoVertical = false;

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

        //Se va a poder parar en dos posiciones distintas antes de subir y bajar, de forma aleatoria
        if (Random.Range(1, 2) == 1)
        {
            vaASegundaPosicion = false;
        }
        else {
            vaASegundaPosicion = true;
        }

        //Además, la mitad de las veces empezará moviendose hacia arriba y la mitad hacia abajo
        if (Random.Range(1, 3) == 1)
        {
            direccionMovimientoVertical = Vector3.up;
        }
        else
        {
            direccionMovimientoVertical = Vector3.down;
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direccionMovimiento * speed * Time.deltaTime, Space.World);  //Para que si están rotados, igualmente vayan a la izquierda en X
        
        if (((!vaASegundaPosicion && transform.position.x < 11) || (vaASegundaPosicion && transform.position.x < 3.19)) && !haIniciadoMovimientoVertical)
        {
            direccionMovimiento = direccionMovimientoVertical; //Si llega hasta ese punto, empezará a ir hacia la derecha
            haIniciadoMovimientoVertical = true;
        }

        //Si llega a un límite de la pantalla, se empieza a mover hacia el otro lado
        if (transform.position.y > maxCamara.y - 0.5f || transform.position.y < minCamara.y + 0.5f) {
            direccionMovimiento *= -1;

            //Y además se aumenta la frecuencia de disparo cada vez que choca
            if (delayDisparo > 0.4f) {
                delayDisparo -= 0.1f;
            }
        }

        if (puedeDisparar)
        {
            Disparar();
        }
    }

    private void Disparar()
    {
        Vector3 posicionDisparo = transform.GetChild(0).position;  //Se coge la posición del cañon, que es de donde va a salir la bala
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
