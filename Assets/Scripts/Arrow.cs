using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform target; // объект за которым надо следить
    public Transform arrow; // стрелка
    public Camera mainCamera;

    Vector3 newPos;
    void FixedUpdate()
    {
        Vector3 targetOnScreen = mainCamera.WorldToViewportPoint(target.position);
        targetOnScreen.x = Mathf.Clamp01(targetOnScreen.x);
        targetOnScreen.y = Mathf.Clamp01(targetOnScreen.y);
        
        newPos = new Vector3(mainCamera.ViewportToWorldPoint(targetOnScreen).x, mainCamera.ViewportToWorldPoint(targetOnScreen).y, 0);

        arrow.position = newPos;
    }
}