using UnityEngine;

public class SpriteSlider : MonoBehaviour
{
    [SerializeField] private Transform leftEdge, rightEdge, marker, boundsLeft, boundsRight;

    public float GetX(float v) => Mathf.Lerp(leftEdge.position.x, rightEdge.position.x, v);

    public void SetBoundsVisible(bool value)
    {
        boundsLeft.gameObject.SetActive(value);
        boundsRight.gameObject.SetActive(value);
    }

    public void SetBoundPosition(float middle, float range)
    {
        boundsLeft.position = new Vector3(GetX(middle - range), boundsLeft.position.y, boundsLeft.position.z);
        boundsRight.position = new Vector3(GetX(middle + range), boundsRight.position.y, boundsRight.position.z);
    }

    public void SetMarkerPosition(float v)
    {
        marker.position = new Vector3(GetX(v), marker.position.y, marker.position.y);
    }
}
