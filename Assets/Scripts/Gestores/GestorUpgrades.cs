using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UpgradeObject;

public class GestorUpgrades : MonoBehaviour
{

    public static GestorUpgrades Instance;
    private CanvasManager canvasManager;

    [SerializeField] List<UpgradeObject> listaUpgradeObjectsBase;    //Lista de las mejoras tal cual, sin info de cuál se tiene

    private List<UpgradeState> listaUpgradesState;   //Ene sta lista es donde se indica la info de cada mejora en esta partida, como por ejemplo si ya la he cogido

    Dictionary<enumRarezas, Color> coloresPorRareza = new Dictionary<enumRarezas, Color>();

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

    void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Se obtiene la referencia al canvas manager de cada escena
        canvasManager = FindAnyObjectByType<CanvasManager>();
        if (scene.name.Equals("SampleScene"))
        {
            ReiniciarParametros();
        }

    }


    public List<UpgradeState> ObtenerListaTodasUpgrades() {
        return listaUpgradesState;
    }

    public Dictionary<enumRarezas, Color> ObtenerDiccionarioColoresRarezas()
    {
        return coloresPorRareza;
    }


    void ReiniciarParametros() {

        //Solo se rellena una vez, no se pueden duplicar keys
        if (coloresPorRareza.Count == 0) {
            coloresPorRareza.Add(enumRarezas.Comun, new Color(0f, 0.34f, 1f, 0.8f));
            coloresPorRareza.Add(enumRarezas.Rara, new Color(0.725f, 0f, 0.925f, 0.8f));
            coloresPorRareza.Add(enumRarezas.Legendaria, new Color(1f, 0f, 0f, 0.8f));
        }

        //Se reinician los valores de cada mejora (los que indican por ejemplo si he cogido esa mejora)
        listaUpgradesState = new List<UpgradeState>();

        foreach (UpgradeObject upgradeObject in listaUpgradeObjectsBase) {
            UpgradeState upgradeStateAniadir = new UpgradeState(upgradeObject); //En este constructor es donde se indica el reinicio de cada upgrade
            listaUpgradesState.Add(upgradeStateAniadir);
        }
    
    }


    public void SeleccionarMejorasAleatorias() {

        List<int> listaIndicesEscogidos = new List<int>();
        List<UpgradeState> listaUpgradesEscogidos = new List<UpgradeState>();
        List<UpgradeState> listaUpgradesPosibles = new List<UpgradeState>(); ; //Aquí se almacenan solo los posibles, para que no se repitan

        //Solo se cogen los no obtenidos ya
        foreach (UpgradeState upgSt in listaUpgradesState) {
            if (upgSt.obtenida == false) {
                listaUpgradesPosibles.Add(upgSt);
            }
        }


        //Cada vez que se coge uno, se elimina de la lista de posibilidades
        /*for (int i = 0; i < 3; i++) {

            if (listaUpgradesPosibles.Count > 0) {
                int indice = Random.Range(0, listaUpgradesPosibles.Count);
                listaUpgradesEscogidos.Add(listaUpgradesPosibles[indice]);
                listaUpgradesPosibles.RemoveAt(indice);
            }


        }*/

        //Cada vez que se coge uno, se elimina de la lista de posibilidades
        for (int i = 0; i < 3; i++){
            if (listaUpgradesPosibles.Count > 0) {

                //Primero se coge la rareza que se va a buscar, basándose en lso porcentajes de las rarezas
                //bool existeDeEsaRareza = false;
                //while (existeDeEsaRareza == false){
                    


                int sumaTodosLosPorcentajes = 0;
                enumRarezas rarezaEscogida = enumRarezas.Comun; //Se inicializa a común, pero esto cambia durante el siguiente for, es por iniciarla

                foreach(UpgradeState upgradePosible in listaUpgradesPosibles)
                {
                    sumaTodosLosPorcentajes += upgradePosible.upgrade.ObtenerPorcentajesRarezas(upgradePosible.upgrade.Rareza);
                }


                int indiceUpgradeEscogido = Random.Range(0, sumaTodosLosPorcentajes + 1);    //Inidca el % que se coge
                int porcentajeAcumulado = 0;    //Cuando se supere, será que esa es la rareza escogida
                //Se acumula ahora la suma de porcentajes. Así cada uno tendrá opciones de salir en función de su porcentaje de aparición
                foreach (UpgradeState upgradePosible in listaUpgradesPosibles)
                {
                    porcentajeAcumulado += upgradePosible.upgrade.ObtenerPorcentajesRarezas(upgradePosible.upgrade.Rareza);
                    if (porcentajeAcumulado >= indiceUpgradeEscogido)
                    {
                        rarezaEscogida = upgradePosible.upgrade.Rareza;
                        listaUpgradesEscogidos.Add(upgradePosible);
                        //Se elimina esta upgrade de la lista de TODAS las posibles mejoras, para que no se vuelva a coger
                        listaUpgradesPosibles.Remove(upgradePosible);
                        break;
                    }
                }



           
                //Una vez se tiene la rareza a coger, se mira si existe alguna upgrade de esa rareza aún disponible
               /*while (existeDeEsaRareza == false) {
                    List<UpgradeState> listaPosiblesSeleccionesDeEstaRareza = listaUpgradesPosibles.FindAll(x => x.upgrade.Rareza == rarezaEscogida);
                    if (listaPosiblesSeleccionesDeEstaRareza.Count != 0)
                    {
                        existeDeEsaRareza = true;
                        int indiceSeleccion = Random.Range(0, listaPosiblesSeleccionesDeEstaRareza.Count);
                        listaUpgradesEscogidos.Add(listaPosiblesSeleccionesDeEstaRareza[indiceSeleccion]);
                        
                        //Se elimina esta upgrade de la lista de TODAS las posibles mejoras, para que no se vuelva a coger
                        listaUpgradesPosibles.Remove(listaPosiblesSeleccionesDeEstaRareza[indiceSeleccion]);
                    }
                    else
                    {
                        //Si no hay de esta rareza, se repite el proceso
                        //Si no hay de la rareza escogida, se va bajando de rareza hasta que haya
                        if (rarezaEscogida == enumRarezas.Legendaria) {
                            rarezaEscogida = enumRarezas.Rara;
                        } else if (rarezaEscogida == enumRarezas.Rara) {
                            rarezaEscogida = enumRarezas.Comun;
                        }
                        existeDeEsaRareza = false;
                    }
                }*/

                //}
            }


        }
        
        canvasManager.MostrarFaseSuperada(listaUpgradesEscogidos, coloresPorRareza);

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
