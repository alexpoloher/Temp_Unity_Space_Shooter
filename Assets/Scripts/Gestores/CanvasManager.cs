using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UpgradeObject;

public class CanvasManager : MonoBehaviour
{
    [Header("ElementosDelCanvas")]
    [SerializeField] Image iconoPowerUp;
    [SerializeField] Image barraVida;
    [SerializeField] Image barraExp;
    [SerializeField] TextMeshProUGUI textoNivel;
    [SerializeField] Image backGroundFaseSuperada;
    [SerializeField] Image backGroundCuentaAtrasPasarNivel;
    [SerializeField] List<GameObject> tarjetasMejora;
    [SerializeField] TextMeshProUGUI textoPuntuacionTotal;


    [Header("ImagenesPowerUps")]
    [SerializeField] Sprite iconoPowerUpEscudo;
    [SerializeField] Sprite iconoPowerUpRecharge;
    [SerializeField] Sprite iconoPowerUpMultiFire;
    [SerializeField] Sprite iconoPowerUpBomb;
    [SerializeField] Sprite iconoPowerUpSaw;



    private float vidaMaxima = 100;
    private float puntuacionParaSiguienteNivel;
    private float nivelActual = 1;
    private bool seEstaPasandoNivel = false;
    private float tiempoRestantePasarNivel = 3.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vidaMaxima = GestorPlayer.Instance.vidaMaxima;
        puntuacionParaSiguienteNivel = GestorPlayer.Instance.puntuacionParaSiguienteNivel;

        nivelActual = GestorPlayer.Instance.nivelActual;
        if (nivelActual >= GestorPlayer.Instance.nivelMaximo)
        {
            textoNivel.text = "LVL. MAX"; ;
        }
        else
        {
            textoNivel.text = "LVL. " + nivelActual;
        }

    }


    // Update is called once per frame
    void Update()
    {
        //Se actualiza la barra de vida y experiencia
        barraVida.fillAmount = GestorPlayer.Instance.vidaActual / vidaMaxima;
        if (GestorPlayer.Instance.nivelActual == GestorPlayer.Instance.nivelMaximo)
        {
            barraExp.fillAmount = 1;
        }
        else {
            barraExp.fillAmount = GestorPlayer.Instance.puntuacionActual / puntuacionParaSiguienteNivel;
        }

        if (textoPuntuacionTotal != null) {
            textoPuntuacionTotal.text = "" + GestorPlayer.Instance.puntuacionTotal + " Pts.";
        }

        if (seEstaPasandoNivel) {
            tiempoRestantePasarNivel -= Time.unscaledDeltaTime;
            //Se indica que el primer parametro (el 0), se escriba con 1 número
            backGroundCuentaAtrasPasarNivel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0:0}", tiempoRestantePasarNivel);

            if (tiempoRestantePasarNivel <= 0) {
                GestorPlayer.Instance.PasarDeNivel();
            }
        }

    }

    public void ActualizarPuntosTotales(float puntuacionTotal) {
        textoPuntuacionTotal.text = "" + puntuacionTotal + " Pts.";
    }

    public void ActualizarVidaMaxima(float nuevaVidaMaxima) { 
        vidaMaxima = nuevaVidaMaxima;
    }


    public void ModificarTextoNivel(int nivelNuevo) {
        if (nivelNuevo >= GestorPlayer.Instance.nivelMaximo)
        {
            textoNivel.text = "LVL. MAX"; ;
        }
        else
        {
            textoNivel.text = "LVL. " + nivelNuevo;
        }
    }

    public void RecibirPowerUp(string tipoPowerUp) {

        iconoPowerUp.gameObject.SetActive(true);
        if (tipoPowerUp.Equals("Shield"))
        {
            iconoPowerUp.sprite = iconoPowerUpEscudo;
        }
        else if (tipoPowerUp.Equals("Recharge"))
        {
            iconoPowerUp.sprite = iconoPowerUpRecharge;
        }
        else if (tipoPowerUp.Equals("MultiFire"))
        {
            iconoPowerUp.sprite = iconoPowerUpMultiFire;
        }
        else if (tipoPowerUp.Equals("Bomb"))
        {
            iconoPowerUp.sprite = iconoPowerUpBomb;
        }
        else if (tipoPowerUp.Equals("Saw"))
        {
            iconoPowerUp.sprite = iconoPowerUpSaw;
        }
    }

    public void TerminarPowerUp() {
        iconoPowerUp.gameObject.SetActive(false);

    }

    public void ActualizarTiempoPowerUp(float tiempoRestantePowerUp) {
        //Se indica que el primer parametro (el 0), se escriba con 2 números
        iconoPowerUp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}", tiempoRestantePowerUp);
    }

    public void MostrarFaseSuperada(List<UpgradeState> upgrades, Dictionary<enumRarezas, Color> coloresPorRareza) {

        bool hayAlmenosUnaTarjeta = false;

        //Se comprueba que haya info para mostarr. Así si solo hay info para 2 tarjetas, solo se muestran 2
        if (upgrades.Count > 0) {
            hayAlmenosUnaTarjeta = true;
        }


        if (hayAlmenosUnaTarjeta) {
            int indice = 0;
            foreach (UpgradeState upgrade in upgrades)
            {
                if (tarjetasMejora[indice] != null)
                {
                    tarjetasMejora[indice].gameObject.GetComponent<TarjetaUpgrade>().CargarDatosTarjeta(upgrade, coloresPorRareza);
                    indice++;
                }


            }

            backGroundFaseSuperada.gameObject.SetActive(true);
            //Así si solo hay info para 2 tarjetas, solo se muestran 2
            for (int i = 0; i < upgrades.Count; i++)
            {
                tarjetasMejora[i].gameObject.SetActive(true);
            }
        }

        
      
    }

    //ESTO ES SOLO PARA LA PRUEBA
    /*IEnumerator Tempo() {

        yield return new WaitForSeconds(3f);
        IniciarCuentaAtras();
    }*/

    //A este se le llama cuando tras elegir mejora, se le da a continuar
    
    public void IniciarCuentaAtras()
    {
        backGroundFaseSuperada.gameObject.SetActive(false);
        backGroundCuentaAtrasPasarNivel.gameObject.SetActive(true);
        seEstaPasandoNivel = true;
    }



}
