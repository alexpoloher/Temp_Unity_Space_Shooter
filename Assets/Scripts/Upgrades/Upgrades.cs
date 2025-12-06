using UnityEngine;

public class Upgrades : MonoBehaviour
{

    //Lista con todas las mejoraas posibles
    Upgrade[] listaUpgrades = new Upgrade[] {

        new Upgrade{Name= "primera", Descripcion = "primera primera primera", Rareza = "Común" },
        new Upgrade{Name= "segunda", Descripcion = "segunda segunda segunda", Rareza = "Rara" },
        new Upgrade{Name= "tercera", Descripcion = "tercera tercera tercera", Rareza = "Legendaria" },
        new Upgrade{Name= "cuarta", Descripcion = "cuarta cuarta cuarta", Rareza = "Común" },

    };




    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
