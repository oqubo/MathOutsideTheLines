using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlanetController : MonoBehaviour
{
    [SerializeField] private int valor;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClipPlaneta;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
    }
    
    public void SetValor(int nuevoValor)
    {
        valor = nuevoValor;
        if (sprites != null && sprites.Length > valor)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[valor];
        }
    }

    private void OnMouseDown()
    {
        audioSource.PlayOneShot(audioClipPlaneta);
        NaveController ship = FindObjectOfType<NaveController>();
        if (ship != null)
        {
            ship.MoveTo(transform.position, valor);
            Debug.Log("Nave va hacia: " + gameObject.name + " usando " + ship.GetOperationSymbol());
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        style.fontSize = 16;
        style.alignment = TextAnchor.MiddleCenter;
        Handles.Label(transform.position, valor.ToString(), style);
    }
#endif
}
