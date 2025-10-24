using Grid;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    [SerializeField] private GameObject _blockObj;
    [SerializeField] private Transform _spawnTransform;

    private Block _block;

    private void Awake()
    {
        _spawnTransform.gameObject.SetActive(false);

        SetBlock(_blockObj.transform);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            RotateBlock();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveController(MoveDirection.Right);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MoveController(MoveDirection.Left);
        }
    }

    public void SetBlock(Transform blockTransform)
    {
        blockTransform.parent = transform;
        blockTransform.position = _spawnTransform.position;
        _block = blockTransform.GetComponent<Block>();
    }

    public void RotateBlock()
    {
        _block.Rotate().Forget();
    }

    public void MoveController(MoveDirection moveDirection)
    {
        _block.Move(moveDirection);
    }
}
