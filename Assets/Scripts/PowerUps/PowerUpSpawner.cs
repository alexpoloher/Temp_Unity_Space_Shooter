using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{

    [SerializeField] GameObject[] arrayPowerUpsPrefabs;

    float porcentajeAparicionPowerUp = 30;    //POner que se coja del gestorPlayer, por si es algoque cambiarña en funcin delvl y mejoras

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnearPowerUp(Vector3 posicion) {

        bool seGeneraPowerUp = DecidirSiSpawneaPowerUp();

        if (seGeneraPowerUp) {
            GameObject powerUpSpawnear = SeleccionarPowerUpASpawnear();
            InstanciarPowerUp(powerUpSpawnear, posicion);

        }

    }

    void InstanciarPowerUp(GameObject powerUpSpawnear, Vector3 posicion) {
        Instantiate(powerUpSpawnear, posicion, Quaternion.identity);
    }


    bool DecidirSiSpawneaPowerUp() {

        porcentajeAparicionPowerUp = GestorPlayer.Instance.porcentajeAparicionPowerUp;

        int valor = Random.Range(0, 100);
        if (valor <= porcentajeAparicionPowerUp - 1)
        {
            return true;
        }
        else {
            return false;
        }
    }

    GameObject SeleccionarPowerUpASpawnear() {

        //Se coge el array con % de aparición de power Ups del gestorplayer
        int[] porcentajesPowerUps = GestorPlayer.Instance.porcentajesAparicionPowerUps;

        //Para utilizar el spawn en función de porcentajes, se genera primero un número
        int indiceEscogido = Random.Range(1, 101);
        int porcentajeAcumulado = 0;
        GameObject powerUpAGenerar = null;
        //Se recorren los porcentajes de enemigo a elegir, sumando su porcentaje, en el momento en que el número acumulado sea mayor/igual al número sacado random, ese índice es el que va a generar
        for (int i = 0; i < porcentajesPowerUps.Length; i++)
        {
            porcentajeAcumulado += porcentajesPowerUps[i];

            if (porcentajeAcumulado >= indiceEscogido)
            {
                powerUpAGenerar = arrayPowerUpsPrefabs[i];
                break;
            }
        }

        return powerUpAGenerar;
    }

}
