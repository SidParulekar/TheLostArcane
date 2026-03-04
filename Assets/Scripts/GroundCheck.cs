using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundCheck : MonoBehaviour
{    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<TilemapCollider2D>()) // When player comes in contact with the ground
        {
            PlayerController.Instance.SetOnGround(true);
            PlayerController.Instance.SetOnAttackableGround(true);
            //Debug.Log ("Grounded");
        }

        if (collision.gameObject.GetComponent<MovingPlatform>())
        {
            PlayerController.Instance.SetOnGround(true);
            PlayerController.Instance.SetOnAttackableGround(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) // When player leaves the gorund
    {
        if (collision.gameObject.GetComponent<TilemapCollider2D>())
        {
            PlayerController.Instance.SetOnGround(false);            
            PlayerController.Instance.AirBorne();
            //Debug.Log("Not Grounded");

        }

        if (collision.gameObject.GetComponent<MovingPlatform>())
        {
            PlayerController.Instance.SetOnGround(false);
            PlayerController.Instance.AirBorne();
        }
    }

    private void OnTriggerStay2D(Collider2D collision) // When player is on the ground
    {
        if (collision.gameObject.GetComponent<TilemapCollider2D>())
        {
            PlayerController.Instance.SetOnGround(true);
            //Debug.Log("Grounded");

        }

        if (collision.gameObject.GetComponent<MovingPlatform>())
        {
            PlayerController.Instance.SetOnGround(true);
        }
    }
}
