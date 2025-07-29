using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public string sceneName = "MainGame";
    public GameObject[] imagenesTutorial;

    private int indiceActual = 0;

    void Start()
    {
        MostrarImagen(indiceActual);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SiguienteImagen();
        }
    }

    void MostrarImagen(int indice)
    {
        for (int i = 0; i < imagenesTutorial.Length; i++)
        {
            imagenesTutorial[i].SetActive(i == indice);
        }
    }

    void SiguienteImagen()
    {
        indiceActual++;
        if (indiceActual < imagenesTutorial.Length)
        {
            MostrarImagen(indiceActual);
        }
        else
        {
            Play();
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(sceneName);
    }
}