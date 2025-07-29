using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NaveController : MonoBehaviour
{
    public enum Operation { Add, Subtract, Multiply, Divide }
    public Operation currentOperation = Operation.Add;
    
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 360f;
    
    [SerializeField] private float symbolSpacing = 0.5f;
    [SerializeField] private GameObject symbolPrefab;

    [SerializeField] private Vector3 posicionInicial = new Vector3(1.6f, -4.5f, 0); 
    
    [SerializeField] private Sprite imgSuma, imgResta, imgMultiplicacion, imgDivision;
    [SerializeField] private Sprite imgEstelaSuma, imgEstelaResta, imgEstelaMultiplicacion, imgEstelaDivision, imgEstelaActual;
    [SerializeField] private Image decoracionImage; 
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClipBoton, audioClipLinea;

    
    private bool isBusy = false;

    private Vector3 moveDirection;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        RecolocarNave();
    }

    void Update()
    {

    }


    public void SetSuma()
    {
        audioSource.PlayOneShot(audioClipBoton);
        currentOperation = Operation.Add;
        if (decoracionImage != null) decoracionImage.sprite = imgSuma;
        imgEstelaActual = imgEstelaSuma;
        GetComponent<SpriteRenderer>().color = Color.red;

    }
    
    public void SetResta()
    {
        audioSource.PlayOneShot(audioClipBoton);
        currentOperation = Operation.Subtract;
        if (decoracionImage != null) decoracionImage.sprite = imgResta;
        imgEstelaActual = imgEstelaResta;
        GetComponent<SpriteRenderer>().color = Color.yellow;
    }
    
    public void SetMultiplicacion()
    {
        audioSource.PlayOneShot(audioClipBoton);
        currentOperation = Operation.Multiply;
        if (decoracionImage != null) decoracionImage.sprite = imgMultiplicacion;
        imgEstelaActual = imgEstelaMultiplicacion;
        GetComponent<SpriteRenderer>().color = Color.blue;
    }
    
    public void SetDivision()
    {
        audioSource.PlayOneShot(audioClipBoton);
        currentOperation = Operation.Divide;
        if (decoracionImage != null) decoracionImage.sprite = imgDivision;
        imgEstelaActual = imgEstelaDivision;
        GetComponent<SpriteRenderer>().color = Color.green;
    }
    
    
    public void RecolocarNave()
    {
        transform.position = posicionInicial;
        SetSuma();
    }
    

    public void MoveTo(Vector3 destination, int valor)
    {

        if (!isBusy)
        {
            StartCoroutine(RotateAndMove(destination, valor));
        }
        
        
    }

     private IEnumerator RotateAndMove(Vector3 destination, int valor)
    {
        isBusy = true;
        Vector3 direction = (destination - transform.position).normalized;

        // ROTACIÓN
        /*
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        while (true)
        {
            float currentAngle = transform.eulerAngles.z;
            float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) < 1f)
                break;

            yield return null;
        }
        */

        // MOVIMIENTO
        Vector3 lastSymbolPosition = transform.position;
        moveDirection = direction;
        PlaceSymbol(transform.position); // símbolo inicial

        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, lastSymbolPosition) >= symbolSpacing)
            {
                PlaceSymbol(transform.position);
                lastSymbolPosition = transform.position;
            }

            yield return null;
        }

        isBusy = false;
        GameManager.Instance.ModificarValorActual(valor);
    }

    void PlaceSymbol(Vector3 position)
    {
        GameObject symbol = Instantiate(symbolPrefab, position, Quaternion.identity);
        symbol.GetComponent<SpriteRenderer>().sprite = imgEstelaActual;
        symbol.transform.up = moveDirection;
    }

    public string GetOperationSymbol()
    {
        return currentOperation switch
        {
            Operation.Add => "+",
            Operation.Subtract => "-",
            Operation.Multiply => "*",
            Operation.Divide => "÷",
            _ => "?",
        };
    }
    
    Color GetOperationColor()
    {
        return currentOperation switch
        {
            Operation.Add => Color.blue,
            Operation.Subtract => Color.red,
            Operation.Multiply => Color.yellow,
            Operation.Divide => new Color(1f, 0.5f, 0f),
            _ => Color.white
        };
    }
    
}