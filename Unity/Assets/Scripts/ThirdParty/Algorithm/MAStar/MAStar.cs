using System;
using System.Collections.Generic;

namespace M.Algorithm
{
    public class Grid
    {
        public          int  X;
        public          int  Y;
        public          int  G;
        public          int  H;
        public          int  F;
        public          byte State; //0：密封的，1：活跃的，2：关闭的
        public          Grid Parent;
        public readonly int  Hash;

        public Grid(int x, int y)
        {
            X = x;
            Y = y;
            Hash = (X * 397) ^ Y;
        }

        public void SetParent(Grid parent, int g)
        {
            Parent = parent;
            G = parent.G + g;
            F = G + H;
        }

        public override bool Equals(object obj)
        {
            return obj is Grid grid && Equals(grid);
        }

        public bool Equals(Grid grid)
        {
            return Hash == grid.Hash;
        }

        public override int GetHashCode()
        {
            return Hash;
        }
    }

    public class MAStar
    {
        private static bool Compare(Grid a, Grid b)
        {
            return a.F <= b.F;
        }

        private static readonly Func<Grid, Grid, bool> COMPARE = Compare;

        private readonly Dictionary<int, Grid>                 _pool = new Dictionary<int, Grid>();
        private readonly StaticLinkedListDictionary<int, Grid> _openList;
        protected        int[]                                 _results;
        protected        int[,]                                _map;

        protected int _width;
        protected int _height;

        private Grid  _start;
        private Grid  _end;
        private float _w;
        private int   _sum;

        public MAStar(int[,] map, int len, int jump, float w)
        {
            _map = map;
            _width = _map.GetLength(1);
            _height = _map.GetLength(0);
            _results = new int[len];
            _openList = new StaticLinkedListDictionary<int, Grid>(jump, 20);
            _sum = _width + _height;
            _w = _sum / w;
        }

        public bool Find(int startX, int startY, int endX, int endY, List<Grid> grids)
        {
            Clear();
            grids.Clear();
            _end = HatchEnd(endX, endY);
            _start = Hatch(startX, startY);
            _openList.Add(_start.GetHashCode(), _start);

            Grid cur = Pop();

            while (true)
            {
                if (cur == null)
                {
                    break;
                }

                if (cur.Equals(_end))
                {
                    while (true)
                    {
                        if (grids.Count == 0)
                        {
                            grids.Add(cur);
                        }
                        else
                        {
                            var grid = grids[^1];

                            if (cur.Parent == null || !ComparePos(grid, cur))
                            {
                                grids.Add(cur);
                            }
                        }

                        if (cur.Equals(_start))
                        {
                            break;
                        }

                        cur = cur.Parent;
                    }

                    break;
                }

                var count = GetEffective(cur.X, cur.Y);

                for (int i = 0; i < count; i++)
                {
                    var x = _results[i] % _width;
                    var y = _results[i] / _width;
                    Join(Hatch(x, y), cur);
                }

                cur.State = 2;
                cur = Pop();
            }

            return grids.Count > 0;
        }

        private void Clear()
        {
            _openList.Clear();

            foreach (var grid in _pool.Values)
            {
                grid.State = 0;
            }
        }

        private Grid Hatch(int x, int y)
        {
            if (_pool.TryGetValue((x * 397) ^ y, out var grid))
            {
                if (grid.State == 0)
                {
                    grid.H = (int)(ComputeCost(grid, _end) / _w * _sum);
                    grid.State = 1;
                    grid.Parent = null;
                }

                return grid;
            }

            grid = new Grid(x, y);
            grid.H = (int)(ComputeCost(grid, _end) / _w * _sum);
            grid.State = 1;
            _pool[grid.GetHashCode()] = grid;

            return grid;
        }

        private Grid HatchEnd(int x, int y)
        {
            if (_pool.TryGetValue((x * 397) ^ y, out var grid))
            {
                grid.H = 0;
                grid.State = 0;
                grid.Parent = null;

                return grid;
            }

            grid = new Grid(x, y);
            grid.H = 0;
            _pool[grid.GetHashCode()] = grid;

            return grid;
        }

        private void Join(Grid grid, Grid parent)
        {
            if (grid.State == 2)
            {
                return;
            }

            var hash = grid.GetHashCode();

            if (_openList.ContainsKey(hash))
            {
                var g = (int)(ComputeCost(grid, parent) * _w);

                if (parent.G + g + grid.H < grid.F)
                {
                    grid.SetParent(parent, g);
                    _openList.Remove(hash);
                    _openList.Add(hash, grid, COMPARE);
                }
            }
            else
            {
                grid.SetParent(parent, (int)(ComputeCost(grid, parent) * _w));
                _openList.Add(hash, grid, COMPARE);
            }
        }

        private Grid Pop()
        {
            if (_openList.Length > 0)
            {
                var grid = _openList.GetElement(_openList.GetElement(1).right).element;
                _openList.Remove(grid.Hash);

                return grid;
            }

            return null;
        }

        public virtual int GetEffective(int x, int y)
        {
            return 0;
        }

        public virtual float ComputeCost(Grid a, Grid b)
        {
            return 0;
        }

        public virtual bool ComparePos(Grid a, Grid b)
        {
            return true;
        }
    }
}