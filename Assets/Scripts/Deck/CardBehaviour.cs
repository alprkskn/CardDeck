using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] private CardInfo _info;

    private Vector3? _targetPosition;
    private Quaternion? _targetRotation;
    private RectTransform _rectTransform;

    private const float PositionDamping = 0.8f;
    private const float RotationDamping = 0.8f;

    public RectTransform RectTransform
    {
        get
        {
            if (!_rectTransform)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            return _rectTransform;
        }
    }

    private void Update()
    {
        if (_targetRotation.HasValue)
        {
            RectTransform.rotation = Quaternion.Lerp(RectTransform.rotation, _targetRotation.Value, RotationDamping);
        }

        if (_targetPosition.HasValue)
        {
            RectTransform.position = Vector3.Lerp(RectTransform.position, _targetPosition.Value, PositionDamping);
        }
    }

    public void SetTarget(Vector3 position, Quaternion rotation, bool immediate = false)
    {
        _targetPosition = position;
        _targetRotation = rotation;

        if (immediate)
        {
            RectTransform.position = position;
            RectTransform.rotation = rotation;
        }
    }
}
