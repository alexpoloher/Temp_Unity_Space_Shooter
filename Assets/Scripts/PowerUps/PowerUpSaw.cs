using UnityEngine;

public class PowerUpSaw : MonoBehaviour
{
    [SerializeField] float velocidadRotacionTraslacion = 60.0f;
    [SerializeField] float velocidadRotacionSierra = 120.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.back * velocidadRotacionTraslacion * Time.deltaTime, Space.World);
        transform.GetChild(0).transform.Rotate(Vector3.back * velocidadRotacionSierra * Time.deltaTime, Space.World);

    }
}
