using System;
using UnityEngine;

//En esta clase se indica si el jugador ya ha cogido X mejora o no, así se usa esta para modificar valores y reiniciarlos en cada partida
//Si cada habilidad va a tener niveles, aquí se indica a qué nivel está
public class UpgradeState
{
    public UpgradeObject upgrade;
    public bool obtenida;

    public UpgradeState(UpgradeObject upgradeRecibida) { 
        upgrade = upgradeRecibida;
        obtenida = false;
    }




}
