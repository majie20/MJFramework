using UnityEngine;

namespace Model
{
    public class ActiveHandle2D : MonoBehaviour
    {
        private static Vector3 VEC_10000 = new Vector3(10000, 10000, 10000);

        private Rigidbody2D _rigidbody2D;
        private Collider2D  _collider2D;

        private Vector3 _curLocalPosition;
        private Vector3 _curLocalScale;
        private bool    _isKinematic;
        private bool    _isCollider;

        private void Awake()
        {
            this._rigidbody2D = GetComponent<Rigidbody2D>();
            this._collider2D = GetComponent<Collider2D>();
        }

        public void SetActiveByLocalPosition(bool isActive)
        {
            if (isActive)
            {
                if (gameObject.activeSelf)
                {
                    if (_collider2D != null)
                    {
                        _collider2D.enabled = _isCollider;
                    }

                    if (_rigidbody2D != null)
                    {
                        _rigidbody2D.isKinematic = _isKinematic;
                    }

                    transform.localPosition = _curLocalPosition;
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
                    if (_collider2D != null)
                    {
                        _isCollider = _collider2D.enabled;
                        _collider2D.enabled = false;
                    }

                    if (_rigidbody2D != null)
                    {
                        _isKinematic = _rigidbody2D.isKinematic;
                        _rigidbody2D.isKinematic = true;
                    }

                    _curLocalPosition = transform.localPosition;
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
                    if (_collider2D != null)
                    {
                        _collider2D.enabled = _isCollider;
                    }

                    if (_rigidbody2D != null)
                    {
                        _rigidbody2D.isKinematic = _isKinematic;
                    }

                    transform.localScale = _curLocalScale;
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
                    if (_collider2D != null)
                    {
                        _isCollider = _collider2D.enabled;
                        _collider2D.enabled = false;
                    }

                    if (_rigidbody2D != null)
                    {
                        _isKinematic = _rigidbody2D.isKinematic;
                        _rigidbody2D.isKinematic = true;
                    }

                    _curLocalScale = transform.localScale;
                    transform.localScale = Vector3.zero;
                }
            }
        }
    }
}