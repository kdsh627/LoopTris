using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private int _rotateCount;
    [SerializeField] private BlockSO _blockData;
    [SerializeField] private float _rotateDuration;
    [SerializeField] private Vector3 _rotateAngle;
    [SerializeField] private bool isRotate;
    private Transform _transform;

    public void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void Start()
    {
        Rotate().Forget();
    }

    private void OnDisable()
    {
    }

    public async UniTaskVoid Rotate()
    {
        if (isRotate) return;

        isRotate = true;
        await _transform.DORotate(_rotateAngle, _rotateDuration)
                  .SetEase(Ease.OutCirc)
                  .SetRelative(true)
                  .ToUniTask(cancellationToken: this.destroyCancellationToken);
        isRotate = false;
    }
}
