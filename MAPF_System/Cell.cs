using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Security;
using System.Windows.Forms;

namespace MAPF_System
{
    public class Cell
    {
        private bool isBlock;
        private bool wasvisited;
        private int idVisit;
        private bool isBad;
        private Tunell tunell;

        public Cell(bool isBlock, bool wasvisited = false, int idVisit = -1, bool isBad = false)
        {
            this.isBlock = isBlock;
            this.wasvisited = wasvisited;
            this.idVisit = idVisit;
            this.isBad = isBad;
        }
        public Cell(string str)
        {
            string[] arr = str.Split(' ');
            // Задание параметров клетки на основе строки из файла
            isBlock = arr[0] == "True";
            wasvisited = arr[1] == "True";
            idVisit = int.Parse(arr[2]);
            isBad = arr[3] == "True";
        }
        public Cell CopyWithoutBlock() { return new Cell(false, wasvisited, idVisit, isBad); }
        public int IdVisit() { return idVisit; }
        public string ToStr() { return isBlock + " " + wasvisited + " " + idVisit + " " + isBad; }
        public Tunell Tunell() { return tunell; }
        public Tunell MakeTunell(Tunell tunell) { return this.tunell = tunell; }
        public void ClearTunell() { tunell = null; }
        public void MakeVisit(int n)
        {
            wasvisited = true;
            idVisit = n;
        }
        public void MakeBad() { isBad = true; }
        public void MakeBlock() { isBlock = true; }
        public bool IsBad() { return isBad; }
        public bool IsBlock() { return isBlock; }
        public bool IsTunell() { return !(tunell is null); }
        public bool WasVisit() { return wasvisited; }
        public void DelBlokcs() { isBlock = false; }
        public int ReversBlock() 
        { 
            if (isBlock = !isBlock)
                return 1;
            return 2;
        }
    }
}
