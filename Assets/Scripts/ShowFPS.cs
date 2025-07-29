using UnityEngine;
using TMPro;

public class ShowFPS : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI fpsText;
    private float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fpsText.text = "FPS: " + (1.0f / deltaTime).ToString();
        
    }
}
