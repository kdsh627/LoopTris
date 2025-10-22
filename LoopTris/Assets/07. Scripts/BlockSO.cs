using UnityEngine;

public enum Direction
{ 
    Top = 0b0001,
    Right = 0b0010,
    Bottom = 0b0100,
    Left = 0b1000
}

[CreateAssetMenu(fileName = "BlockSO", menuName = "Scriptable Objects/BlockSO")]
public class BlockSO : ScriptableObject
{
    [SerializeField] private bool[] Connection;

    public int Connect(int count)
    {
        int info = 0;
        int start = count % 4;
        for (int i = start; i < 4; i++)
        {
            if(Connection[i])
            {
                info |= (1 << i);
            }
        }

        return info;
    }
}
