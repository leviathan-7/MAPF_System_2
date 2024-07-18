using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Security;
using System.Windows.Forms;
using System.Media;
using System.Collections;

namespace MAPF_System
{
    public class Board
    {
        private List<Unit> units;
        private List<Tunell> tunells;
        private Cell[,] Arr;
        private int X;
        private int Y;
        private Random rnd;
        private string name;
        private bool WasGame;
        private bool AreNotTunells;

        public Board(int X, int Y, int Blocks, int N_Units)
        {
            name = "";
            this.X = X;
            this.Y = Y;
            Arr = new Cell[X, Y];
            rnd = new Random();
            tunells = new List<Tunell>();
            // Генерация пустых узлов
            int N = X * Y - Blocks - 1;
            int x = rnd.Next(X);
            int y = rnd.Next(Y);
            int x_sum = x;
            int y_sum = y;
            int kol = 1;
            Arr[x, y] = new Cell(false);
            while (N > 0)
            {
                int x1 = rnd.Next(X);
                int y1 = rnd.Next(Y);
                int x2 = rnd.Next(X);
                int y2 = rnd.Next(Y);
                int x3 = rnd.Next(X);
                int y3 = rnd.Next(Y);
                if (Math.Abs(x1 - x_sum / kol) > Math.Abs(x2 - x_sum / kol))
                    x = x1;
                else
                    x = x2;
                if (Math.Abs(y1 - y_sum / kol) > Math.Abs(y2 - y_sum / kol))
                    y = y1;
                else
                    y = y2;
                if (Math.Abs(x - x_sum / kol) < Math.Abs(x3 - x_sum / kol))
                    x = x3;
                if (Math.Abs(y - y_sum / kol) < Math.Abs(y3 - y_sum / kol))
                    y = y3;
                x_sum += x;
                y_sum += y;
                kol++;
                bool a = (x == 0) || (Arr[x - 1, y] is null);
                bool b = (x == X - 1) || (Arr[x + 1, y] is null);
                bool c = (y == 0) || (Arr[x, y - 1] is null);
                bool d = (y == Y - 1) || (Arr[x, y + 1] is null);
                if (Arr[x, y] is null && !(a && b && c && d))
                {
                    Arr[x, y] = new Cell(false);
                    N--;
                }
            }
            // Генерация юнитов
            units = new List<Unit>();
            int id = 0;
            while (N_Units > 0)
            {
                x = rnd.Next(X);
                y = rnd.Next(Y);
                int x_Purpose = rnd.Next(X);
                int y_Purpose = rnd.Next(Y);
                bool b = (Arr[x, y] != null) && (Arr[x_Purpose, y_Purpose] != null) && !((x == x_Purpose) && (y == y_Purpose));
                foreach (var Unit in units)
                    b = b && !((Unit.X() == x) && (Unit.Y() == y)) && !((Unit.X_Purpose() == x_Purpose) && (Unit.Y_Purpose() == y_Purpose))
                        && !((Unit.X() == x_Purpose) && (Unit.Y() == y_Purpose)) && !((Unit.X_Purpose() == x) && (Unit.Y_Purpose() == y));
                if (b)
                {
                    units.Add(new Unit(new int[X, Y], x, y, x_Purpose, y_Purpose, id, -1, -1, X, Y));
                    N_Units--;
                    id++;
                }
            }
            // Генерация препятствий
            for (int i = 0; i < X; i++)
                for (int j = 0; j < Y; j++)
                    if (Arr[i, j] is null)
                        Arr[i, j] = new Cell(true);
        }
        public Board(int X, int Y, Cell[,] Arr, List<Unit> units, string name, List<Tunell> tunells)
        {
            this.name = name;
            this.X = X;
            this.Y = Y;
            this.Arr = Arr;
            this.units = units;
            this.tunells = tunells;
        }
        public Board()
        {
            // Открытие файла в формате board
            OpenFileDialog openFileDialog1 = new OpenFileDialog() { Filter = "(*.board)|*.board", };
            openFileDialog1.ShowDialog();
            // Проверка на то, что board файл был выбран
            if (openFileDialog1.FileName == "")
                return;
            Constructor(openFileDialog1.FileName);
        }
        public Board(string path) { Constructor(path); }
        public Board CopyWithoutBlocks()
        {
            Cell[,] CopyArr = new Cell[X, Y];
            List<Unit> CopyUnits = new List<Unit>();
            // Скопировать юнитов
            foreach (var Unit in units)
                CopyUnits.Add(Unit.Copy(true));
            // Скопировать доску без блоков
            for (int i = 0; i < X; i++)
                for (int j = 0; j < Y; j++)
                    CopyArr[i, j] = Arr[i, j].CopyWithoutBlock();
            return new Board(X, Y, CopyArr, CopyUnits, name, tunells);
        }
        public void Draw(Graphics t, bool b = true, Tuple<int, int> C = null, Tuple<int, int> C1 = null, bool viewtunnel = true)
        {
            int height = 18;
            if (Math.Max(X, Y) < 30)
                height = 24;
            int YY = 115;
            int XX = 95;
            using (Graphics g = t)
            {
                if (b)
                    g.Clear(SystemColors.Control); // Очистка области рисования
                using (Pen pen = new Pen(Color.Blue, 1))
                {
                    var Size = new Size(height, height);
                    var Font = new Font("Arial", 9, FontStyle.Bold);
                    var Font1 = new Font("Arial", 7, FontStyle.Bold);
                    for (int i = 0; i < X; i++)
                    {
                        if (b)
                            g.DrawString("" + i, Font1, Brushes.Coral, new Point(XX + 9 + height * i, YY - 7));
                        for (int j = 0; j < Y; j++)
                        {
                            Rectangle rect = new Rectangle(new Point(XX + 5 + height * i, YY + 5 + height * j), Size);
                            if (b)
                            {
                                g.DrawString("" + j, Font1, Brushes.Coral, new Point(XX - 7, YY + 9 + height * j));
                                g.DrawRectangle(pen, rect);
                            }
                            // Отрисовка блоков
                            if (Arr[i, j].IsBlock())
                                g.FillRectangle(Brushes.Black, rect);
                            // Отрисовка пройденного пути
                            if (Arr[i, j].WasVisit())
                            {
                                int W = 100 + 100 * (Arr[i, j].IdVisit() + 1) / units.Count;
                                g.FillRectangle(new SolidBrush(Color.FromArgb(W, W, 255)), rect);
                            }
                            // Отрисовка плохих узлов
                            if (Arr[i, j].IsBad())
                                g.DrawString("X", Font1, Brushes.Red, new Point(XX + 9 + height * i, YY + 9 + height * j));
                            // Отрисовка туннеля
                            if (viewtunnel && Arr[i, j].IsTunell())
                                g.DrawRectangle(new Pen(Color.Red, 1), new Rectangle(new Point(XX + 6 + height * i, YY + 6 + height * j), new Size(height - 2, height - 2)));
                        }
                    }
                    Size = new Size(height - 5, height - 5);
                    foreach (var Unit in units)
                    {
                        // Отрисовка цели
                        if (!Unit.IsRealEnd())
                        {
                            g.FillRectangle(Brushes.LawnGreen, new Rectangle(new Point(XX + 8 + height * Unit.X_Purpose(), YY + 8 + height * Unit.Y_Purpose()), Size));
                            g.DrawString("" + Unit.Id(), Font, Brushes.Black, new Point(XX + 8 + height * Unit.X_Purpose(), YY + 8 + height * Unit.Y_Purpose()));
                        }
                    }
                    foreach (var Unit in units)
                    {
                        // Отрисовка юнитов
                        g.FillRectangle(Brushes.Red, new Rectangle(new Point(XX + 8 + height * Unit.X(), YY + 8 + height * Unit.Y()), Size));
                        g.DrawString("" + Unit.Id(), Font, Brushes.Black, new Point(XX + 8 + height * Unit.X(), YY + 8 + height * Unit.Y()));
                    }
                    if (!(C is null))
                    {
                        Size = new Size(height, height);
                        g.FillRectangle(Brushes.White, new Rectangle(new Point(XX + 5 + height * C.Item1, YY + 5 + height * C.Item2), Size));
                        g.DrawRectangle(pen, new Rectangle(new Point(XX + 5 + height * C.Item1, YY + 5 + height * C.Item2), Size));
                        if (!(C1 is null))
                        {
                            g.FillRectangle(Brushes.White, new Rectangle(new Point(XX + 5 + height * C1.Item1, YY + 5 + height * C1.Item2), Size));
                            g.DrawRectangle(pen, new Rectangle(new Point(XX + 5 + height * C1.Item1, YY + 5 + height * C1.Item2), Size));
                        }
                    }
                }
            }
        }
        public string Save(string name_, bool b = false)
        {
            try
            {
                StreamWriter sw = (new FileInfo(name_ + ".board")).CreateText();
                sw.WriteLine(X + " " + Y + " " + units.Count + " " + b);
                // Записать в файл доску с блоками и пройденным путем
                for (int i = 0; i < X; i++)
                    for (int j = 0; j < Y; j++)
                        sw.WriteLine(Arr[i, j].ToStr());
                // Записать в файл юнитов
                foreach (var item in units)
                    sw.WriteLine(item.ToStr());
                sw.Close();
                name = name_ + ".board";
            }
            catch (Exception e) { }
            return name;
        }
        

        //
        
        public void MakeStep(Board Board)
        {
            // Добавить блоки в пределах видимости юнитов
            foreach (var Unit in units)
            {
                MakeBlock(Board, Unit.X(), Unit.Y() - 1);
                MakeBlock(Board, Unit.X(), Unit.Y() + 1);
                MakeBlock(Board, Unit.X() - 1, Unit.Y());
                MakeBlock(Board, Unit.X() + 1, Unit.Y());
            }

            // Добавить узлы -- части туннелей
            if (!AreNotTunells)
                while (true)
                {
                    int k = 0;
                    for (int i = 0; i < X; i++)
                        for (int j = 0; j < Y; j++)
                            if (!(IsTunell(i, j) || Arr[i, j].IsBlock()))
                            {
                                int kk = 0;
                                kk += TunellAndNoEmpthy(i - 1, j);
                                kk += TunellAndNoEmpthy(i + 1, j);
                                kk += TunellAndNoEmpthy(i, j - 1);
                                kk += TunellAndNoEmpthy(i, j + 1);

                                if (!AreNotTunells && kk == 3)
                                {
                                    List<Tunell> LT = new List<Tunell>();
                                    LT_ADD(LT, i - 1, j);
                                    LT_ADD(LT, i + 1, j);
                                    LT_ADD(LT, i, j - 1);
                                    LT_ADD(LT, i, j + 1);

                                    var T = new Tunell(this);
                                    T.Add(LT);
                                    Arr[i, j].MakeTunell(T);
                                    tunells.Add(T);
                                    T.Add(i, j);
                                }

                                if (kk == 4 && Arr[i,j].WasVisit())
                                {
                                    AreNotTunells = true;
                                    for (int ii = 0; ii < X; ii++)
                                        for (int jj = 0; jj < Y; jj++)
                                            Arr[ii, jj].ClearTunell();
                                }
                            }

                    if (k == 0)
                        break;
                }

            foreach (var Unit in units)
            {
                Arr[Unit.X(), Unit.Y()].MakeVisit(Unit.Id());
                Unit.PlusArr();
                if (Unit.IsRealEnd())
                    Unit.ClearArr();
            }

            var new_units = new List<Unit>();
            foreach (var Claster in Clasterization(units))
                new_units.AddRange(NewUnits(Claster));
            units = new_units;

        }

        private List<HashSet<Unit>> Clasterization(List<Unit> units)
        {
            HashSet<Unit> clasterizations = new HashSet<Unit>();
            List<HashSet<Unit>> clasters = new List<HashSet<Unit>>();
            foreach (var unit in units)
                if (!clasterizations.Contains(unit))
                {
                    HashSet<Unit> claster = unit.FindClaster(units);
                    clasters.Add(claster);
                    clasterizations.UnionWith(claster);
                }
            return clasters;
        }

        private IEnumerable<Unit> NewUnits(IEnumerable<Unit> claster)
        {
            Tuple<IEnumerable<Unit>, IEnumerable<Unit>> TT = new Tuple<IEnumerable<Unit>, IEnumerable<Unit>>(new List<Unit>(), claster);
            int min_sum = claster.Sum(unit => unit.RealManheton()) - claster.Count(unit => !unit.IsRealEnd());
            IEnumerable<Unit> res = NewUnitsStack(TT, min_sum, claster, true);
            if (!(res is null))
                return res;
            return NewUnitsStack(TT, min_sum, claster, false);
        }

        private IEnumerable<Unit> NewUnitsStack(Tuple<IEnumerable<Unit>, IEnumerable<Unit>> TT, int min_sum, IEnumerable<Unit> claster, bool b)
        {
            int sum = int.MaxValue;
            Stack<Tuple<IEnumerable<Unit>, IEnumerable<Unit>>> stack = new Stack<Tuple<IEnumerable<Unit>, IEnumerable<Unit>>>();
            stack.Push(TT);
            IEnumerable<Unit> res = null;
            while (stack.Count() != 0)
            {
                var T = stack.Pop();
                if (T.Item2.Count() == 0)
                {
                    var s = T.Item1.Sum(unit => unit.Manheton(this));
                    if (s == min_sum)
                        return T.Item1;
                    if ((s < sum) && isntEqw(claster, T.Item1, b))
                    {
                        sum = s;
                        res = T.Item1;
                    }
                }
                else
                    foreach (var unit in T.Item2.First().MakeStep(this, T.Item1, claster, b))
                        stack.Push(new Tuple<IEnumerable<Unit>, IEnumerable<Unit>>(T.Item1.Concat(new List<Unit> { unit }), T.Item2.Skip(1)));
            }
            return res;
        }

        private bool isntEqw(IEnumerable<Unit> units1, IEnumerable<Unit> units2, bool b)
        {
            var sort1 = units1.OrderBy(unit => unit.Id()).ToList();
            var sort2 = units2.OrderBy(unit => unit.Id()).ToList();
            for (int i = 0; i < sort1.Count(); i++)
                if ((sort1[i].X() != sort2[i].X()) || (sort1[i].Y() != sort2[i].Y()))
                {
                    if (sort1[i].IsRealEnd() && !sort2[i].IsRealEnd())
                    {
                        Tunell T = Tunell(sort1[i].X(), sort1[i].Y());
                        if (!(T is null) && !T.RealIds().Contains(sort1[i].Id()))
                            return true;
                    }
                    else
                        return true;
                }


            return false;
        }

        public Tunell Tunell(int x, int y) 
        { 
            return Arr[x, y].Tunell(); 
        }

        public bool InTunell(int unit_id, Tunell tunell) 
        {
            Unit unit = units.Find(u => u.Id() == unit_id);
            return Arr[unit.X(), unit.Y()].Tunell() == tunell; 
        }

        //

        public bool IsEnd()
        {
            bool b = true;
            // Проверяем, что все юниты дошли до своих целей
            foreach (var Unit in units)
                b = b && Unit.IsRealEnd();
            return b;
        }
        public bool IsEmpthy(int x, int y)
        {
            // Проверка на выход за пределы поля
            if ((x < 0) || (y < 0) || (x >= X) || (y >= Y))
                return false;
            return !Arr[x, y].IsBlock();
        }
        public bool IsTunell(int x, int y)
        {
            // Проверка на выход за пределы поля
            if ((x < 0) || (y < 0) || (x >= X) || (y >= Y))
                return false;
            return Arr[x, y].IsTunell();
        }
        public string Name() { return name; }
        public List<Unit> Units() { return units; }
        
        public int GET_X() { return X; }
        public int GET_Y() { return Y; }
        public bool GetWasGame() { return WasGame; }
        public int ReversBlock(Tuple<int, int> c)
        {
            // Проверка на выход за пределы поля
            if ((c.Item1 < 0) || (c.Item2 < 0) || (c.Item1 >= X) || (c.Item2 >= Y))
                return 0;
            foreach (var unit in units)
                if (((unit.X() == c.Item1) && (unit.Y() == c.Item2)) || ((unit.X_Purpose() == c.Item1) && (unit.Y_Purpose() == c.Item2)))
                    return 0;
            return Arr[c.Item1, c.Item2].ReversBlock();
        }
        public bool Move(Tuple<int, int> C0, Tuple<int, int> C1)
        {
            if (!IsEmpthy(C1.Item1, C1.Item2))
                return false;
            foreach (var unit in units)
                if (((unit.X() == C1.Item1) && (unit.Y() == C1.Item2)) || ((unit.X_Purpose() == C1.Item1) && (unit.Y_Purpose() == C1.Item2)))
                    return false;
            foreach (var unit in units)
            {
                if ((unit.X() == C0.Item1) && (unit.Y() == C0.Item2))
                    return unit.Move(C1, true);
                if ((unit.X_Purpose() == C0.Item1) && (unit.Y_Purpose() == C0.Item2))
                    return unit.Move(C1, false);
            }
            return false;
        }
        public void PlusUnit()
        {
            int Blocks = 0;
            for (int i = 0; i < X; i++)
                for (int j = 0; j < Y; j++)
                    if (Arr[i, j].IsBlock())
                        Blocks++;
            if ((Blocks + 2 * units.Count + 2) >= (X * Y))
            {
                SystemSounds.Beep.Play();
                return;
            }
            rnd = new Random();
            while (true)
            {
                int x = rnd.Next(X);
                int y = rnd.Next(Y);
                int x_Purpose = rnd.Next(X);
                int y_Purpose = rnd.Next(Y);
                bool b = !Arr[x, y].IsBlock() && !Arr[x_Purpose, y_Purpose].IsBlock() && (Arr[x, y] != null) && (Arr[x_Purpose, y_Purpose] != null) && !((x == x_Purpose) && (y == y_Purpose));
                foreach (var Unit in units)
                    b = b && !((Unit.X() == x) && (Unit.Y() == y)) && !((Unit.X_Purpose() == x_Purpose) && (Unit.Y_Purpose() == y_Purpose))
                        && !((Unit.X() == x_Purpose) && (Unit.Y() == y_Purpose)) && !((Unit.X_Purpose() == x) && (Unit.Y_Purpose() == y));
                if (b)
                {
                    units.Add(new Unit(new int[X, Y], x, y, x_Purpose, y_Purpose, units.Count, -1, -1, X, Y));
                    return;
                }
            }
        }
        public Tuple<Tuple<int, int>, Tuple<int, int>> MinusUnit()
        {
            if (units.Count() <= 1)
            {
                SystemSounds.Beep.Play();
                return null;
            }
            var L = units.Last();
            units.Remove(L);
            return new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(L.X(), L.Y()), new Tuple<int, int>(L.X_Purpose(), L.Y_Purpose()));
        }
        public void PlusColumn()
        {
            X++;
            var arr = new Cell[X, Y];
            for (int i = 0; i < X - 1; i++)
                for (int j = 0; j < Y; j++)
                    arr[i, j] = Arr[i, j];
            for (int j = 0; j < Y; j++)
                arr[X - 1, j] = new Cell(false);
            Arr = arr;
            foreach (var unit in units)
                unit.NewArr(X, Y);
        }
        public void PlusRow()
        {
            Y++;
            var arr = new Cell[X, Y];
            for (int i = 0; i < X; i++)
                for (int j = 0; j < Y - 1; j++)
                    arr[i, j] = Arr[i, j];
            for (int i = 0; i < X; i++)
                arr[i, Y - 1] = new Cell(false);
            Arr = arr;
            foreach (var unit in units)
                unit.NewArr(X, Y);
        }
        public void DelBlokcs()
        {
            for (int i = 0; i < X; i++)
                for (int j = 0; j < Y; j++)
                    Arr[i, j].DelBlokcs();
        }
        public bool DelUnits()
        {
            if (units.Count() <= 1)
            {
                SystemSounds.Beep.Play();
                return false;
            }
            units = new List<Unit> { units[0] };
            return true;
        }

        private void Constructor(string path)
        {
            try
            {
                name = path.Split('\\').Last();
                string[] readText = File.ReadAllLines(path);
                string[] arr = readText[0].Split(' ');
                // Размеры поля
                X = int.Parse(arr[0]);
                Y = int.Parse(arr[1]);
                // Количество юнитов
                int N_Units = int.Parse(arr[2]);
                // Была ли игра
                WasGame = (arr[3] == "True");
                // Создать доску по данным файла
                Arr = new Cell[X, Y];
                int t = 1;
                for (int i = 0; i < X; i++)
                    for (int j = 0; j < Y; j++)
                    {
                        Arr[i, j] = new Cell(readText[t]);
                        t++;
                    }
                // Создать юнитов по данным файла
                units = new List<Unit>();
                for (int i = 0; i < N_Units; i++)
                {
                    units.Add(new Unit(readText[t], i, X, Y));
                    t++;
                }
                tunells = new List<Tunell>();
            }
            catch (Exception e)
            {
                MessageBox.Show("Файл повреждён.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool IsBlock(int x, int y)
        {
            // Проверка на выход за пределы поля
            if ((x < 0) || (y < 0) || (x >= X) || (y >= Y))
                return false;
            return Arr[x, y].IsBlock();
        }
        private void LT_ADD(List<Tunell> LT, int i, int j)
        {
            if (IsTunell(i, j))
                LT.Add(Arr[i, j].Tunell());
        }
        private int TunellAndNoEmpthy(int i, int j)
        {
            if (!IsEmpthy(i, j) || IsTunell(i, j))
                return 1;
            return 0;
        }
        private void MakeBlock(Board Board, int x, int y)
        {
            if (Board.IsBlock(x, y))
                Arr[x, y].MakeBlock();
        }

    }
}
