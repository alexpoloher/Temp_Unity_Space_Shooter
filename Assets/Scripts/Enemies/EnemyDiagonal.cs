using System.Collections;
using UnityEngine;

public class EnemyDiagonal : MonoBehaviour
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

    Vector3 direccionMovimientoVertical = Vector3.up;
    private bool haIniciadoMovimientoVertical = false;
    private bool moviendoseEnDiagonal = false;
    private int numCaniones = 3;

    private Vector2 minCamara;
    private Vector2 maxCamara;
    Vector3 objetivoActual = new Vector3();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        enemyParameters = GetComponent<EnemyParameters>();
        damageToDeal = enemyParameters.damageToDeal;

        //Se obtienen las coordenadas máximas y minimas de la cámara
        minCamara = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));    //(el 0,0 es el borde inferior izquierdo)
        maxCamara = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));    //(el 1,1 es el borde superior derecho de la cámara)

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
  
        if (!moviendoseEnDiagonal) {
            transform.Translate(direccionMovimiento * speed * Time.deltaTime, Space.World);  //Para que si están rotados, igualmente vayan a la izquierda en X
        }

        if (transform.position.x < 11 && !haIniciadoMovimientoVertical)
        {
            direccionMovimiento = direccionMovimientoVertical; //Si llega hasta ese punto, empezará a ir hacia la derecha
            haIniciadoMovimientoVertical = true;
        }

        //Si llega a un límite de la pantalla, se mueve en diagonal hasta la mitad opeusta de la pantalla
        if (transform.position.y > maxCamara.y - 0.5f || transform.position.y < minCamara.y + 0.5f)
        {
            //Para solo calcular el objetivo el primer frame en que va a iniciar el movimiento diagonal
            if (moviendoseEnDiagonal == false) {
                //Es max camera - min camera porque el mincamwera en x es negativo ya
                if (objetivoActual == Vector3.zero)
                {
                    //Si es el primexr movimiento diagonal, va hasta el medio
                    objetivoActual = new Vector3((maxCamara.x + minCamara.x) / 2, transform.position.y * -1, transform.position.z);
                }
                else {
                    //Si es el segundo, va hasta el final de la pantalla y se va
                    objetivoActual = new Vector3(minCamara.x - 2f, transform.position.y * -1, transform.position.z);
                }

            }

            //direccionMovimiento = direccionMovimiento * -1;
            moviendoseEnDiagonal = true;

            print(objetivoActual);

        }

        if (moviendoseEnDiagonal) {
            transform.position = Vector3.MoveTowards(transform.position, objetivoActual, speed * Time.deltaTime);

            if (transform.position == objetivoActual) {
                moviendoseEnDiagonal = false;

            }

        }

        if (puedeDisparar)
        {
            Disparar();
        }
        
        //Cuando se salga, se elimina
        if (transform.position.x < minCamara.x - 1.5f)
        {
            Destroy(gameObject);
        }


    }

    private void Disparar()
    {
        //Este dispara por 3 cañones
        /*Vector3 posicionDisparo1 = transform.GetChild(0).position;  //Se coge la posición del cañon, que es de donde va a salir la bala
        GameObject bulletCreada1 = Instantiate(projectilePrefab, posicionDisparo1, transform.GetChild(0).rotation);
        Vector3 posicionDisparo2 = transform.GetChild(1).position;  
        GameObject bulletCreada2 = Instantiate(projectilePrefab, posicionDisparo2, transform.GetChild(1).rotation);
        Vector3 posicionDisparo3 = transform.GetChild(2).position;  
        GameObject bulletCreada3 = Instantiate(projectilePrefab, posicionDisparo3, transform.GetChild(2).rotation);


        bulletCreada1.GetComponent<EnemyParameters>().damageToDeal = damageToDeal;   //La bala creada hará el mismo daño que el enemigo que la dispara
        bulletCreada2.GetComponent<EnemyParameters>().damageToDeal = damageToDeal;   
        bulletCreada3.GetComponent<EnemyParameters>().damageToDeal = damageToDeal;   
        StartCoroutine(RecargaDisparo());*/

        for (int i = 0; i < numCaniones; i++)
        {
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
