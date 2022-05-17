using UnityEngine;

public class ActiveHandle3D : MonoBehaviour
{
    private static Vector3 VEC_10000 = new Vector3(10000, 10000, 10000);

    private Rigidbody rigidbody;
    private Collider collider;

    private Vector3 curLocalPosition;
    private Vector3 curLocalScale;
    private bool isKinematic;
    private bool isCollider;

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody>();
        this.collider = GetComponent<Collider>();
    }

    public void SetActiveByLocalPosition(bool isActive)
    {
        if (isActive)
        {
            if (gameObject.activeSelf)
            {
                if (collider != null)
                {
                    collider.enabled = isCollider;
                }
                if (rigidbody != null)
                {
                    rigidbody.isKinematic = isKinematic;
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
                if (collider != null)
                {
                    isCollider = collider.enabled;
                    collider.enabled = false;
                }

                if (rigidbody != null)
                {
                    isKinematic = rigidbody.isKinematic;
                    rigidbody.isKinematic = true;
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
                if (collider != null)
                {
                    collider.enabled = isCollider;
                }
                if (rigidbody != null)
                {
                    rigidbody.isKinematic = isKinematic;
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
                if (collider != null)
                {
                    isCollider = collider.enabled;
                    collider.enabled = false;
                }

                if (rigidbody != null)
                {
                    isKinematic = rigidbody.isKinematic;
                    rigidbody.isKinematic = true;
                }

                curLocalScale = transform.localScale;
                transform.localScale = Vector3.zero;
            }
        }
    }
}