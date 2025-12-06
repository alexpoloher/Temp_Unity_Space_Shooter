using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GestorVictoria : MonoBehaviour
{

    [Header("ElementosDelCanvasVictoria")]
    [SerializeField] GameObject fondoVictoria;
    [SerializeField] Image botonSalir;
    [SerializeField] TextMeshProUGUI textoVictoria;
    [SerializeField] TextMeshProUGUI textoVictoria2;


    public void MostrarPantallaVictoria()
    {

        Color opacidad = textoVictoria.color;
        opacidad.a = 0;
        textoVictoria.color = opacidad;       //Se pone a 0 la opacidad
        textoVictoria2.color = opacidad;       //Se pone a 0 la opacidad
        fondoVictoria.gameObject.SetActive(true);
        StartCoroutine(MostrarMensajeVictoria());


    }

    IEnumerator MostrarMensajeVictoria()
    {

        while (textoVictoria.color.a < 1f)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            //Se va aumentando gradualmente la opacidad, para que aparezca el texto poco a poco
            Color opacidad = textoVictoria.color;
            opacidad.a = opacidad.a + 0.05f;
            textoVictoria.color = opacidad;
            textoVictoria2.color = opacidad;      
        }
        botonSalir.gameObject.SetActive(true);

    }

    public void OnClickSalir() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    
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
