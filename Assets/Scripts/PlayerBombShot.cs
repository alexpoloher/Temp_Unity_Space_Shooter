using System.Collections;
using UnityEngine;

public class PlayerBombShot : MonoBehaviour
{
    [SerializeField] float speed = 500f;
    [SerializeField] GameObject explosionBetwenBulletsPrefab;
    [SerializeField] float tiempoParaExplotar = 1.5f;

    [Header("Sonidos")]
    [SerializeField] AudioClip sonidoExplosion;

    private Animator animator;
    private bool haExplotado = false;
    private Vector2 minCamara;
    private Vector2 maxCamara;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //Se obtienen las coordenadas máximas y minimas de la cámara
        minCamara = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));    //(el 0,0 es el borde inferior izquierdo)
        maxCamara = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));    //(el 1,1 es el borde superior derecho de la cámara)

        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        //Si sale de pantalla, se destruye el dipsaro
        if (transform.position.x > maxCamara.x || transform.position.y > maxCamara.y || transform.position.y < minCamara.y)
        {
            Destroy(gameObject);
        }

        //Se va reduciendo la cuenta atrás
        tiempoParaExplotar -= Time.deltaTime;
        if (tiempoParaExplotar <= 0 && !haExplotado) {
            Explotar();
        }



    }

    private void OnTriggerEnter2D(Collider2D elOtro)
    {



        if (elOtro.gameObject.CompareTag("Enemy") || elOtro.gameObject.CompareTag("EnemyBullet") || elOtro.gameObject.CompareTag("Shield"))
        {
            if (elOtro.gameObject.CompareTag("Enemy"))
            {
                float experienciaGanada = elOtro.gameObject.GetComponent<EnemyParameters>().puntosDeExperiencia;
                GestorPlayer.Instance.SumarExp(experienciaGanada);
            }

            if (!haExplotado) {
                Explotar();
            }
            
        }
        /*else if (elOtro.gameObject.CompareTag("Shield"))
        {

            //Poner sonido como opaco
            Destroy(gameObject);
        }*/

    }

    void Explotar() {
        haExplotado = true;
        speed = 0f;
        animator.SetTrigger("Explotar");    //Se activa la animación de explosión
        GestorSonido.Instance.EjecutarSonido(sonidoExplosion);
    }

    void DestruirBomba() {
        Destroy(gameObject);
    }


}
