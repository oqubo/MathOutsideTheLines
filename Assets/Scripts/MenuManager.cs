using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public string sceneName = "Tutorial";

    public TextMeshProUGUI textoUI;
    public string[] frases = new string[] { };

    private int indiceFrase = 0;

    void Start()
    {
        MostrarFraseActual();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            indiceFrase++;
            if (indiceFrase < frases.Length)
            {
                MostrarFraseActual();
            }
            else
            {
                Play();
            }
        }
    }

    void MostrarFraseActual()
    {
        textoUI.text = frases[indiceFrase];
    }

    public void Play()
    {
        SceneManager.LoadScene(sceneName);
    }
}