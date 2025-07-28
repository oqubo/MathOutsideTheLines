using TMPro;
using UnityEngine;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    
    // Variables de estado del juego
    [SerializeField] private int valorObjetivoActual, valorObjetivoMin, valorActual, planetasActual, planetasMin, nivel, puntos;
    [SerializeField] private GameObject planetaPrefab;

    [SerializeField] private TextMeshProUGUI txtLevel, txtTarget, txtValue;
        
//------------------------------
// CREACION
//------------------------------
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Elimina duplicados
            Destroy(gameObject);
        }

        ConfigurarFPS();
    }


    private void ConfigurarFPS()
    {
        // Limitar FPS en el editor
#if UNITY_EDITOR
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        // Sin limite en compilado
#else
        Application.targetFrameRate = -1;
#endif
    }
    
    
//------------------------------
// PARTIDA
//------------------------------
    private void Start()
    {
        nivel = 1;
        puntos = 0;
        valorObjetivoMin = 10;
        planetasMin = 2;
        valorActual = 0;
        valorObjetivoActual = 0;
        planetasActual = 0;
        
        CrearNivel();
    }

    private void CrearNivel()
    {
        // Lógica para crear un nuevo nivel

        valorObjetivoActual = Random.Range(
            valorObjetivoMin, 
            Mathf.Min(valorObjetivoMin + 10*nivel,101)
            ); // Valor objetivo aleatorio
        
        planetasActual = Random.Range(
            Mathf.Min(planetasMin + nivel, 4), 
            Mathf.Min(planetasMin + nivel, 7)
            ); // Número de planetas aleatorio

                        
        
        Debug.Log($"Nuevo nivel creado: Objetivo {valorObjetivoActual}, Planetas {planetasActual}");
        
        Rect area = new Rect(-1.6f, -3.2f, 6.3f, 6.8f);
        Vector3 centro = new Vector3(1.55f, 0.2f, 0f);
        float radio = 3.5f;
        float distanciaMinima = 1f;

        List<Vector3> posicionesGeneradas = new List<Vector3>();

        for (int i = 0; i < planetasActual; i++)
        {
            Vector3 posicion;
            bool posicionValida = false;

            while (!posicionValida)
            {
                float angulo = Random.Range(0f, Mathf.PI * 2f);
                float distancia = Mathf.Sqrt(Random.Range(0f, 1f)) * radio;
                posicion = centro + new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0) * distancia;

                // Comprobar que esté lejos de otros planetas
                posicionValida = true;
                foreach (Vector3 posExistente in posicionesGeneradas)
                {
                    if (Vector3.Distance(posicion, posExistente) < distanciaMinima)
                    {
                        posicionValida = false;
                        break;
                    }
                }

                if (posicionValida)
                {
                    posicionesGeneradas.Add(posicion);
                    Instantiate(planetaPrefab, posicion, Quaternion.identity);
                }
            }
        }
        
        ActualizarUI();
    }
    
    private void LimpiarPlanetas()
    {
        PlanetController[] planets = FindObjectsOfType<PlanetController>();
        foreach (var planet in planets)
        {
            Destroy(planet.gameObject);
        }
    }
    
    private void LimpiarEstelas()
    {
        GameObject[] simbols = GameObject.FindGameObjectsWithTag("Symbol");
        foreach (var simbol in simbols)
        {
            Destroy(simbol);
        }
    }

    public void Win()
    {
        nivel++;
        LimpiarPlanetas();
        LimpiarEstelas();
        RecolocarNave();
        CrearNivel();
    }

    public void Lose()
    {
    }
    
    public void RecolocarNave()
    {
        NaveController ship = FindObjectOfType<NaveController>();
        if (ship != null)
        {
            ship.RecolocarNave();
        }
    }
    
    public void ReiniciarNivel()
    {
        RecolocarNave();
        valorActual = 1;
        LimpiarEstelas();
        ActualizarUI();
    }
    
    public void ModificarValorActual(int valor)
    {
        NaveController ship = FindObjectOfType<NaveController>();
        if (ship != null)
        {
            switch (ship.currentOperation)
            {
                case NaveController.Operation.Add:
                    valorActual += valor;
                    break;
                case NaveController.Operation.Subtract:
                    valorActual -= valor;
                    break;
                case NaveController.Operation.Multiply:
                    valorActual *= valor;
                    break;
                case NaveController.Operation.Divide:
                    if (valor != 0) // Evitar división por cero
                        valorActual /= valor;
                    break;
            }
        }
        
        ActualizarUI();
        
        if (valorActual == valorObjetivoActual)
        {
            Win();
        }
    }
    
    public void ActualizarUI()
    {
        txtLevel.text = nivel.ToString();
        txtTarget.text = valorObjetivoActual.ToString();
        txtValue.text = valorActual.ToString();
    }
    

//------------------------------
// ESCENAS
//------------------------------

    public void CargarEscena(int sceneBuildIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneBuildIndex);
    }

    public void CargarEscena(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    public void ReiniciarEscena()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        CargarEscena(currentSceneIndex);
        PausarJuegoOff();
    }


    public void PausarJuegoOn()
    {
        Time.timeScale = 0;
    }

    public void PausarJuegoOff()
    {
        Time.timeScale = 1;
    }

    public void SalirDeJuego()
    {
        // Detiene el juego en el editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        // Cierra la aplicacion en compilado
#else
        Application.Quit(); 
#endif

        // guardar partida
        GuardarPartida();
    }


    public void FinalizarPartida()
    {
        Invoke("ReiniciarEscena", 5f);
    }


//------------------------------
// GUARDADO
//------------------------------

    public void GuardarPartida()
    {
        /*
        if (PlayerPrefs.GetInt("nivelDesbloqueado") < nivel)
            PlayerPrefs.SetInt("nivelDesbloqueado", nivel);
        if (PlayerPrefs.GetInt("puntosMax") < puntos)
            PlayerPrefs.SetInt("puntosMax", puntos);
        */
    }

    public void CargarPartida()
    {
        /*
        nivel = PlayerPrefs.GetInt("nivelDesbloqueado");
        puntos = PlayerPrefs.GetInt("puntosMax");
        */
    }

}
