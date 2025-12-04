using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public enum SpawnMode{ 
        Line,
        Points,
    }

    [SerializeField] SpawnMode spawnMode;

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform spawnLineTop;
    [SerializeField] Transform spawnLineBottom;
    [SerializeField] GameObject[] enemyPrefabs;     //Array en el que guardo todos los enemigos que va a poder spawnear este spawner

    [SerializeField] Transform[] spawnPointsHorizontales;
    [SerializeField] Transform[] spawnPointsVerticales;
    [SerializeField] float delaySpawnMaximo = 3.0f;

    private bool puedeSpawnear = true;
    private bool esEnemigoVertical = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (spawnMode == SpawnMode.Line)
        {
            //Se inicia la corrutina
            StartCoroutine(LineSpawning());

        }
        else if (spawnMode == SpawnMode.Points)
        {

            /*if (puedeSpawnear)
            {
                puedeSpawnear = false;
            }
            int numPoints = spawnPoints.Length;
            int j = Random.Range(0, numPoints); //Con maxExclusive

            Vector3 startPosition = spawnPoints[j].position;   //Si vale 0, develve top, si no 1, y si vale entre medias, devuelve según del que esté mas cerca
            Instantiate(enemyPrefab, startPosition, Quaternion.identity);*/

        }


    }

    //Corrutinas, se ejecutan ellas en el tiempo en lugar de en un fotograma
    //No llamar a corrutinas desde corrutinas, salvo que se esté muy seguro, y aque se generan hilos de ejecución
    IEnumerator LineSpawning() {

        Vector3 lineTop = spawnLineTop.position;
        Vector3 lineBottom = spawnLineBottom.position;

        for (int i = 0; i < 5; i++)
        {
            float t = Random.Range(0f, 1f); //Si este numerpo está mas cerda de 0, que esté mas cerca de top y si mas cerca de 1, mas cerca de bottom
            Vector3 startPosition = Vector3.Lerp(lineTop, lineBottom, t);   //Si vale 0, develve top, si no 1, y si vale entre medias, devuelve según del que esté mas cerca
            Instantiate(enemyPrefab, startPosition, enemyPrefab.transform.rotation);

            yield return new WaitForSeconds(0.5f);  //Esto hace que el código se pare aquí duante esos segundos
        }

    }


    // Update is called once per frame
    void Update()
    {

        //Cada x segundos se Spawnea un enemigo
        if (spawnMode == SpawnMode.Points)
        {
            if (puedeSpawnear)
            {

                GameObject enemigoSpawnear = SeleccionarEnemigoASpawnear();

                if (enemigoSpawnear != null) {

                    if (esEnemigoVertical) {
                        int numPoints = spawnPointsVerticales.Length;
                        int j = Random.Range(0, numPoints); //Con maxExclusive

                        Vector3 startPosition = spawnPointsVerticales[j].position;
                        Instantiate(enemigoSpawnear, startPosition, enemigoSpawnear.transform.rotation);
                    }
                    else {
                        int numPoints = spawnPointsHorizontales.Length;
                        int j = Random.Range(0, numPoints); //Con maxExclusive

                        Vector3 startPosition = spawnPointsHorizontales[j].position;   
                        Instantiate(enemigoSpawnear, startPosition, enemigoSpawnear.transform.rotation);
                    }

                    StartCoroutine(RecargarSpawn());
                }



            }

        }
    }

    GameObject SeleccionarEnemigoASpawnear() {

        int nivelJugador = GestorPlayer.Instance.nivelActual;
        int[] porcentajesEnemigosHprizontales = GestorPlayer.Instance.porcentajesAparicionEnemigos;

        //Para utilizar el spawn en función de porcentajes, se genera primero un número
        int indiceEscogido = Random.Range(1, 101);
        int porcentajeAcumulado = 0;
        GameObject enemigoGenerar = null;
        //Se recorren los porcentajes de enemigo a elegir, sumando su porcentaje, en el momento en que el número acumulado sea mayor/igual al número sacado random, ese índice es el que va a generar
        for (int i = 0; i < porcentajesEnemigosHprizontales.Length; i++) {
            porcentajeAcumulado += porcentajesEnemigosHprizontales[i];

            if (porcentajeAcumulado >= indiceEscogido) {
                if (i == 4 || i == 6 || i == 7) {
                    esEnemigoVertical = true;
                }
                else {
                    esEnemigoVertical = false;
                }
                enemigoGenerar = enemyPrefabs[i];
                break;
            }
        }

        return enemigoGenerar;


    }

    IEnumerator RecargarSpawn()
    {
        puedeSpawnear = false;
        float delaySpawn = Random.Range(delaySpawnMaximo/2, delaySpawnMaximo);
        yield return new WaitForSeconds(delaySpawn);
        puedeSpawnear = true;

    }
}
