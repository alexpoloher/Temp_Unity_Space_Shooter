using System.Collections;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    [SerializeField] float speed = 2.0f;
    Vector3 direccionMovimiento = Vector3.up;

    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float delayDisparo = 1f;

    [SerializeField] float velocidadRotacionEscudos = 45f;

    private float damageToDeal;
    private bool puedeDisparar = true;
    private Animator animator;
    private EnemyParameters enemyParameters;
    private float tiempoMoverseDiagonal = 1.5f;
    private bool haEntradoEnPantalla = false;
    private bool empiezaHaciaArriba;

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

        if (transform.position.y > maxCamara.y) {
            direccionMovimiento = Vector3.down;
            empiezaHaciaArriba = false;
        }
        else {
            direccionMovimiento = Vector3.up;
            empiezaHaciaArriba = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direccionMovimiento * speed * Time.deltaTime, Space.World);  //Para que si están rotados, igualmente vayan a la izquierda en X
        transform.GetChild(1).transform.Rotate(Vector3.back * velocidadRotacionEscudos * Time.deltaTime, Space.World);
        
        if (transform.position.x < minCamara.x - 2f)
        {
            Destroy(gameObject);
        }


        //Si llega a un límite de la pantalla, se empieza a mover hacia el otro lado
        if ((transform.position.y > maxCamara.y - 0.5f || transform.position.y < minCamara.y + 0.5f) && haEntradoEnPantalla)
        {
            direccionMovimiento *= -1;
            StartCoroutine(MoverseDiagonal());
        }

        if (!haEntradoEnPantalla && ((transform.position.y < maxCamara.y - 0.5f && !empiezaHaciaArriba) || (transform.position.y > minCamara.y + 0.5f && empiezaHaciaArriba))) { 
            haEntradoEnPantalla = true;
        }

        if (puedeDisparar)
        {
            Disparar();
        }

    }

    private void Disparar()
    {

        Vector3 posicionDisparo1 = transform.GetChild(0).position;  //Se coge la posición del cañon, que es de donde va a salir la bala
        GameObject bulletCreada1 = Instantiate(projectilePrefab, posicionDisparo1, transform.GetChild(0).rotation);
        bulletCreada1.GetComponent<EnemyParameters>().damageToDeal = damageToDeal;   //La bala creada hará el mismo daño que el enemigo que la dispara

        StartCoroutine(RecargaDisparo());
    }

    IEnumerator RecargaDisparo()
    {
        puedeDisparar = false;
        yield return new WaitForSeconds(delayDisparo);
        puedeDisparar = true;
    }

    IEnumerator MoverseDiagonal() {

        direccionMovimiento = direccionMovimiento + Vector3.left;

        yield return new WaitForSeconds(tiempoMoverseDiagonal);

        //Tras desplazarse diagonalmente un rato, vuelve a moverse en vertical
        if (direccionMovimiento.y > 0)
        {
            direccionMovimiento = Vector3.up;
        }
        else {
            direccionMovimiento = Vector3.down;
        }

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

    private void DestruirEnemigo()
    {
        Destroy(gameObject);

    }
}
