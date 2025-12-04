using System;
using System.Globalization;
using UnityEngine;

//Esta clase es la que reprsenta cada mejora, no se tiene en cuenta si ya está usada o no en esta partida

[CreateAssetMenu(fileName = "UpgradeObject", menuName = "Scriptable Objects/UpgradeObject")]
public class UpgradeObject : ScriptableObject
{
    public enum enumRarezas
    {
        Comun,
        Rara,
        Legendaria,
    }
    public enum enumTipoUpgrade
    {
        Vida,
        DelayDisparo,
        PorcentajeAparicionPowerUp,
        DuracionPowerUp,
        PuntosExperiencia
    }



    [SerializeField] private string nombre;
    [SerializeField] private string descripcion;
    [SerializeField] private enumRarezas rareza;
    [SerializeField] private enumTipoUpgrade tipoUpgrade;
    [SerializeField] float cantidadEfecto;  //Por ejemplo si es de tipo vida y aquí vale 20, pues es que se suma 20 de vida
    [SerializeField] Sprite iconoUpgrade;
    [SerializeField] int costeDePuntos;

    public string Nombre { get { return nombre; } }
    public string Descripcion { get { return descripcion; } }
    public enumRarezas Rareza { get { return rareza; } }
    public enumTipoUpgrade TipoUpgrade { get { return tipoUpgrade; } }
    public float CantidadEfecto { get { return cantidadEfecto; } }
    public Sprite IconoUpgrade { get { return iconoUpgrade; } }
    public int CosteDePuntos { get { return costeDePuntos; } }

    public int ObtenerPorcentajesRarezas(enumRarezas rareza)
    {
        int porcentajeObtenido = 0;

        switch (rareza)
        {

            case enumRarezas.Comun:
                porcentajeObtenido = 50;
                break;
            case enumRarezas.Rara:
                porcentajeObtenido = 30;
                break;
            case enumRarezas.Legendaria:
                porcentajeObtenido =  20;
                break;
        }

        return porcentajeObtenido;

    }

}
