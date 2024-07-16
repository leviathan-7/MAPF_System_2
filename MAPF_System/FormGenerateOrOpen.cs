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
using System.IO;

namespace MAPF_System
{
    public partial class FormGenerateOrOpen : Form
    {
        public FormGenerateOrOpen(string[] args) 
        { 
            InitializeComponent();
            if (args.Length != 0)
                (new FormAlgorithm(new Board(args[0]), 0, false, "7", false, false)).ShowDialog();
        }
        
        private void button_Generation_Click(object sender, EventArgs e)
        {
            label_Error.Text = "";
            // Считывание введенных данных
            int X = 0, Y = 0, Blocks = 0, Units = 0;
            bool isNumeric = int.TryParse(textBox_X.Text, out X) && int.TryParse(textBox_Y.Text, out Y)
                && int.TryParse(textBox_Blocks.Text, out Blocks) && int.TryParse(textBox_Units.Text, out Units);

            // Проверка введенных данных на правильность
            if (!isNumeric)
            {
                SystemSounds.Beep.Play();
                label_Error.Text = "Вы ввели не число!";
                return;
            }
            if ((X > 45) || (Y > 45))
            {
                SystemSounds.Beep.Play();
                label_Error.Text = "Размер поля превышает пределы!";
                return;
            }
            if ((X < 2) || (Y < 2))
            {
                SystemSounds.Beep.Play();
                label_Error.Text = "Поле слишком маленькое!";
                return;
            }
            if (Blocks < 0)
            {
                SystemSounds.Beep.Play();
                label_Error.Text = "Не должно быть отрицательных чисел!";
                return;
            }
            if (Units < 1)
            {
                SystemSounds.Beep.Play();
                label_Error.Text = "Должен быть хоть один юнит!";
                return;
            }
            if ((Blocks + 2 * Units) >= (X * Y))
            {
                SystemSounds.Beep.Play();
                label_Error.Text = "Количество препятствий и юнитов слишком большое!";
                return;
            }

            // Если все данные введены правильно
            (new FormAlgorithm(new Board(X, Y, Blocks, Units), 0, false, "7")).Show();
        }
        
        private void button_Load_Click(object sender, EventArgs e) 
        {
            label_Error.Text = "";
            label12.Text = "⏳";
            Board Board = new Board();
            if(!(Board.Units() is null))
                (new FormAlgorithm(Board, 0, false, "7", false, false)).Show();
            label12.Text = "";
        }
        
        private void button_BigStart_Click(object sender, EventArgs e)
        {
            label_Error.Text = "";
            label11.Text = "⏳";
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    DataTable table = new DataTable("results");
                    table.Columns.Add("Имя файла", typeof(string));
                    table.Columns.Add("Колличество шагов", typeof(string));
                    int a = 0, b = 0;
                    foreach (var f in (from f in Directory.GetFiles(fbd.SelectedPath) where Path.GetExtension(f).ToLower() == ".board" select f))
                    {
                        Board Board = new Board(f);
                        int kol_iter_a_star = 7;
                        // Максимальное колличество итераций
                        int N = 5000;
                        Board TimeBoard = Board.CopyWithoutBlocks();
                        int i = 0;
                        while (!TimeBoard.IsEnd() && (i++) < (N - 1))
                            TimeBoard.MakeStep(Board);
                        if (i == N)
                        {
                            table.Rows.Add(f.Split('\\').Last(), "Ошибка");
                            a++;
                        }
                        else
                        {
                            table.Rows.Add(f.Split('\\').Last(), "" + i);
                            b++;
                        }
                    }
                    table.WriteXml(fbd.SelectedPath + "\\results.xml");
                    label11.Text = "";
                    if(MessageBox.Show("Пройденно "+b+" из "+(a+b)+ "\nРезультаты сохранены в файл results.xml\nОткрыть файл?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
                        System.Diagnostics.Process.Start("results.xml");
                }
                label11.Text = "";
            }
        }

        private void FormGenerateOrOpen_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            (new FormAbout()).Show();
            e.Cancel = true;
        }

        private void FormGenerateOrOpen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
                (new FormAbout()).Show();
        }
    }
}
