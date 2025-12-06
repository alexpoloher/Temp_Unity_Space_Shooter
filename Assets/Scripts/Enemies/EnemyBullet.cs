using UnityEngine;

public class EnemyBullet : MonoBehaviour
{

    [SerializeField] float speed = 500f;

    private Vector2 minCamara;
    private Vector2 maxCamara;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Se obtienen las coordenadas máximas y minimas de la cámara
        minCamara = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));    //(el 0,0 es el borde inferior izquierdo)
        maxCamara = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));    //(el 1,1 es el borde superior derecho de la cámara)

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.Self);

        //Si sale de pantalla, se destruye el dipsaro
        if (transform.position.x > maxCamara.x || transform.position.x < minCamara.x  || transform.position.y > maxCamara.y || transform.position.y < minCamara.y)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D elOtro)
    {
        //Si choca con el jugador, la bala se destruye
        if (elOtro.gameObject.CompareTag("Player") || elOtro.gameObject.CompareTag("PlayerShot") || elOtro.gameObject.CompareTag("Saw"))
        { 
            Destroy(gameObject);
        }
    }
}
