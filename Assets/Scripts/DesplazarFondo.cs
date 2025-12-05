using UnityEngine;

public class DesplazarFondo : MonoBehaviour
{

    [SerializeField] float parallaxMultiplier = 0.1f;

    private Material materialParallax;
    private Transform player;
    private float lastPlayerX;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        materialParallax = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        materialParallax.mainTextureOffset += new Vector2(parallaxMultiplier * Time.deltaTime, 0);
    }
}
