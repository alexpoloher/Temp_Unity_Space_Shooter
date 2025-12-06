using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UpgradeObject;

public class CanvasShopManager : MonoBehaviour
{

    [SerializeField] List<GameObject> tarjetasOpcionesCompra;
    [SerializeField] GameObject interfazCompra;
    [SerializeField] TextMeshProUGUI textoPuntosDisponibles;

    [Header("Controls")]
    [SerializeField] InputActionReference shop;


    private bool tiendaAbierta = false;
    private List<UpgradeState> listaUpgradesState;   //Ene sta lista es donde se indica la info de cada mejora en esta partida, como por ejemplo si ya la he cogido
    Dictionary<enumRarezas, Color> coloresPorRareza = new Dictionary<enumRarezas, Color>();

    //Este es el que se encarga de detectar cuando pulsas P y muestra la pantalla de Pausa



    private void OnEnable()
    {
        //Se habilita en este momento la lectura de estos inputs
        shop.action.Enable();
        shop.action.started += OnShop;
        //pause.action.performed += OnPause;
        listaUpgradesState = GestorUpgrades.Instance.ObtenerListaTodasUpgrades();
        coloresPorRareza = GestorUpgrades.Instance.ObtenerDiccionarioColoresRarezas();
        CargarTarjetas();
    }

    private void OnDisable()
    {
        //Se deshabilita en este momento la lectura de estos inputs
        shop.action.started -= OnShop;
        shop.action.Disable();

    }


    //Al pulsar la Q, se abre la tienda
    private void OnShop(InputAction.CallbackContext obj){

        if (tiendaAbierta)
        {
            if (GestorEstadoPausa.Instance.tiposPausa == GestorEstadoPausa.tiposDePausaEnum.Tienda)
            {
                tiendaAbierta = false;
                CerrarTienda();
            }
        }
        else {
            //Solo se puede abrir la tienda si no está en pausa (del menú pausa)
            if (GestorEstadoPausa.Instance.estaPausado == false && GestorEstadoPausa.Instance.tiposPausa == GestorEstadoPausa.tiposDePausaEnum.Libre) {
                tiendaAbierta = true;
                AbrirTienda();
            }

        }

    }


    void AbrirTienda() {
        GestorEstadoPausa.Instance.estaPausado = true;
        GestorEstadoPausa.Instance.tiendaAbierta = true;
        GestorEstadoPausa.Instance.tiposPausa = GestorEstadoPausa.tiposDePausaEnum.Tienda;
        Time.timeScale = 0f;    //Pausa el juego
        interfazCompra.SetActive(true);
    }

    void CerrarTienda() {
        GestorEstadoPausa.Instance.estaPausado = false;
        GestorEstadoPausa.Instance.tiendaAbierta = false;
        GestorEstadoPausa.Instance.tiposPausa = GestorEstadoPausa.tiposDePausaEnum.Libre;
        Time.timeScale = 1f;    //Despausa el juego
        interfazCompra.SetActive(false);
    }


    void CargarTarjetas() {

        int indice = 0;
        foreach (UpgradeState upgrade in listaUpgradesState)
        {
            if (tarjetasOpcionesCompra[indice] != null) {
                tarjetasOpcionesCompra[indice].gameObject.GetComponent<TarjetaOpcionTienda>().CargarDatosTarjeta(upgrade, coloresPorRareza);
                indice++;
            }

        }
    }

    public void ClickBotonVolver() {
        tiendaAbierta = false;
        CerrarTienda();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tiendaAbierta) {
            textoPuntosDisponibles.text = "Puntos disponibles: " + GestorPlayer.Instance.puntuacionTotal + " Ptos.";
        }
    }
}
