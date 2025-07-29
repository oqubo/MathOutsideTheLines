using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    
    // Variables de estado del juego
    [SerializeField] private int valorObjetivoActual, valorObjetivoMin, valorActual, planetasActual, planetasMin, nivel, puntos;
    [SerializeField] private GameObject planetaPrefab;

    [SerializeField] private TextMeshProUGUI txtLevel, txtTarget, txtValue;
        
    [SerializeField] private GameObject despegueImage;
    [SerializeField] private Image imgProgreso;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClipBoton, audioClipWin;
    
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

        //ConfigurarFPS();
        
        despegueImage.SetActive(true);
        Invoke("OcultarImagen",3f);
        
    }

    private void OcultarImagen()
    {
        despegueImage.SetActive(false);
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
        audioSource = GetComponent<AudioSource>();
        
        nivel = 1;
        puntos = 0;
        valorObjetivoMin = 10;
        planetasMin = 1;
        valorActual = 0;
        valorObjetivoActual = 0;
        planetasActual = 0;
        
        CrearNivel();
    }

    private void CrearNivel()
    {
        
        // niveles de 2, 3, 4, 5 planetas 
        planetasActual = planetasMin + nivel;
        if (planetasActual > 5) planetasActual = 5;
        
        
        Rect area = new Rect(-1.4f, -2.8f, 4.5f, 3.4f);
        Vector3 centro = new Vector3(1.55f, 0.2f, 0f);
        float radio = 3f;
        float distanciaMinima = 1f;

        List<Vector3> posicionesGeneradas = new List<Vector3>();
        List<int> listaValores = new List<int>();
        GameObject planetaGenerado = null;
        int valorPlaneta = 1;

        // Generar una posición aleatoria dentro del área circular
        // Instanciar el planeta en esa posición
        // asignarle un valor entre 1 y 10
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
                    planetaGenerado = Instantiate(planetaPrefab, posicion, Quaternion.identity);
                    valorPlaneta = Random.Range(1, 11);
                    planetaGenerado.GetComponent<PlanetController>().SetValor(valorPlaneta);
                    listaValores.Add(valorPlaneta);
                    
                }
            }
        }

        // Calcular el valor objetivo en base a los valores de los planetas
        // para que se pueda alcanzar con las operaciones
        valorObjetivoActual = listaValores[0];
        int valorTemporal = valorObjetivoActual;
        string pista = "";
        string pistaTemporal = "";
        pista += valorTemporal.ToString();
        
        int operacion = 1;
        bool valido = false;
        for (int i = 1; i < listaValores.Count; i++)
        {
            operacion = Random.Range(1, 5); // 1: suma, 2: resta, 3: multiplicación, 4: división
            valido = false;
            switch (operacion)
            {
                case 1: // Suma
                    valorTemporal += listaValores[i];
                    pistaTemporal = " + " + listaValores[i].ToString();
                    break;
                case 2: // Resta
                    valorTemporal -= listaValores[i];
                    pistaTemporal = " - " + listaValores[i].ToString();
                    break;
                case 3: // Multiplicación
                    valorTemporal *= listaValores[i];
                    pistaTemporal = " * " + listaValores[i].ToString();
                    break;
                case 4: // División
                    if (listaValores[i] != 0)
                    {
                        valorTemporal /= listaValores[i];
                        pistaTemporal = " / " + listaValores[i].ToString();
                    }
                    break;
            }

            if (valorTemporal <= 0)
            {
                valorTemporal = valorObjetivoActual;
                i--; // Repetir la iteración actual
                operacion = Random.Range(1, 5);
                pistaTemporal = " ";
            }
            else
            {
                valorObjetivoActual = valorTemporal;
                valido = true;
                pista += pistaTemporal;
                pistaTemporal = " ";
            }
        }
        
        Debug.Log($"Nuevo nivel creado: Objetivo {valorObjetivoActual}, Planetas {planetasActual}");
        Debug.Log(pista);
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
        audioSource.PlayOneShot(audioClipWin);
        
        nivel++;
        LimpiarPlanetas();
        LimpiarEstelas();

        if (nivel >= 9)
        {
            ActualizarUI();
            CargarEscena("Fin");
        }
        
        RecolocarNave();
        CrearNivel();
        valorActual = 0;
        ActualizarUI();
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
        audioSource.PlayOneShot(audioClipBoton);
        RecolocarNave();
        valorActual = 0;
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
        imgProgreso.fillAmount = ((nivel-1) / 8f);
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
