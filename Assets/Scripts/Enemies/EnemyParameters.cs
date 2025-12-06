using UnityEngine;

public class EnemyParameters : MonoBehaviour
{


    [SerializeField] public float damageToDeal = 25;
    [SerializeField] public float puntosDeExperiencia = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D elOtro)
    {

    }
}
