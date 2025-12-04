using UnityEngine;

public class PlayerShot : MonoBehaviour
{

    [SerializeField] float speed = 500f;
    [SerializeField] GameObject explosionBetwenBulletsPrefab;

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
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        //Si sale de pantalla, se destruye el dipsaro
        if (transform.position.x > maxCamara.x || transform.position.y > maxCamara.y || transform.position.y < minCamara.y)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D elOtro)
    {
        if (elOtro.gameObject.CompareTag("Enemy") || elOtro.gameObject.CompareTag("EnemyBullet"))
        {
            if (elOtro.gameObject.CompareTag("Enemy"))
            {
                float experienciaGanada = elOtro.gameObject.GetComponent<EnemyParameters>().puntosDeExperiencia;
                GestorPlayer.Instance.SumarExp(experienciaGanada);
            }
            if (elOtro.gameObject.CompareTag("EnemyBullet"))
            {
                Instantiate(explosionBetwenBulletsPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        } else if (elOtro.gameObject.CompareTag("Shield")) {
            
            //Poner sonido como opaco
            Destroy(gameObject);
        }

    }
}
