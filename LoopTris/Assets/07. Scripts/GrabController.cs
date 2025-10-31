using Grid;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private Block _block;
    [SerializeField] private Transform _spawnTransform;

    private void Awake()
    {
        _spawnTransform.gameObject.SetActive(false);

        SetBlock(_block);
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
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveController(MoveDirection.Down);
        }
    }

    public void SetBlock(Block block)
    {
        _block = block;
        block.transform.position = _spawnTransform.position;
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
