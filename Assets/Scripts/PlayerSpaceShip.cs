using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpaceShip : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float acceleration = 300f; //En m/s2En 0.3 seg, alcanza la velocidad máxima

    [Header("Controls")]
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference shoot;

    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject projectilePrefabBase;
    [SerializeField] GameObject projectilePrefabRecharge;
    [SerializeField] GameObject projectilePrefabMultiFire;
    [SerializeField] GameObject projectilePrefabBomb;

    [Header("Camara")]
    [SerializeField] GameObject Camara;
    //Límites de la cámara para que el jugador no se salga de pantalla:
    [SerializeField] float limiteSuperior, limiteInferior, limiteDerecha, limiteIzquierda;

    [Header("Golpeo")]
    [SerializeField] float tiempoInmunidad = 1.0f;
    [SerializeField] GameObject explosionPlayerPrefab;

    [Header("PowerUps")]
    [SerializeField] GameObject escudoPrefab;
    [SerializeField] GameObject sawPrefab;

    [Header("Sonidos")]
    [SerializeField] AudioClip sonidoDisparo;
    [SerializeField] AudioClip sonidoVida;
    [SerializeField] AudioClip sonidoCogerEscudo;
    [SerializeField] AudioClip sonidoPowerOff;
    [SerializeField] AudioClip sonidoRecharge;
    [SerializeField] AudioClip sonidoMultiFire;
    [SerializeField] AudioClip sonidoBomb;
    [SerializeField] AudioClip sonidoSaw;

    Vector2 rawMove;
    Vector2 currentVelocity = Vector2.zero;

    const float rawMovethresholdForBraking = 0.1f;
    private bool puedeDisparar = true;
    private Animator animator;
    private Vector3 posInicial;
    private bool esInmune = false;
    private bool estaReapareciendo = false;
    private SpriteRenderer spriteRenderer;
    private Blink materialBlink;
    private bool tienePowerUp = false;
    private string tipoPowerUp = "";
    private bool tieneEscudo;
    private GameObject escudoGenerado;
    private GameObject sierraGenerada;
    private float delayDisparosOriginal = 0.5f; //Este se mantendrá para que si se modifica con el Power Up, se pueda volver al valor original
    private float delayDisparos = 0.5f;

    //Ocurre cuando se habilita para su suso este componente
    private void OnEnable()
    {
        //Se habilita en este momento la lectura de estos inputs
        move.action.Enable();
        shoot.action.Enable();

        //El performed es la llamada que ocurrirá sobre los métodos que añadas, cuando ocuirra el uso de la acción de move 
        move.action.started += OnMove;
        move.action.performed += OnMove;
        move.action.canceled += OnMove;

        shoot.action.started += OnShoot;


        //Se obtiene la referencia al animator propio 
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        posInicial = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        materialBlink = GetComponent<Blink>();
        delayDisparos = GestorPlayer.Instance.delayDisparos;
        delayDisparosOriginal = delayDisparos;
    }


    void Update()
    {
        if (GestorEstadoPausa.Instance.estaPausado == false) {
            if (estaReapareciendo == false)
            {
                //controlar para que se pueda frenar, solo cuando no estamos pulsando se ejecuta esto
                if (rawMove.magnitude < rawMovethresholdForBraking)
                { 
                    currentVelocity *= 0.1f * Time.deltaTime; //Añade un rozamineto, cada vez se moverá menos
                }

                currentVelocity += rawMove.normalized * acceleration * Time.deltaTime; //M/S2 * S = m/s (Es decir, así se obtiene la velocidad)

                /*if (rawMove.y == 0) {
                    currentVelocity.y = 0;
                }
                if (rawMove.x == 0) { 
                    currentVelocity.x = 0;
                }*/


                float linearVelocity = currentVelocity.magnitude;   //Magnitud es la longitud de un vector. Así se obtiene el vector de movimiento
                linearVelocity = Mathf.Clamp(linearVelocity, 0, maxSpeed);  //Recorta por los lados. Para controlar si se pasa de velocidad.  Ni más pequeño de 0 ni más grande que maxSpeed
                currentVelocity = currentVelocity.normalized * linearVelocity;

                AplicarMovimiento();
            }
        }



    }


    private void AplicarMovimiento()
    {
        //Se comprueba si se ha pasado de los límites. Si sí, se evita, situando al jugador dentro de los límites
        //Esto se hace cada frame
        transform.Translate(currentVelocity * Time.deltaTime);

        Vector3 posicionAct = transform.position;
        posicionAct.x = Mathf.Clamp(posicionAct.x, limiteIzquierda, limiteDerecha);
        posicionAct.y = Mathf.Clamp(posicionAct.y, limiteInferior, limiteSuperior);
        transform.position = posicionAct;

    }
    private void OnDisable()
    {
        move.action.Disable();
        shoot.action.Disable();

        //Se quitan esos eventos
        move.action.started -= OnMove;
        move.action.performed -= OnMove;
        move.action.canceled -= OnMove;

        shoot.action.started -= OnShoot;

    }


    private void OnMove(InputAction.CallbackContext obj)
    {
        if (GestorEstadoPausa.Instance.estaPausado == false || estaReapareciendo)
        {
            rawMove = obj.ReadValue<Vector2>();
        }
 
    }

    private void OnShoot(InputAction.CallbackContext obj)
    {
        if (GestorEstadoPausa.Instance.estaPausado == false) {

            if (puedeDisparar && !estaReapareciendo)
            {

 
                if (tienePowerUp && tipoPowerUp.Equals("MultiFire"))
                {
                    for (int i = 0; i <= transform.childCount - 1; i++)
                    {
                        Vector3 posicionDisparo = transform.GetChild(i).position;  //Se coge la posición del cañon, que es de donde va a salir la bala
                        Instantiate(projectilePrefab, posicionDisparo, transform.GetChild(i).rotation);
                    }
                    //Tras disparar, tardará X segundos en recargar
                    StartCoroutine(RecargarDisparo());
                }
                else
                {
                    Vector3 posicionDisparo = transform.GetChild(0).position;  //Se coge la posición del cañon, que es de donde va a salir la bala
                    Instantiate(projectilePrefab, posicionDisparo, Quaternion.identity);
                    //Tras disparar, tardará X segundos en recargar
                    StartCoroutine(RecargarDisparo());
                }
                GestorSonido.Instance.EjecutarSonido(sonidoDisparo);


            }
        }
    }

    IEnumerator RecargarDisparo()
    {
        puedeDisparar = false;
        yield return new WaitForSeconds(delayDisparos);
        puedeDisparar = true;

    }


    private void OnTriggerEnter2D(Collider2D elOtro)
    {
        if (esInmune == false && !estaReapareciendo && (elOtro.gameObject.CompareTag("Enemy") || elOtro.gameObject.CompareTag("EnemyBullet")))
        {
            //Destroy(gameObject);
            if (tieneEscudo) {
                GestorPlayer.Instance.TerminarPowerUp();
            }
            else {
                float damageRecibido = elOtro.gameObject.GetComponent<EnemyParameters>().damageToDeal;
                StartCoroutine(RecibirGolpe(damageRecibido));

                StartCoroutine(DetenerMovimiento());
            }

        }
        else if(elOtro.gameObject.CompareTag("PowerUp") && !estaReapareciendo) {
            RecogerPowerUp(elOtro.gameObject);

        
        }
    }

    void RecogerPowerUp(GameObject powerUpGameObject) {

        if (powerUpGameObject.GetComponent<PowerUpBaseScript>().tipoPowerUp.Equals("Vida"))
        {
            //Si se recoge este powerUp, se suma la cantidad de vida indicada
            float vidaARecuperar = powerUpGameObject.GetComponent<PowerUpVida>().vidaARecuperar;
            GestorPlayer.Instance.SumarVida(vidaARecuperar);
            GestorSonido.Instance.EjecutarSonido(sonidoVida);
        } else if ((tienePowerUp == false || tipoPowerUp == "Shield") && powerUpGameObject.GetComponent<PowerUpBaseScript>().tipoPowerUp.Equals("Shield")) {

            //Solo se genera un escudo. Si ya lo tengo, no genero otro
            if (tienePowerUp == false) {
                escudoGenerado = Instantiate(escudoPrefab, transform.position, Quaternion.identity);
                escudoGenerado.transform.parent = transform;    //Se establece que el nuevo padre del escudo es la nave del player
            }
            tienePowerUp = true;
            tipoPowerUp = "Shield";
            tieneEscudo = true;
            GestorPlayer.Instance.RecibirPowerUp("Shield");
            GestorSonido.Instance.EjecutarSonido(sonidoCogerEscudo);

        } else if ((tienePowerUp == false || tipoPowerUp == "Recharge") && powerUpGameObject.GetComponent<PowerUpBaseScript>().tipoPowerUp.Equals("Recharge")) {
            //Solo se aplica el efecto una vez. Si se coge de nuevo, solo se le sube el contador
            if (!tienePowerUp) {
                delayDisparos = delayDisparos / 2;
            }
            
            tienePowerUp = true;
            tipoPowerUp = "Recharge";
            //Se cambia el proyectil que se dispara
            projectilePrefab = projectilePrefabRecharge;
            //Este Power Up permite disparar más frecuentemente
            GestorPlayer.Instance.RecibirPowerUp("Recharge");
            GestorSonido.Instance.EjecutarSonido(sonidoRecharge);
        } else if ((tienePowerUp == false || tipoPowerUp == "MultiFire") && powerUpGameObject.GetComponent<PowerUpBaseScript>().tipoPowerUp.Equals("MultiFire"))
        {
            //Este Power Up permite disparar en 3 direcciones
            tienePowerUp = true;
            tipoPowerUp = "MultiFire";
            //Se cambia el proyectil que se dispara
            projectilePrefab = projectilePrefabMultiFire;
            GestorPlayer.Instance.RecibirPowerUp("MultiFire");
            GestorSonido.Instance.EjecutarSonido(sonidoMultiFire);
        } else if ((tienePowerUp == false || tipoPowerUp == "Bomb") && powerUpGameObject.GetComponent<PowerUpBaseScript>().tipoPowerUp.Equals("Bomb"))
        {
            //Este Power Up permite disparar una bomba, mayor rango de detección de enemigos y mayor rango de daño de la explosión
            tienePowerUp = true;
            tipoPowerUp = "Bomb";
            //Se cambia el proyectil que se dispara
            projectilePrefab = projectilePrefabBomb;
            GestorPlayer.Instance.RecibirPowerUp("Bomb");
            GestorSonido.Instance.EjecutarSonido(sonidoBomb);
        } else if ((tienePowerUp == false || tipoPowerUp == "Saw") && powerUpGameObject.GetComponent<PowerUpBaseScript>().tipoPowerUp.Equals("Saw"))
        {
            //Solo se genera una sierra. Si ya lo tengo, no genero otro
            if (tienePowerUp == false)
            {
                sierraGenerada = Instantiate(sawPrefab, transform.position, Quaternion.identity);
                sierraGenerada.transform.parent = transform;    //Se establece que el nuevo padre de la sierra es la nave del player
            }
            //Este Power Up genera una sierra que orbita a tu alrededor y elimina todo lo que toca
            tienePowerUp = true;
            tipoPowerUp = "Saw";
            GestorPlayer.Instance.RecibirPowerUp("Saw");
            GestorSonido.Instance.EjecutarSonido(sonidoSaw);
        }
    }

    public void TerminarPowerUp() {

        if (tipoPowerUp.Equals("Shield")) {
            tieneEscudo = false;
            Destroy(escudoGenerado);
        } else if (tipoPowerUp.Equals("Recharge")) {
            delayDisparos = GestorPlayer.Instance.delayDisparos;
        } else if (tipoPowerUp.Equals("Saw"))
        {
            Destroy(sierraGenerada);
        }

        tipoPowerUp = "";
        tienePowerUp = false;
        projectilePrefab = projectilePrefabBase;
        GestorSonido.Instance.EjecutarSonido(sonidoPowerOff);
    }


    IEnumerator RecibirGolpe(float damageRecibido)
    {
        if((GestorPlayer.Instance.vidaActual - damageRecibido) <= 0)
        {
            GestorPlayer.Instance.QuitarVida(damageRecibido);
            //GestorPlayer.Instance.vidaActual = 0;
            if (tienePowerUp) {
                GestorPlayer.Instance.TerminarPowerUp();
            }
            Destroy(gameObject);
        }
        else
        {
            GestorPlayer.Instance.QuitarVida(damageRecibido);
            //GestorPlayer.Instance.vidaActual -= damageRecibido;
        }

        Instantiate(explosionPlayerPrefab, transform.position, Quaternion.identity);    //Se muestra la explosión
        transform.position = transform.position + new Vector3(0, 0, -100);  //Se lelva detrás del fondo para que no se vea

        //Se desactivan las colisiones hasta que reaparezca
        gameObject.GetComponent<Collider2D>().enabled = false;
        if (tipoPowerUp.Equals("Saw") && sierraGenerada != null) { 
            sierraGenerada.GetComponent<Collider2D>().enabled = false;
        }
        
        yield return new WaitForSeconds(0.5f);
        transform.position = posInicial;

        //Se vuelven a activar las colisiones
        //Se desactivan las colisiones hasta que reaparezca
        gameObject.GetComponent<Collider2D>().enabled = true;
        if (tipoPowerUp.Equals("Saw") && sierraGenerada != null)
        {
            sierraGenerada.GetComponent<Collider2D>().enabled = true;
        }
        esInmune = true;    //Durante un poco de tiempo, el jugador será inmune y no se podrá tocar
        StartCoroutine(ParpadeoInmunidad());
        yield return new WaitForSeconds(tiempoInmunidad);
        esInmune = false;

    }

    //Durante un poco de tiempo, se detiene el movimiento, para darse cuenta de la situación
    IEnumerator DetenerMovimiento()
    {
        estaReapareciendo = true;
        currentVelocity = new Vector2 (0, 0);
        rawMove = new Vector3(0, 0, 0);    //El movimiento con el que empiezas es 0
        yield return new WaitForSeconds(0.5f + (tiempoInmunidad / 2));
        estaReapareciendo = false;

    }

    //Durante el tiempo que se es inmune, se va a estar parpadeando para mostrar esa inmunidad
    IEnumerator ParpadeoInmunidad()
    {
        while (esInmune)
        {
            spriteRenderer.material = materialBlink.blink;  //Se accede al scrit que tiene el material de parpadeo
            yield return new WaitForSeconds(tiempoInmunidad / 6);
            spriteRenderer.material = materialBlink.original;
            yield return new WaitForSeconds(tiempoInmunidad / 6);
        }
    }


}
