using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorEstadoPausa : MonoBehaviour
{

    //Este tiene una variable global que todos verán para gestionar con ella las pausas

    public static GestorEstadoPausa Instance;
    public bool estaPausado = false;
    public bool tiendaAbierta = false;
    public enum tiposDePausaEnum { 
        Libre,
        Pausa,
        Tienda
    }

    //En esta variable se indica el tipo de pausa para que no se puedan varias a la vez
    public tiposDePausaEnum tiposPausa = tiposDePausaEnum.Libre;

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
        estaPausado = false;

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
