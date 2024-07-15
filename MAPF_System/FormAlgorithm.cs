using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAPF_System
{
    public partial class FormAlgorithm : Form
    {
        private Board Board;
        private bool was_game;
        private bool move;
        private Tuple<int, int> C;
        private Tuple<int, int> CC;

        public FormAlgorithm(Board Board, int kol_iterat = 0, bool error = false, string str_kol_iter_a_star = "", bool block_elem = false, bool viewtunnel = true)
        {
            this.Board = Board;
            was_game = Board.GetWasGame();
            InitializeComponent();
            textBox_kol_iter_a_star.Text = str_kol_iter_a_star;
            label6.Text = Board.Name();
            // Позиция данной формы
            StartPosition = FormStartPosition.Manual;
            Location = new Point(100, 100);
            // Отрисовка поля
            Paint += delegate { Board.Draw(CreateGraphics(), true, null, null, viewtunnel); };
            if (kol_iterat != 0)
                label_kol_iterat.Text = "Количество шагов = " + kol_iterat;
            if (error)
                label_Error.Text = "Ошибка! Алгоритм зациклен";
            if (block_elem || was_game)
            {
                was_game = true;
                button_Start.Dispose();
                Controls.Remove(button_Start);
                button_Step.Dispose();
                Controls.Remove(button_Step);
                ButtonPlusUnit.Dispose();
                Controls.Remove(ButtonPlusUnit);
                ButtonMinusUnit.Dispose();
                Controls.Remove(ButtonMinusUnit);
                ButtonPlusRow.Dispose();
                Controls.Remove(ButtonPlusRow);
                ButtonPlusColumn.Dispose();
                Controls.Remove(ButtonPlusColumn);
                ButtonDelBlock.Dispose();
                Controls.Remove(ButtonDelBlock);
                ButtonDelUnits.Dispose();
                Controls.Remove(ButtonDelUnits);
                label4.Text = "";
                label7.Text = "";
                label8.Text = "";
                label10.Text = "";
                label11.Text = "";
                label12.Text = "";
                label13.Text = "";
                label14.Text = "";
                label15.Text = "";
                label16.Text = "";
                label17.Text = "";
                label21.Text = "";
                label22.Text = "";
            }
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox_kol_iter_a_star.Text, out int kol_iter_a_star) || (kol_iter_a_star < 7) || (kol_iter_a_star > 20))
            {
                SystemSounds.Beep.Play();
                label_Error.Text = "Глубина не верна!";
                return;
            }
            // Максимальное колличество итераций
            int N = 5000;
            Board TimeBoard = Board.CopyWithoutBlocks();
            int i = 0;
            while (!TimeBoard.IsEnd() && (i++) < (N-1))
                TimeBoard.MakeStep(Board, kol_iter_a_star);
            (new FormAlgorithm(TimeBoard, i, i == N, "" + kol_iter_a_star, true)).Show();
        }

        private void button_Step_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox_kol_iter_a_star.Text, out int kol_iter_a_star) || (kol_iter_a_star < 7) || (kol_iter_a_star > 20))
            {
                SystemSounds.Beep.Play();
                label_Error.Text = "Глубина не верна!";
                return;
            }
            Board TimeBoard = Board.CopyWithoutBlocks();
            int i = 1;
            FormAlgorithm F = new FormAlgorithm(TimeBoard, 0, false, "" + kol_iter_a_star, true);
            F.Show();
            while (!TimeBoard.IsEnd()) 
            {
                TimeBoard.MakeStep(Board, kol_iter_a_star);
                TimeBoard.Draw(F.CreateGraphics(), false);
                F.label_kol_iterat.Text = "Количество шагов = " + i++;
                if (MessageBox.Show("Далее?", "▶▶", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    F.Close();
                    return;
                }
            }
        }

        private void FormAlgorithm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
            if (e.Control && e.KeyCode == Keys.S)
                button_Save_Click(sender, null);
            if (!was_game && e.KeyCode == Keys.F5)
                button_Start_Click(sender, null);
            if (!was_game && e.KeyCode == Keys.F10)
                button_Step_Click(sender, null);
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            move = false;
            if (textBox_Name.Text.Length == 0)
            {
                SystemSounds.Beep.Play();
                label_Error.Text = "Вы не ввели имя файла!";
                return;
            }
            label6.Text = Board.Save(textBox_Name.Text, was_game);
            label_Error.Text = "Сохранено!";
        }

        private Tuple<int, int> CELL(MouseEventArgs e)
        {
            int height = 18;
            if (Math.Max(Board.GET_X(), Board.GET_Y()) < 30)
                height = 24;
            return new Tuple<int, int>((e.Location.X - 100) / height, (e.Location.Y - 120) / height);
        }
        
        private void FormAlgorithm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (was_game)
                return;
            move = false;
            var C = CELL(e);
            int r = Board.ReversBlock(C);
            if (r == 1)
                Board.Draw(CreateGraphics(), false);
            if (r == 2)
                Board.Draw(CreateGraphics(), false, C);
        }

        private void FormAlgorithm_MouseClick(object sender, MouseEventArgs e)
        {
            if (was_game)
                return;
            var C1 = CELL(e);
            if (move)
                if(Board.Move(C, C1))
                    Board.Draw(CreateGraphics(), false, C);
            C = C1;
            move = !move;
        }

        private void FormAlgorithm_MouseMove(object sender, MouseEventArgs e)
        {
            using (Graphics g = CreateGraphics())
            {
                var C = CELL(e);
                int height = 18;
                if (Math.Max(Board.GET_X(), Board.GET_Y()) < 30)
                    height = 24;
                var Font = new Font("Arial", 7, FontStyle.Bold);
                var Font1 = new Font("Arial", 7, FontStyle.Bold | FontStyle.Underline);
                if (!(CC is null) && (CC.Item1 < Board.GET_X()) && (CC.Item2 < Board.GET_Y()))
                {
                    g.DrawString("" + CC.Item2, Font1, Brushes.White, new Point(88, 124 + height * CC.Item2));
                    g.DrawString("" + CC.Item1, Font1, Brushes.White, new Point(104 + height * CC.Item1, 108));
                    g.DrawString("" + CC.Item2, Font, Brushes.Coral, new Point(88, 124 + height * CC.Item2));
                    g.DrawString("" + CC.Item1, Font, Brushes.Coral, new Point(104 + height * CC.Item1, 108));
                }
                if ((C.Item1 < Board.GET_X()) && (C.Item2 < Board.GET_Y()) && (e.Location.Y > 115) && (e.Location.X > 95))
                {
                    if (!was_game)
                        Cursor = Cursors.Hand;
                    g.DrawString("" + C.Item2, Font1, Brushes.Blue, new Point(88, 124 + height * C.Item2));
                    g.DrawString("" + C.Item1, Font1, Brushes.Blue, new Point(104 + height * C.Item1, 108));
                }
                else
                {
                    if (!was_game)
                        Cursor = Cursors.Default;
                }
                CC = C;
            }
        }

        private void ButtonPlusUnit_Click(object sender, EventArgs e)
        {
            move = false;
            Board.PlusUnit();
            Board.Draw(CreateGraphics(), false);
        }

        private void ButtonMinusUnit_Click(object sender, EventArgs e)
        {
            move = false;
            var U = Board.MinusUnit();
            if (!(U is null))
                Board.Draw(CreateGraphics(), false, U.Item1, U.Item2);
        }

        private void ButtonPlusRow_Click(object sender, EventArgs e)
        {
            move = false;
            Board.PlusRow();
            Board.Draw(CreateGraphics());
        }

        private void ButtonPlusColumn_Click(object sender, EventArgs e)
        {
            move = false;
            Board.PlusColumn();
            Board.Draw(CreateGraphics());
        }

        private void ButtonDelBlock_Click(object sender, EventArgs e)
        {
            move = false;
            Board.DelBlokcs();
            Board.Draw(CreateGraphics());
        }

        private void ButtonDelUnits_Click(object sender, EventArgs e)
        {
            move = false;
            if(Board.DelUnits())
                Board.Draw(CreateGraphics());
        }

        private void ButtonCopy_Click(object sender, EventArgs e)
        {
            move = false;
            int height = 18;
            if (Math.Max(Board.GET_X(), Board.GET_Y()) < 30)
                height = 24;
            using (Bitmap screenshot = new Bitmap(height * Board.GET_X() + 15, height * Board.GET_Y() + 15))
            {
                using (Graphics graphics = Graphics.FromImage(screenshot))
                {
                    graphics.CopyFromScreen(PointToScreen(new Point(88, 108)), Point.Empty, Bounds.Size);
                }
                Clipboard.SetImage(screenshot);
            }
            label20.Text = "Скопировано!";
        }

    }
}
