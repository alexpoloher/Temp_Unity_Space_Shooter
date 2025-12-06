using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuPrincipalCanvasManager : MonoBehaviour
{

    [Header("ElementosDelCanvas")]
    [SerializeField] GameObject fondoMenuInicial;
    [SerializeField] GameObject fondoAyuda;

    [Header("Controls")]
    [SerializeField] InputActionReference salir;

    private bool ayudaAbierta = false;

    private void OnEnable()
    {
        //Se habilita en este momento la lectura de estos inputs
        salir.action.Enable();
        salir.action.started += OnExit;
    }

    private void OnDisable()
    {
        //Se deshabilita en este momento la lectura de estos inputs
        salir.action.started -= OnExit;
        salir.action.Disable();

    }

    //Al pulsar escape se cierra el juego
    private void OnExit(InputAction.CallbackContext obj)
    {
        Application.Quit();
    }


    public void OnClickPlay() {
        Time.timeScale = 1f;
        GestorEstadoPausa.Instance.estaPausado = false;
        GestorEstadoPausa.Instance.tiposPausa = GestorEstadoPausa.tiposDePausaEnum.Libre;
        SceneManager.LoadScene("SampleScene");
    }

    public void OnClickAyuda() {
        if (!ayudaAbierta) {
            ayudaAbierta = true;
            fondoMenuInicial.SetActive(false);
            fondoAyuda.SetActive(true);
        }
    }

    public void OnclickVolverDeAyuda() {
        if (ayudaAbierta)
        {
            ayudaAbierta = false;
            fondoMenuInicial.SetActive(true);
            fondoAyuda.SetActive(false);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        GestorEstadoPausa.Instance.estaPausado = false;
        GestorSonido.Instance.DetenerSonidos();
        GestorSonido.Instance.IniciarMusicaMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
