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

    public async void Move(MoveDirection direction)
    {
        if (isMove) return;

        isMove = true;

        Vector2 position = _transform.position;
        if (_gridManager.IsMovable(direction, ref position))
        {
            switch (direction)
            {
                case MoveDirection.Left:
                case MoveDirection.Right:
                    await HorizontalMove_Task(position.x);
                    break;
                case MoveDirection.Down:
                    Debug.Log("뭐지");
                    _gridManager.ReserveGrid(this, position);
                    await VerticalMove_Task(position.y);
                    _gridManager.AddBlockGraph(this, position);
                    break;
                default:
                    return;
            }
        }
        else
        {
            Debug.Log("실패");
            isMove = false;
        }
    }

    public async UniTask HorizontalMove_Task(float endValue)
    {
        await _transform.DOMoveX(endValue, _horizontalMoveDuration)
            .SetEase(Ease.InCirc)
            .ToUniTask(cancellationToken: this.destroyCancellationToken);

        isMove = false;
    }

    public async UniTask VerticalMove_Task(float endValue)
    {
        await _transform.DOMoveY(endValue, _verticalMoveDuration)
            .SetEase(Ease.InCirc)
            .ToUniTask(cancellationToken: this.destroyCancellationToken);

        isMove = false;
    }
}
