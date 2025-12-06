using UnityEngine;

public class BossSpawner : MonoBehaviour
{

    [SerializeField] GameObject bossPrefab;
    [SerializeField] GameObject puntoSpawn;
    [SerializeField] public GameObject[] puntosAparicion;


    public void SpawnearBoss() { 
    
        Instantiate(bossPrefab, puntoSpawn.transform.position, bossPrefab.transform.rotation);
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
