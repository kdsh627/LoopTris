using System.Collections.Generic;
using System.Drawing;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.InputManagerEntry;

namespace Grid
{
    public enum MoveDirection
    {
        Left,
        Right,
        Down,
        Top
    }
    public static class PositionConverter
    {
        public static int Compression(int x, int y, int width)
        {
            return x + y * width;
        }

        public static (int, int) Expansion(int node, int width)
        {
            int x = ExpansionX(node, width);
            int y = ExpansionY(node, width);
            return (x, y);
        }

        public static int ExpansionX(int node, int width)
        {
            int x = node % width;
            return x;
        }

        public static int ExpansionY(int node, int width)
        {
            int y = node / width;
            return y;
        }
    }

    public class UnionFind
    {
        int size;
        int width;

        private int[] parent;
        private int[] height;

        public UnionFind(int size, int width)
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
            bool isEdgeA = (A % width == 0 || (A + 1) % width == 0);
            bool isEdgeB = (B % width == 0 || (B + 1) % width == 0);

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

    public class Graph
    {
        private List<List<int>> _graphList;
        private int _size;
        private UnionFind _unionFind;

        public List<List<int>> GraphList => _graphList;

        public Graph(int size, int width)
        {
            _size = size;
            _graphList = new List<List<int>>(size);
            _unionFind = new UnionFind(size, width);
        }

        public void Append(int fromPoint, int toPoint)
        {
            _graphList[fromPoint].Add(toPoint);
            _graphList[toPoint].Add(fromPoint);
            _unionFind.Union(fromPoint, toPoint);
        }

        public void Clear()
        {
            _graphList.Clear();
            _graphList = new List<List<int>>(_size);
        }
    }

    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private int _gridWidth;
        [SerializeField] private int _gridHeight;

        private Graph _blockGraph;
        private Block[,] _blockGrid;
        private int[] _gridColmunHeight;

        private int[] dx = new int[4] { -1, 1, 0, 0 };
        private int[] dy = new int[4] { 0, 0, -1, 1 };

        public float GridWidth => _gridWidth;
        public float GridHeight => _gridHeight;
        private float startX = 0f;
        private float endX => startX + _gridWidth;

        public void Awake()
        {
            _blockGrid = new Block[_gridHeight, _gridWidth];
            _blockGraph = new Graph(_gridWidth * _gridHeight, _gridWidth);
            _gridColmunHeight = new int[_gridHeight];
        }

        public void ReserveGrid(Block block, Vector2 blockPosition)
        {
            Debug.Log("등록");

            //해당 오브젝트 기준 로컬좌표계로 변환
            Vector2 position = _transform.InverseTransformPoint(blockPosition);

            int y = (int)position.x;
            int x = (int)position.y;

            _blockGrid[y, x] = block;
        }

        /// <summary>
        /// 월드 좌표를 매개변수로 해당 자리에 블럭을 추가하는 로직
        /// </summary>
        /// <param name="block"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddBlockGraph(Block block, Vector2 blockPosition)
        {
            //해당 오브젝트 기준 로컬좌표계로 변환
            Vector2 position = _transform.InverseTransformPoint(blockPosition);

            int y = (int)position.x;
            int x = (int)position.y;

            _gridColmunHeight[x]++;

            ConnectDirection blockConnection = block.GetConnectInfo();

            int fromPoint = PositionConverter.Compression((int)position.x, (int)position.y, _gridWidth);
            int toPoint = 0;
            //왼쪽에 연결되어있으면
            if ((blockConnection & ConnectDirection.Left) == ConnectDirection.Left)
            {
                Debug.Log("왼쪽 연결");

                //경계선 검사
                if (x - 1 < 0) return;

                if (_blockGrid[y, x - 1] == null) return;

                toPoint = PositionConverter.Compression(x - 1, y, _gridWidth);

                _blockGraph.Append(fromPoint, toPoint);
            }
            //오른쪽에 연결되어있으면
            if ((blockConnection & ConnectDirection.Right) == ConnectDirection.Right)
            {
                Debug.Log("오른쪽 연결");

                //경계선 검사
                if (x + 1 >= _gridWidth) return;

                if (_blockGrid[y, x + 1] == null) return;

                toPoint = PositionConverter.Compression(x + 1, y, _gridWidth);

                _blockGraph.Append(fromPoint, toPoint);
            }
            //위에 연결되어있으면
            if ((blockConnection & ConnectDirection.Top) == ConnectDirection.Top)
            {
                Debug.Log("위쪽 연결");

                if (y + 1 >= _gridHeight) return;

                if (_blockGrid[y + 1, x] == null) return;

                toPoint = PositionConverter.Compression(x, y + 1, _gridWidth);

                _blockGraph.Append(fromPoint, toPoint);
            }
            //아래에 연결되어있으면
            if ((blockConnection & ConnectDirection.Bottom) == ConnectDirection.Bottom)
            {
                Debug.Log("아래쪽 연결");

                //경계선 검사
                if (y - 1 < 0) return;

                if (_blockGrid[y - 1, x] == null) return;

                toPoint = PositionConverter.Compression(x, y - 1, _gridWidth);

                _blockGraph.Append(fromPoint, toPoint);
            }
        }

        /// <summary>
        /// 해당 좌표로 이동이 가능한가 반환
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsMovable(MoveDirection direction, ref Vector2 position)
        {
            Debug.Log(position);
            //해당 오브젝트 기준 로컬좌표계로 변환
            Vector2 localPosition = _transform.InverseTransformPoint(position);

            if (direction == MoveDirection.Left || direction == MoveDirection.Right)
            {
                if (IsHorizontalMovable((int)direction, ref localPosition))
                {
                    //다시 월드 좌표로 변환
                    position = _transform.TransformPoint(localPosition);
                    return true;
                }
            }
            else if (direction == MoveDirection.Down)
            {
                if (IsVerticalMovable(ref localPosition))
                {
                    //다시 월드 좌표로 변환
                    position = _transform.TransformPoint(localPosition);
                    return true;
                }
            }

            return false;
        }

        private bool IsHorizontalMovable(int direction, ref Vector2 position)
        {
            position.x += dx[direction];

            Debug.Log(position);
            if (position.x >= (startX + 1) && position.x <= (endX - 2))
            {
                return true;
            }
            return false;
        }

        private bool IsVerticalMovable(ref Vector2 position)
        {
            int x = (int)position.x;
            int y = (int)position.y;
            //현재 열의 높이가 최대 높이를 넘으면
            if (_gridColmunHeight[x] >= _gridHeight) return false;

            if (_blockGrid[y, x] != null) return false;

            position.y = _gridColmunHeight[x];
            return true;
        }
    }
}