using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grid
{
    public enum MoveDirection
    {
        Left,
        Right,
        Down,
        Top
    }
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private float _gridWidth;
        [SerializeField] private float _gridHeight;

        private int[] dx = new int[4] { -1, 1, 0, 0 };
        private int[] dy = new int[4] { 0, 0, -1, 1 };

        public float GridWidth => _gridWidth;
        public float GridHeight => _gridHeight;
        private float startX => _transform.position.x;
        private float startY => _transform.position.y;
        private float endX => _transform.position.x + _gridWidth;
        private float endY => _transform.position.y + _gridHeight;

        public bool isMovable(MoveDirection direction, ref Vector2 position)
        {
            position.x += dx[(int)direction];
            position.y += dy[(int)direction];

            Debug.Log(position.x);
            if (position.x >= (startX + 1) && position.x <= (endX - 2) && position.y >= startY && position.y <= endY)
            {
                Debug.Log("통과");
                return true;
            }
            return false;
        }
    }


    public static class PositionConverter
    {
        public static int Compression(int x, int y, int size)
        {
            return x + y * size;
        }

        public static (int, int) Expansion(int node, int size)
        {
            int x = ExpansionX(node, size);
            int y = ExpansionY(node, size);
            return (x, y);
        }

        public static int ExpansionX(int node, int size)
        {
            int x = node % size;
            return x;
        }

        public static int ExpansionY(int node, int size)
        {
            int y = node / size;
            return y;
        }
    }


    public class UnionFind
    {
        int size;

        private int[] parent;
        private int[] height;

        public UnionFind(int size)
        {
            this.size = size;
            parent = new int[size];
            height = new int[size];

            for (int i = 0; i < size; ++i)
            {
                parent[i] = i;
                height[i] = 1;
            }
        }

        private int Find(int node)
        {
            //부모가 자신이라는 뜻은 루트 노드라는 뜻
            if (node == parent[node]) return node;
            return parent[node] = Find(parent[node]); //각각 노드들은 부모를 가리키고 있으므로 재귀함수로 구현가능
        }

        public void Union(int a, int b)
        {
            int A = Find(a);
            int B = Find(b);

            //같은 소속이면 그냥 리턴
            if (A == B) return;

            //각 그래프의 루트가 맵 양끝에 존재하는가 판별
            bool isEdgeA = (A % size == 0 || (A + 1) % size == 0);
            bool isEdgeB = (B % size == 0 || (B + 1) % size == 0);

            if (isEdgeA && isEdgeB)
            {
                //이러면 터뜨려야함
            }

            //최우선으로 양끝단에 있는 노드가 무조건 루트가 됨
            if (isEdgeA)
            {
                parent[A] = B;
                if (height[A] == height[B])
                {
                    height[A]++;
                }
            }
            else if (isEdgeB)
            {
                parent[B] = A;
                if (height[A] == height[B])
                {
                    height[B]++;
                }
            }
            //높이가 낮은 트리를 높은 트리로 편입
            else if (height[A] < height[B])
            {
                //A의 최상단 부모를 B로 지정한다 = A를 B아래에 편입
                parent[A] = B;
            }
            else if (height[A] > height[B])
            {
                parent[B] = A;
            }
            else
            {
                //높이가 같으면 B를 A에 편입시키고 높이를 1상승
                parent[B] = A;
                height[B]++;
            }
        }

        //A와 B가 같은 소속인지 판별하는 변수
        public bool IsMember(int A, int B)
        {
            return Find(A) == Find(B);
        }

        public void Remove(int node)
        {
            parent[node] = node;
            height[node] = 1;
        }
    }
}