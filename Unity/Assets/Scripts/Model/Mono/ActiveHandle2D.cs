using UnityEngine;

public class ActiveHandle2D : MonoBehaviour
{
    private static Vector3 VEC_10000 = new Vector3(10000, 10000, 10000);

    private Rigidbody2D rigidbody2D;
    private Collider2D collider2D;

    private Vector3 curLocalPosition;
    private Vector3 curLocalScale;
    private bool isKinematic;
    private bool isCollider;

    private void Awake()
    {
        this.rigidbody2D = GetComponent<Rigidbody2D>();
        this.collider2D = GetComponent<Collider2D>();
    }

    public void SetActiveByLocalPosition(bool isActive)
    {
        if (isActive)
        {
            if (gameObject.activeSelf)
            {
                if (collider2D != null)
                {
                    collider2D.enabled = isCollider;
                }
                if (rigidbody2D != null)
                {
                    rigidbody2D.isKinematic = isKinematic;
                }

                transform.localPosition = curLocalPosition;
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
        else
        {
            if (gameObject.activeSelf)
            {
                if (collider2D != null)
                {
                    isCollider = collider2D.enabled;
                    collider2D.enabled = false;
                }

                if (rigidbody2D != null)
                {
                    isKinematic = rigidbody2D.isKinematic;
                    rigidbody2D.isKinematic = true;
                }

                curLocalPosition = transform.localPosition;
                transform.localPosition = VEC_10000;
            }
        }
    }

    public void SetActiveByLocalScale(bool isActive)
    {
        if (isActive)
        {
            if (gameObject.activeSelf)
            {
                if (collider2D != null)
                {
                    collider2D.enabled = isCollider;
                }
                if (rigidbody2D != null)
                {
                    rigidbody2D.isKinematic = isKinematic;
                }

                transform.localScale = curLocalScale;
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
        else
        {
            if (gameObject.activeSelf)
            {
                if (collider2D != null)
                {
                    isCollider = collider2D.enabled;
                    collider2D.enabled = false;
                }

                if (rigidbody2D != null)
                {
                    isKinematic = rigidbody2D.isKinematic;
                    rigidbody2D.isKinematic = true;
                }

                curLocalScale = transform.localScale;
                transform.localScale = Vector3.zero;
            }
        }
    }
}