using System.Collections;
using UnityEngine;

public class PowerUpHandler : MonoBehaviour
{

    public Camera mainCamera;
    public new GameObject collider;
    public float respawnInterval;

    private Bounds _bounds;
    
    private void Start()
    {
        _bounds = collider.GetComponent<BoxCollider2D>().bounds;
        
        RandomizePosition();

        StartCoroutine(nameof(ChangePosition));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RandomizePosition();
        }
    }

    private IEnumerator ChangePosition()
    {
        for (;;)
        {
            RandomizePosition();
            yield return new WaitForSeconds(respawnInterval);
        }
    }

    private void RandomizePosition()
    {
        var min = _bounds.min;
        var max = _bounds.max;

        var xPosition = Mathf.Round(Random.Range(min.x, max.x));
        var yPosition = Mathf.Round(Random.Range(min.y, max.y));

        transform.position = new Vector3(
            xPosition,
            yPosition,
            mainCamera.farClipPlane - 1);
    }
}
