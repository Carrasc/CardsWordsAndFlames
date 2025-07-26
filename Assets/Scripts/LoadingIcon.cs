using UnityEngine;

public class LoadingIcon : MonoBehaviour
{
    [SerializeField] private Transform myTransform;

    private void Update()
    {
        myTransform.Rotate(0, 0, -80 * Time.deltaTime);
    }
}
