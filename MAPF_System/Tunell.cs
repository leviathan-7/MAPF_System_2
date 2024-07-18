using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAPF_System
{
    public class Tunell
    {
        private Board board;
        private List<int> tunell_units_id;
        private List<Tunell> tunells;

        public Tunell(Board board)
        {
            this.board = board;
            tunell_units_id = new List<int>();
            tunells = new List<Tunell>();
        }
        public void Add(int x, int y)
        {
            foreach (var Unit in board.Units())
                if ((Unit.X_Purpose() == x) && (Unit.Y_Purpose() == y))
                {
                    tunell_units_id.Add(Unit.Id());
                    break;
                }
        }
        public void Add(List<Tunell> LT)
        {
            foreach (var t in LT)
            {
                tunells.Add(t);
                foreach (var u in t.tunell_units_id)
                    tunell_units_id.Add(u);
                foreach (var tt in t.tunells)
                    tunells.Add(tt);
            }
        }

        public List<int> Ids(Board board)
        {
            List<int> lst = new List<int>();
            foreach (var Unit_id in (from unit in board.Units() select unit.Id()).Except(tunell_units_id))
                if (board.InTunell(Unit_id, this))
                    return lst;
            foreach (var Unit_id in tunell_units_id)
            {
                lst.Add(Unit_id);
                bool b = board.InTunell(Unit_id, this);
                foreach (var tunell in tunells)
                    b = b || board.InTunell(Unit_id, tunell);
                if (!b)
                    return lst;
            }
            return lst;
        }

        public List<int> RealIds()
        {
            List<int> lst = new List<int>();
            foreach (var Unit_id in tunell_units_id)
            {
                lst.Add(Unit_id);
                bool b = board.InTunell(Unit_id, this);
                foreach (var tunell in tunells)
                    b = b || board.InTunell(Unit_id, tunell);
                if (!b)
                    return lst;
            }
            return lst;
        }
    }
}
