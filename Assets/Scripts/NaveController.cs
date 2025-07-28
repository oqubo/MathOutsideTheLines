using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class NaveController : MonoBehaviour
{
    public enum Operation { Add, Subtract, Multiply, Divide }
    public Operation currentOperation = Operation.Add;
    
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 360f; // grados por segundo
    [SerializeField] private float symbolSpacing = 0.5f;

    [SerializeField] private TextMeshPro operationLabel; // Asignar en Inspector
    [SerializeField] private GameObject symbolPrefab;

    [SerializeField] private Vector3 posicionInicial = new Vector3(1.6f, -4.5f, 0); 
    
    [SerializeField] private Sprite imgSuma, imgResta, imgMultiplicacion, imgDivision;
    [SerializeField] private Image decoracionImage; 
    
    private bool isBusy = false;

    private Vector3 moveDirection;
    
    void Start()
    {
        RecolocarNave();
        PaintOperationLabel();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit != null && hit.gameObject == this.gameObject)
            {
                CycleOperation();
            }
        }
    }

    void CycleOperation()
    {
        currentOperation = (Operation)(((int)currentOperation + 1) % 4);
        Debug.Log("Operation: " + currentOperation);
        
        PaintOperationLabel();
    }

    public void SetSuma()
    {
        currentOperation = Operation.Add;
        if (decoracionImage != null) decoracionImage.sprite = imgSuma;
        PaintOperationLabel();
    }
    
    public void SetResta()
    {
        currentOperation = Operation.Subtract;
        if (decoracionImage != null) decoracionImage.sprite = imgResta;
        PaintOperationLabel();
    }
    
    public void SetMultiplicacion()
    {
        currentOperation = Operation.Multiply;
        if (decoracionImage != null) decoracionImage.sprite = imgMultiplicacion;
        PaintOperationLabel();
    }
    
    public void SetDivision()
    {
        currentOperation = Operation.Divide;
        if (decoracionImage != null) decoracionImage.sprite = imgDivision;
        PaintOperationLabel();
    }
    
    
    public void RecolocarNave()
    {
        transform.position = posicionInicial;
        PaintOperationLabel();
    }

    public void PaintOperationLabel()
    {
        if (operationLabel != null)
        {
            operationLabel.text = GetOperationSymbol();
            operationLabel.color = GetOperationColor();
        }
    }

    public void MoveTo(Vector3 destination, int valor)
    {
        if (!isBusy)
            StartCoroutine(RotateAndMove(destination, valor));
        
        
    }

     private IEnumerator RotateAndMove(Vector3 destination, int valor)
    {
        isBusy = true;

        // ROTACIÓN
        Vector3 direction = (destination - transform.position).normalized;
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
        TextMeshPro tmp = symbol.GetComponent<TextMeshPro>();
        tmp.text = GetOperationSymbol();
        tmp.fontSize = 3;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = GetOperationColor();
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