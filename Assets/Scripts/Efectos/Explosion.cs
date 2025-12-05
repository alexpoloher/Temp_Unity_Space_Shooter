using UnityEngine;

public class Explosion : MonoBehaviour
{

    [Header("Sonidos")]
    [SerializeField] AudioClip sonidoExplosion;

    private void Start()
    {
        if (sonidoExplosion != null) {
            GestorSonido.Instance.EjecutarSonido(sonidoExplosion);
        }

    }

    private void DestruirExplosion()
    {
        Destroy(gameObject);    
    }
}
