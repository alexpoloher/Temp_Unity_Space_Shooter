using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestorPlayer : MonoBehaviour
{

    public static GestorPlayer Instance;
    public float vidaActual;
    public float vidaMaxima = 100;
    public float vidaInicial = 100;
    public float duracionMaximaPowerUpOriginal = 15.0f;
    public float duracionMaximaPowerUp = 15.0f;
    public float puntuacionActual = 0;  //Puntuación en cada nivel
    public float puntuacionTotal = 0;
    public float puntuacionParaSiguienteNivel;
    public float[] puntuacionNecesariaPorNivel = { 300, 1000, 1500, 1750, 2000 };   //Necesita 300 puntos para subir a lvl2...y así 

    public int nivelActual = 1;
    public int nivelMaximo = 6;
    public float delayDisparos = 0.5f;
    public float delayDisparosOriginal = 0.5f;
    public int[] porcentajesAparicionEnemigos = { 50, 50, 0, 0, 0, 0, 0, 0 };   //En el nivel uno, los dos primeros enemigos tienen 50/50 de salir
    public int[] porcentajesAparicionPowerUps = { 50, 50, 0, 0, 0, 0};       //En el nivel uno, tanto la vida como el escudo tienen 50/50 de salir
    public float porcentajeAparicionPowerUpOriginal = 30f;  //Cada vez que matas a  un enemigo, hay 30% de posibilidad de que spawnee un PowerUp
    public float porcentajeAparicionPowerUp = 30f;  //Cada vez que matas a  un enemigo, hay 30% de posibilidad de que spawnee un PowerUp
    public float multiplicadorExpOriginal = 1f;
    public float multiplicadorExp = 1f;

    //Gestiona cuándo aparece el timer de power up en pantalla
    private float tiempoQuedaPowerUp;


    private GameObject playerSpaceShip;
    private bool tienePowerUp = false;
    private float tiempoRestantePowerUp;
    private CanvasManager canvasManager;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += onSceneLoaded;

        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Update()
    {

        //Si tiene power up, se calcula el tiempo restante
        if (tienePowerUp) {
            tiempoRestantePowerUp -= Time.deltaTime;
            //Se llama al canvas para que actualice el tiempo
            canvasManager.ActualizarTiempoPowerUp(tiempoRestantePowerUp);
            if (tiempoRestantePowerUp <= 0) {
                TerminarPowerUp();
            }
        }

        print(vidaMaxima);
        print(vidaActual);
    }



    public void RecibirPowerUp(string tipoPowerUp) {
        tienePowerUp = true;
        tiempoRestantePowerUp = duracionMaximaPowerUp;
        canvasManager.RecibirPowerUp(tipoPowerUp);


    }

    public void TerminarPowerUp() {
        tienePowerUp = false;
        tiempoRestantePowerUp = duracionMaximaPowerUp;
        if (playerSpaceShip != null) {
            playerSpaceShip.GetComponent<PlayerSpaceShip>().TerminarPowerUp();
        }
        canvasManager.TerminarPowerUp();
    }


    //Al comenzar el juego, se establecen los valores de inicio
    void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Se obtiene la referencia al canvas manager de cada escena
        canvasManager = FindAnyObjectByType<CanvasManager>();
        playerSpaceShip = GameObject.FindWithTag("Player");
        if (scene.name.Equals("SampleScene"))
        {
            ReiniciarParametros();
        } else if (scene.name.Equals("SceneFase3")) {
            canvasManager.ActualizarPuntosTotales(puntuacionTotal);
        }

    }

    //Al comenzar el juego, se establecen los valores de inicio
    void ReiniciarParametros()
    {

        playerSpaceShip = GameObject.FindWithTag("Player");
        vidaActual = vidaInicial;
        vidaMaxima = vidaInicial;
        puntuacionActual = 0;
        nivelActual = 1;
        puntuacionParaSiguienteNivel = puntuacionNecesariaPorNivel[nivelActual - 1];
        porcentajesAparicionEnemigos = new int[] { 50, 50, 0, 0, 0, 0, 0, 0 };
        porcentajesAparicionPowerUps = new int[] { 50, 50, 0, 0, 0, 0};
        delayDisparos = delayDisparosOriginal;
        porcentajeAparicionPowerUp = porcentajeAparicionPowerUpOriginal;
        duracionMaximaPowerUp = duracionMaximaPowerUpOriginal;
        multiplicadorExp = multiplicadorExpOriginal;

        //De PowerUps
        tienePowerUp = false;
        tiempoRestantePowerUp = duracionMaximaPowerUp;

    }


    public void SumarVida(float vidaSumada)
    {
        if ((vidaActual + vidaSumada) >= vidaMaxima)
        {
            vidaActual = vidaMaxima;
        }
        else
        {
            vidaActual += vidaSumada;
        }
    }


    public void QuitarVida(float damageRecibido)
    {
        if ((vidaActual - damageRecibido) <= 0)
        {
            vidaActual = 0;
        }
        else
        {
            vidaActual -= damageRecibido;
        }
    }

    public void AplicarUpgrade(UpgradeState upgrade) {

        upgrade.obtenida = true;
        switch (upgrade.upgrade.TipoUpgrade) {

            case UpgradeObject.enumTipoUpgrade.Vida:
                vidaMaxima += upgrade.upgrade.CantidadEfecto;
                canvasManager.ActualizarVidaMaxima(vidaMaxima);
                break;
            case UpgradeObject.enumTipoUpgrade.DelayDisparo:
                delayDisparos -= upgrade.upgrade.CantidadEfecto;
                break;
            case UpgradeObject.enumTipoUpgrade.DuracionPowerUp:
                duracionMaximaPowerUp += upgrade.upgrade.CantidadEfecto;
                break;
            case UpgradeObject.enumTipoUpgrade.PorcentajeAparicionPowerUp:
                porcentajeAparicionPowerUp += upgrade.upgrade.CantidadEfecto;
                break;
            case UpgradeObject.enumTipoUpgrade.PuntosExperiencia:
                multiplicadorExp *= upgrade.upgrade.CantidadEfecto;
                break;
        }

        //Se inicia cuenta atrás para pasar de nivel. Una vez termina, desde el canvas se llam al método PasarNivel(9 de este script
        canvasManager.IniciarCuentaAtras();

    }

    public void SumarExp(float expRecibida)
    {
        if ((puntuacionActual + (expRecibida * multiplicadorExp)) >= puntuacionParaSiguienteNivel)
        {
            puntuacionActual = puntuacionParaSiguienteNivel;
            SubirNivel();
        }
        else
        {
            puntuacionActual += (expRecibida * multiplicadorExp);
        }
        puntuacionTotal += (expRecibida * multiplicadorExp);
        if ((nivelActual >= nivelMaximo - 1)  && SceneManager.GetActiveScene().name.Equals("SceneFase3")) {
            canvasManager.ActualizarPuntosTotales(puntuacionTotal);
        }
    }

    void SubirNivel()
    {
        if (nivelActual != nivelMaximo) {
            nivelActual++;
            puntuacionActual = 0;
            if (nivelActual != nivelMaximo) {
                puntuacionParaSiguienteNivel = puntuacionNecesariaPorNivel[nivelActual - 1];
            }
            CalcularNuevosPorcentajes();
            canvasManager.ModificarTextoNivel(nivelActual);
            if (nivelActual == 3 || nivelActual == 5) {
                FaseSuperada();

            }
        }

    }

    void CalcularNuevosPorcentajes() {
        switch (nivelActual) {

            case 1:
                porcentajesAparicionEnemigos = new int[] { 50, 50, 0, 0, 0, 0, 0, 0 };
                porcentajesAparicionPowerUps = new int[] { 50, 50, 0, 0, 0, 0};
                break;

            case 2:
                porcentajesAparicionEnemigos = new int[] { 25, 25, 50, 0, 0, 0, 0, 0 };
                porcentajesAparicionPowerUps = new int[] { 30, 30, 40, 0, 0, 0};
                break;

            case 3:
                porcentajesAparicionEnemigos = new int[] { 20, 20, 30, 30, 0, 0, 0, 0 };
                porcentajesAparicionPowerUps = new int[] { 20, 20, 30, 30, 0, 0};
                break;

            case 4:
                porcentajesAparicionEnemigos = new int[] { 20, 5, 25, 25, 25, 0, 0, 0 };
                porcentajesAparicionPowerUps = new int[] { 15, 15, 25, 25, 20, 0};
                break;

            case 5:
                porcentajesAparicionEnemigos = new int[] { 10, 5, 15, 20, 20, 20, 10, 0 };
                porcentajesAparicionPowerUps = new int[] { 15, 15, 20, 20, 20, 10 };
                break;

            case 6:
                porcentajesAparicionEnemigos = new int[] { 10, 5, 15, 10, 20, 20, 5, 15 };
                porcentajesAparicionPowerUps = new int[] { 15, 15, 20, 20, 20, 10 };
                break;


        }
    }

    void FaseSuperada() {
        //canvasManager.MostrarFaseSuperada();
        //Se seleccionan 3 mejoras. Y desde ahí dentro se llama al canvas manager para que las muestre
        Time.timeScale = 0f;    //Pausa el juego
        GestorUpgrades.Instance.SeleccionarMejorasAleatorias();
    }

    public void ComprarMejora(UpgradeState upgrade)
    {

        upgrade.obtenida = true;
        puntuacionTotal = puntuacionTotal - upgrade.upgrade.CosteDePuntos;  //Se hace el pago de puntos

        switch (upgrade.upgrade.TipoUpgrade)
        {
            case UpgradeObject.enumTipoUpgrade.Vida:
                vidaMaxima += upgrade.upgrade.CantidadEfecto;
                canvasManager.ActualizarVidaMaxima(vidaMaxima);
                break;
            case UpgradeObject.enumTipoUpgrade.DelayDisparo:
                delayDisparos -= upgrade.upgrade.CantidadEfecto;
                break;
            case UpgradeObject.enumTipoUpgrade.DuracionPowerUp:
                duracionMaximaPowerUp += upgrade.upgrade.CantidadEfecto;
                break;
            case UpgradeObject.enumTipoUpgrade.PorcentajeAparicionPowerUp:
                porcentajeAparicionPowerUp += upgrade.upgrade.CantidadEfecto;
                break;
            case UpgradeObject.enumTipoUpgrade.PuntosExperiencia:
                multiplicadorExp *= upgrade.upgrade.CantidadEfecto;
                break;
        }

    }

    public void PasarDeNivel() {

        Time.timeScale = 1f;    //Despausa el juego

        if (nivelActual == 3)
        {
            SceneManager.LoadScene("SceneFase2");
        }
        else
        {
            SceneManager.LoadScene("SceneFase3");
        }
    }

    void Start()
    {
        
    }





}
