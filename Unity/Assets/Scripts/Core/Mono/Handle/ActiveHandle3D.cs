using UnityEngine;

namespace Model
{
    public class ActiveHandle3D : MonoBehaviour
    {
        private static Vector3 VEC_10000 = new Vector3(10000, 10000, 10000);

        private Rigidbody _rigidbody;
        private Collider  _collider;

        private Vector3 _curLocalPosition;
        private Vector3 _curLocalScale;
        private bool    _isKinematic;
        private bool    _isCollider;

        private void Awake()
        {
            this._rigidbody = GetComponent<Rigidbody>();
            this._collider = GetComponent<Collider>();
        }

        public void SetActiveByLocalPosition(bool isActive)
        {
            if (isActive)
            {
                if (gameObject.activeSelf)
                {
                    if (_collider != null)
                    {
                        _collider.enabled = _isCollider;
                    }

                    if (_rigidbody != null)
                    {
                        _rigidbody.isKinematic = _isKinematic;
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
                    if (_collider != null)
                    {
                        _isCollider = _collider.enabled;
                        _collider.enabled = false;
                    }

                    if (_rigidbody != null)
                    {
                        _isKinematic = _rigidbody.isKinematic;
                        _rigidbody.isKinematic = true;
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
                    if (_collider != null)
                    {
                        _collider.enabled = _isCollider;
                    }

                    if (_rigidbody != null)
                    {
                        _rigidbody.isKinematic = _isKinematic;
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
                    if (_collider != null)
                    {
                        _isCollider = _collider.enabled;
                        _collider.enabled = false;
                    }

                    if (_rigidbody != null)
                    {
                        _isKinematic = _rigidbody.isKinematic;
                        _rigidbody.isKinematic = true;
                    }

                    _curLocalScale = transform.localScale;
                    transform.localScale = Vector3.zero;
                }
            }
        }
    }
}