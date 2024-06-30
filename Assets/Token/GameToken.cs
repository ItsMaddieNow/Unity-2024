using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameToken : MonoBehaviour
{
    public GamePlayer colour;
    public Renderer renderer;
    [SerializeField]
    private Material baseMaterial;
    [SerializeField]
    private Material droppingMaterial;
    [SerializeField]
    private Material connectedMaterial;

    public void Drop(float targetHeight)
    {
        StartCoroutine(DropCoroutine(targetHeight));
    }
    IEnumerator DropCoroutine(float targetHeight)
    {
        float velocity = 0;
        while (true)
        {
            velocity += Time.deltaTime * -9.81f;
            float nextPos = Time.deltaTime * velocity + transform.localPosition.y;
            if (nextPos <= targetHeight) break;
            transform.localPosition = new Vector3(transform.localPosition.x, nextPos, transform.localPosition.z);
            yield return null;
        }
        transform.localPosition = new Vector3(transform.localPosition.x, targetHeight, transform.localPosition.z);
        renderer.material = baseMaterial;
    }
    public void HitBottom()
    {
        renderer.material = baseMaterial;
    }

    public GamePlayer GetColour()
    {
        return colour;
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
