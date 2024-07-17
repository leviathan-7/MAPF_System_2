using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAPF_System
{
    public class Unit
    {
        private int X_Board;
        private int Y_Board;
        private int id;
        private int x;
        private int y;
        private int last__x;
        private int last__y;
        private int x_Purpose;
        private int y_Purpose;
        private bool was_step;
        private int[,] Arr;


        public bool flag;


        public Unit(int [,] Arr, int x, int y, int x_Purpose, int y_Purpose, int id, int last__x, int last__y, int X, int Y, bool was_step = false, bool flag = false) {
            this.id = id;
            this.was_step = was_step;
            this.flag = flag;
            X_Board = X;
            Y_Board = Y;
            // Задание местоположения юнита
            this.x = x;
            this.y = y;
            // Задание местоположения цели юнити
            this.x_Purpose = x_Purpose;
            this.y_Purpose = y_Purpose;
            this.last__x = last__x;
            this.last__y = last__y;
            // Массив с количеством посещений узлов
            this.Arr = Arr;
        }
        public Unit(string str, int i, int X, int Y)
        {
            flag = false;
            was_step = false;
            string[] arr = str.Split(' ');
            X_Board = X;
            Y_Board = Y;
            // Задание параметров юнита на основе строки из файла
            x = int.Parse(arr[0]);
            y = int.Parse(arr[1]);
            x_Purpose = int.Parse(arr[2]);
            y_Purpose = int.Parse(arr[3]);
            id = i;
            last__x = -1;
            last__y = -1;
            // Массив с количеством посещений узлов
            Arr = new int[X, Y];
        }
        public Unit Copy(bool b = false) 
        {
            if (b)
                Arr = new int[X_Board, Y_Board];
            return new Unit(Arr, x, y, x_Purpose, y_Purpose, id, last__x, last__y, X_Board, Y_Board, was_step, flag); 
        }
        public int X() { return x; }
        public int Y() { return y; }
        public int X_Purpose() { return x_Purpose; }
        public int Y_Purpose() { return y_Purpose; }
        public int Id() { return id; }
        public string ToStr() { return x + " " + y + " " + x_Purpose + " " + y_Purpose; }
        public bool IsRealEnd() { return (x == x_Purpose) && (y == y_Purpose); }
        public bool Move(Tuple<int, int> C, bool b)
        {
            if (b)
            {
                x = C.Item1;
                y = C.Item2;
            }
            else
            {
                x_Purpose = C.Item1;
                y_Purpose = C.Item2;
            }
            return true;
        }
        public void NewArr(int X, int Y)
        {
            X_Board = X;
            Y_Board = Y;
            Arr = new int[X, Y];
        }
        
        
        //

        public List<Unit> MakeStep(Board Board, IEnumerable<Unit> was_step, IEnumerable<Unit> units)
        {
            List <Unit> lstUnits = new List<Unit>();
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };
            for (int i = 0; i < 4; i++)
                if (Board.IsEmpthy(x + dx[i], y + dy[i]) && NoOneCell(x + dx[i], y + dy[i], was_step, units))
                {
                    Unit U = Copy();
                    U.x = x + dx[i];
                    U.y = y + dy[i];
                    lstUnits.Add(U);
                }

            if (NoOneCell(x, y, was_step, units))
                lstUnits.Add(Copy());

            return lstUnits;
        }

        private bool NoOneCell(int _x, int _y, IEnumerable<Unit> was_step, IEnumerable<Unit> units)
        {
            foreach (var unit in was_step)
                if ((unit.x == _x) && (unit.y == _y))
                    return false;

            foreach (var unit in units)
                if ((unit.x == _x) && (unit.y == _y))
                    foreach (var u in was_step)
                        if((u.id == unit.id) && (u.x == x) && (u.y == y))
                            return false;

            return true;
        }

        public void PlusArr()
        {
            Arr[x, y]++;
        }

        public int Manheton(Board board)
        {
            int s = FindMin(x, y, board, true);

            Tunell T = board.Tunell(x, y);
            if (!(T is null) && !T.Ids(board).Contains(id) && (T.Ids(board).Count() == 0 || T.Ids(board).Last() != id))
                return 1000 + s + Arr[x, y];

            return s != 0 ? s + Arr[x, y] : 0;
        }

        private int FindMin(int x, int y, Board board, bool iter)
        {
            int s = RealManheton(x, y);
            if (s <= 1)
                return s;
            List<int> list = new List<int>();
            if (iter)
            {
                if (board.IsEmpthy(x + 1, y))
                    list.Add(FindMin(x + 1, y, board, !iter));
                if (board.IsEmpthy(x - 1, y))
                    list.Add(FindMin(x - 1, y, board, !iter));
                if (board.IsEmpthy(x, y + 1))
                    list.Add(FindMin(x, y + 1, board, !iter));
                if (board.IsEmpthy(x, y - 1))
                    list.Add(FindMin(x, y - 1, board, !iter));
            }
            else 
            {
                if (board.IsEmpthy(x + 1, y))
                    list.Add(RealManheton(x + 1, y));
                if (board.IsEmpthy(x - 1, y))
                    list.Add(RealManheton(x - 1, y));
                if (board.IsEmpthy(x, y + 1))
                    list.Add(RealManheton(x, y + 1));
                if (board.IsEmpthy(x, y - 1))
                    list.Add(RealManheton(x, y - 1));
            }
            return 1 + list.Min();
        }

        public int RealManheton()
        {
            return RealManheton(x, y);
        }

        private int RealManheton(int x, int y)
        {
            return Math.Abs(x_Purpose - x) + Math.Abs(y_Purpose - y);
        }

        public HashSet<Unit> FindClaster(List<Unit> units)
        {
            HashSet<Unit> claster = new HashSet<Unit>() { this };
            Stack<Unit> stack = new Stack<Unit>();
            stack.Push(this);
            while (stack.Count() != 0)
            {
                Unit u = stack.Pop();
                foreach (var unit in units)
                    if((!claster.Contains(unit)) && 
                        ((((u.x+1 == unit.x) || (u.x -1 == unit.x)) && ((u.y == unit.y) || (u.y - 1 == unit.y) || (u.y + 1 == unit.y))) ||
                        (((u.x + 2 == unit.x) || (u.x - 2 == unit.x)) && (u.y == unit.y)) ||
                        ((u.x == unit.x) && ((u.y - 1 == unit.y) || (u.y + 1 == unit.y) || (u.y - 2 == unit.y) || (u.y + 2 == unit.y)))))
                    {
                        claster.Add(unit);
                        stack.Push(unit);
                    }
            }

            return claster;
        }

    }
}
