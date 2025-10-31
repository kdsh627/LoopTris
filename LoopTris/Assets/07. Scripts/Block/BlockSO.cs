using System;
using UnityEngine;

[Flags]
public enum ConnectDirection
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

    public ConnectDirection Connect(int count)
    {
        ConnectDirection info = 0b000;
        int start = count % 4;
        for (int i = start; i < 4; i++)
        {
            if(Connection[i])
            {
                info |= (ConnectDirection)(1 << i);
            }
        }

        return info;
    }
}
