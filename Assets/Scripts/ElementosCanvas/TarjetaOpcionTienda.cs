using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UpgradeObject;

public class TarjetaOpcionTienda : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI textoNombre;
    [SerializeField] TextMeshProUGUI textoDescripcion;
    [SerializeField] TextMeshProUGUI textoCoste;
    [SerializeField] Image imagenIcono;
    [SerializeField] Image fondoRareza;
    private UpgradeState upgradeQueRepresenta;
    Dictionary<enumRarezas, Color> coloresRarezas = new Dictionary<enumRarezas, Color>();

    public void CargarDatosTarjeta(UpgradeState upgrade, Dictionary<enumRarezas, Color> coloresPorRareza)
    {
        upgradeQueRepresenta = upgrade;
        textoNombre.text = upgrade.upgrade.Nombre;
        textoDescripcion.text = upgrade.upgrade.Descripcion;
        fondoRareza.color = coloresPorRareza[upgrade.upgrade.Rareza];
        imagenIcono.sprite = upgrade.upgrade.IconoUpgrade;

        if (upgrade.obtenida == true)
        {
            textoCoste.text = "Agotado";
            textoCoste.color = new Color(1f, 0f, 0f, 0.8f); //Se pone en rojo
        }
        else {
            textoCoste.text = "Coste: " + upgrade.upgrade.CosteDePuntos + " Pts.";
        }

        coloresRarezas = coloresPorRareza;

    }

    public void onClick() {

        if (GestorPlayer.Instance.puntuacionTotal >= upgradeQueRepresenta.upgrade.CosteDePuntos && upgradeQueRepresenta.obtenida == false)
        {
            GestorPlayer.Instance.ComprarMejora(upgradeQueRepresenta);

            //Tras comprarla, se vuelven a poner los datos pero actualizados
            CargarDatosTarjeta(upgradeQueRepresenta, coloresRarezas);
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
