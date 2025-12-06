using UnityEngine;

public class GestorSonido : MonoBehaviour
{

    public static GestorSonido Instance;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSourceMusicaFondo;
    [SerializeField] AudioClip musicaFondoInicio;
    [SerializeField] AudioClip musicaFondoNivel1;
    [SerializeField] AudioClip musicaFondoNivel2;
    [SerializeField] AudioClip musicaFondoNivel3;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EjecutarSonido(AudioClip audio)
    {
        audioSource.volume = 1f;
        audioSource.PlayOneShot(audio);
    }

    public void DetenerSonidos()
    {
        audioSource.Stop();
        audioSourceMusicaFondo.Stop();
    }

    public void IniciarMusicaMenu() {
        audioSourceMusicaFondo.clip = musicaFondoInicio;
        audioSourceMusicaFondo.Play();
    }

    public void IniciarMusicaNivel(int fase) {
        switch (fase) { 
        
            case 1:
                audioSourceMusicaFondo.clip = musicaFondoNivel1;
                break;
            case 2:
                audioSourceMusicaFondo.clip = musicaFondoNivel2;
                break;
            case 3:
                audioSourceMusicaFondo.clip = musicaFondoNivel3;
                break;
            default:
                audioSourceMusicaFondo.clip = musicaFondoNivel1;
                break;

        }

        audioSourceMusicaFondo.Play();
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
