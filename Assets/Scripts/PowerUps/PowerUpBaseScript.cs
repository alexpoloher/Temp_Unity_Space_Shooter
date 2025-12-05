using UnityEngine;

public class PowerUpBaseScript : MonoBehaviour
{

    [SerializeField] float speed = 10f;
    [SerializeField] public string tipoPowerUp = "";

    private Vector2 minCamara;
    private Vector2 maxCamara;
    private Vector3 direccionMovimiento = Vector3.left;

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
        transform.Translate(direccionMovimiento * speed * Time.deltaTime);

        //Si sale de pantalla, se destruye el dipsaro
        if (transform.position.x < minCamara.x)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D elOtro)
    {
        if (elOtro.gameObject.CompareTag("Player")) {
            Destroy(gameObject);
        }
    }
}
