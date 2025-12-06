using UnityEngine;
using UnityEngine.UI;

public class GestorCanvasVidaBoss : MonoBehaviour
{

    [Header("ElementosDelCanvas")]
    [SerializeField] Image iconoVidaBoss;
    [SerializeField] Image barraVidaBoss;


    public void ActualizarVida(float vidaActual, float vidaMaxima) {
        barraVidaBoss.fillAmount = vidaActual / vidaMaxima;
    }

    public void mostrarBarraVidaBoss() { 
        iconoVidaBoss.gameObject.SetActive(true);
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
