using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UpgradeObject;

public class TarjetaUpgrade : MonoBehaviour
{


    [SerializeField] TextMeshProUGUI textoNombre;
    [SerializeField] TextMeshProUGUI textoDescripcion;
    [SerializeField] Image imagenIcono;
    [SerializeField] Image fondoRareza;

    [Header("Sonidos")]
    [SerializeField] AudioClip sonidoComprado;

    private UpgradeState upgradeQueRepresenta;




    public void CargarDatosTarjeta(UpgradeState upgrade, Dictionary<enumRarezas, Color> coloresPorRareza) {
        upgradeQueRepresenta = upgrade;
        textoNombre.text = upgrade.upgrade.Nombre;
        textoDescripcion.text = upgrade.upgrade.Descripcion;
        fondoRareza.color = coloresPorRareza[upgrade.upgrade.Rareza];
        imagenIcono.sprite = upgrade.upgrade.IconoUpgrade;

    }


    public void onClick() {
        GestorSonido.Instance.EjecutarSonido(sonidoComprado);
        GestorPlayer.Instance.AplicarUpgrade(upgradeQueRepresenta);

    
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
