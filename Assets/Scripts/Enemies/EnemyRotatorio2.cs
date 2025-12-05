using System.Collections;
using UnityEngine;

public class EnemyRotatorio2 : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float velocidadDesplazamiento = 2.0f;
    [SerializeField] float velocidadRotacion = 45f;
    [SerializeField] float velocidadMaximaRotacion = 75f;

    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float delayDisparo = 1f;

    private float damageToDeal;
    private bool puedeDisparar = true;
    private EnemyParameters enemyParameters;
    Vector3 direccionMovimiento = Vector3.up;
    Vector3 dirreccionRotacion = new Vector3(0, 0, 1);

    private Vector2 minCamara;
    private Vector2 maxCamara;
    private bool haEntradoEnPantalla = false;
    private bool empiezaHaciaArriba;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        //A la vez que se desplaza en X, rotará sobre sí mismo
        transform.Translate(direccionMovimiento * velocidadDesplazamiento * Time.deltaTime, Space.World);
        transform.Rotate(dirreccionRotacion * velocidadRotacion * Time.deltaTime, Space.World);
        //Además, el sprite rotará a la vez, para dar sensación de que siempre mira hacia ti
        transform.GetChild(0).transform.Rotate(dirreccionRotacion * velocidadRotacion * -1 * Time.deltaTime, Space.World);

        if (transform.GetChild(0).gameObject.transform.position.y > maxCamara.y || transform.GetChild(0).gameObject.transform.position.y < minCamara.y)
        {
            velocidadRotacion = velocidadRotacion * -1;
        }


        //Si llega a un límite de la pantalla, se empieza a mover hacia el otro lado
        if ((transform.GetChild(0).gameObject.transform.position.y > maxCamara.y - 0.5f || transform.GetChild(0).gameObject.transform.position.y < minCamara.y + 0.5f) && haEntradoEnPantalla)
        {
            direccionMovimiento *= -1;
            dirreccionRotacion *= -1;

            //Y se incrementa si se puede la velocidad de rotación
            AumentarVelocidadRotacion();
        }

        if (!haEntradoEnPantalla && ((transform.GetChild(0).gameObject.transform.position.y < maxCamara.y - 0.5f && !empiezaHaciaArriba) || (transform.GetChild(0).gameObject.transform.position.y > minCamara.y + 0.5f && empiezaHaciaArriba)))
        {
            haEntradoEnPantalla = true;
        }


        if (puedeDisparar)
        {
            Disparar();
        }
    }

    private void Disparar()
    {

        Vector3 posicionDisparo = transform.GetChild(0).GetChild(0).position;  //Se coge la posición del cañon, que es de donde va a salir la bala

        GameObject bulletCreada = Instantiate(projectilePrefab, posicionDisparo, transform.GetChild(0).GetChild(0).rotation);
        bulletCreada.GetComponent<EnemyParameters>().damageToDeal = damageToDeal;   //La bala creada hará el mismo daño que el enemigo que la dispara
        StartCoroutine(RecargaDisparo());
    }

    void AumentarVelocidadRotacion() {

        if (velocidadRotacion < velocidadMaximaRotacion) {
            velocidadRotacion += 10f;
        }

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
