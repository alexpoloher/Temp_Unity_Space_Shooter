using System.Collections;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{

    [SerializeField] float speed = 2.0f;
    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float delayDisparo = 1f;
    private bool puedeDisparar = true;
    private EnemyParameters enemyParameters;
    [SerializeField] GameObject[] puntosAparicion;
    [SerializeField] float damagePorGolpe = 10f;
    [SerializeField] float vidaActual = 100f;
    [SerializeField] float vidaMaxima = 100f;
    private Vector2 minCamara;
    private Vector2 maxCamara;
    [SerializeField] AudioClip sonidoGolpe;

    private Vector3 direccionMovimiento = Vector3.left;
    private float damageToDeal;
    private int numCaniones = 3;
    private GestorCanvasVidaBoss gestorBarraVida;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Se obtienen las coordenadas máximas y minimas de la cámara
        minCamara = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));    //(el 0,0 es el borde inferior izquierdo)
        maxCamara = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));    //(el 1,1 es el borde superior derecho de la cámara)

        enemyParameters = GetComponent<EnemyParameters>();
        damageToDeal = enemyParameters.damageToDeal;
        vidaActual = vidaMaxima;
        gestorBarraVida = GameObject.FindAnyObjectByType<GestorCanvasVidaBoss>();
        gestorBarraVida.mostrarBarraVidaBoss();
        puntosAparicion = GameObject.FindAnyObjectByType<BossSpawner>().puntosAparicion;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direccionMovimiento * speed * Time.deltaTime, Space.World);  //Para que si están rotados, igualmente vayan a la izquierda en X
        if (transform.position.x < minCamara.x - 2 || transform.position.y < minCamara.y -2)
        {
            VolverAAparecer();
        }
        gestorBarraVida.ActualizarVida(vidaActual, vidaMaxima);
        if (puedeDisparar)
        {
            Disparar();
        }
    }

    void VolverAAparecer() {

        int indicePosicionNueva = Random.Range(0, puntosAparicion.Length);
        Vector3 nuevaPosicion = puntosAparicion[indicePosicionNueva].transform.position;
        if (nuevaPosicion.y > maxCamara.y)
        {
            Vector3 nuevaRotacion = new Vector3(0, 0, 180);
            transform.rotation = Quaternion.Euler(nuevaRotacion);
            transform.position = nuevaPosicion;
            direccionMovimiento = Vector3.down;
        }
        else {
            Vector3 nuevaRotacion = new Vector3(0, 0, 90);
            transform.rotation = Quaternion.Euler(nuevaRotacion);
            transform.position = nuevaPosicion;
            direccionMovimiento = Vector3.left;
        }
    }

    void Disparar() {

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
            DaniarEnemigo();
        }
    }

    void DaniarEnemigo() {
        vidaActual -= damagePorGolpe;
        GestorSonido.Instance.EjecutarSonido(sonidoGolpe);
        if (vidaActual <= 0) {
            vidaActual = 0;
            StartCoroutine(Morir());
        }
    
    }

    IEnumerator Morir() {
        speed = 0;
        if (direccionMovimiento.x == 0)
        {   //Va en vertical
            Instantiate(explosionPrefab, transform.position + new Vector3(0, 0, -2), Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            Instantiate(explosionPrefab, transform.position + new Vector3(0, 1, -2), Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            Instantiate(explosionPrefab, transform.position + new Vector3(0, -1, -2), Quaternion.identity);
        }
        else
        {  //Va en horizontal
            Instantiate(explosionPrefab, transform.position + new Vector3(0, 0, -2), Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            Instantiate(explosionPrefab, transform.position + new Vector3(1, 0, -2), Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            Instantiate(explosionPrefab, transform.position + new Vector3(-1, 0, -2), Quaternion.identity);
        }
        yield return new WaitForSeconds(0.5f);
        GestorPlayer.Instance.Victoria();
        Destroy(gameObject);

    }
}
