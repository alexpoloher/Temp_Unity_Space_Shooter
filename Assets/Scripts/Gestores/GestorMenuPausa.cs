using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings.SplashScreen;

public class GestorMenuPausa : MonoBehaviour
{
    public static GestorMenuPausa Instance;
    [SerializeField] GameObject pantallaPausa;

    [Header("Controls")]
    [SerializeField] InputActionReference pause;



    //Este es el que se encarga de detectar cuando pulsas P y muestra la pantalla de Pausa



    private void OnEnable()
    {
        //Se habilita en este momento la lectura de estos inputs
        pause.action.Enable();

        pause.action.started += OnPause;
        //pause.action.performed += OnPause;

    }

    private void OnPause(InputAction.CallbackContext obj)
    {
        if (GestorEstadoPausa.Instance.estaPausado)
        {
            //Solo puede quitar la pausa pulsando p si es pausa tipo Pausa
            if (GestorEstadoPausa.Instance.tiposPausa == GestorEstadoPausa.tiposDePausaEnum.Pausa) {
                if (pantallaPausa != null)
                {
                    pantallaPausa.SetActive(false);
                    DesPausar();
                }
            }
        }
        else {
            if (pantallaPausa != null)
            {
                pantallaPausa.SetActive(true);
                Pausar();
            }

        }
    }


    public void DesPausar() {
        print("despausa");
        GestorEstadoPausa.Instance.estaPausado = false;
        GestorEstadoPausa.Instance.tiposPausa = GestorEstadoPausa.tiposDePausaEnum.Libre;
        Time.timeScale = 1f;    //Despausa el juego
        
    }

    public void Pausar()
    {
        print("pausa");
        GestorEstadoPausa.Instance.estaPausado = true;
        GestorEstadoPausa.Instance.tiposPausa = GestorEstadoPausa.tiposDePausaEnum.Pausa;
        Time.timeScale = 0f;    //Pausa el juego
    }

    

    public void onClickResume() {
        if (pantallaPausa != null)
        {
            pantallaPausa.SetActive(false);
            DesPausar();
        }

    }

    public void onClickSalir()
    {
        //Aquí poner cuando tenga hecho lo de slair al menú
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
