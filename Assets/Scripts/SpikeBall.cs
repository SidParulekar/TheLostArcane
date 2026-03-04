using UnityEngine;

public class SpikeBall : PatrollingEntity
{
    [SerializeField] private float rotationAngle = 90f;

    private void Update()
    {
        RotateBall();
        MoveHorizontal();
    }

    private void RotateBall() //Rotate ball along z axis
    {
        if (_direction == Vector2.left)
        {
            transform.Rotate(Vector3.forward, rotationAngle * Time.deltaTime);
        }

        else
        {
            transform.Rotate(Vector3.back, rotationAngle * Time.deltaTime);
        }
        
    }
}
