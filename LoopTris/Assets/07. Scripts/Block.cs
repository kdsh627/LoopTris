using Cysharp.Threading.Tasks;
using DG.Tweening;
using Grid;
using NUnit;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using VContainer;

public class Block : MonoBehaviour
{
    [SerializeField] private int _rotateCount;
    [SerializeField] private BlockSO _blockData;
    [SerializeField] private float _rotateDuration;
    [SerializeField] private Vector3 _rotateAngle;
    [SerializeField] private bool isRotate;
    [SerializeField] private bool isMove;

    [SerializeField] private float _horizontalMoveDuration;
    [SerializeField] private float _verticalMoveDuration;
    [SerializeField] private GridManager _gridManager;

    private Transform _transform;

    [Inject]
    public void Construct(GridManager gridManager)
    {
        // Block이 생성되자마자 GridManager 인스턴스가 주입
        _gridManager = gridManager;
    }

    public ConnectDirection GetConnectInfo()
    {
        return _blockData.Connect(_rotateCount);
    }

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

        _rotateCount += 1;
        isRotate = false;
    }

    public void Move(MoveDirection direction)
    {
        if (isMove) return;

        isMove = true;

        switch (direction)
        {
            case MoveDirection.Left:
            case MoveDirection.Right:
                Vector2 position = _transform.position;
                if (_gridManager.IsMovable(direction, ref position))
                {
                    HorizontalMove_Task(position.x).Forget();
;                }
                else
                {
                    isMove = false;
                }
                break;
            case MoveDirection.Down:

                break;
            default:
                return;
        }
    }

    public async UniTaskVoid HorizontalMove_Task(float moveAmount)
    {
        await _transform.DOMoveX(moveAmount, _horizontalMoveDuration)
            .SetEase(Ease.InOutCirc)
            .ToUniTask(cancellationToken: this.destroyCancellationToken);

        isMove = false;
    }

    public async UniTaskVoid VerticalMove_Task(Transform transform, float moveAmount)
    {
        await transform.DOMoveX(moveAmount, _horizontalMoveDuration)
            .SetEase(Ease.InOutCirc)
            .SetRelative(true)
            .ToUniTask(cancellationToken: this.destroyCancellationToken);

        isMove = false;
    }
}
