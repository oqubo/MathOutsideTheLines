using UnityEngine;
using UnityEngine.SceneManagement;

public class InicioController : MonoBehaviour
{
    public string sceneName = "Menu";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("CargarEscena", 2f); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void CargarEscena()
    {
        SceneManager.LoadScene(sceneName);
    }
}
