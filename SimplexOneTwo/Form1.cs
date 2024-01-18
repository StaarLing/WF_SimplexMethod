using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimplexOneTwo
{
    public partial class Form1 : Form
    {
        TableLayoutPanel tableLayoutPanel1 = new TableLayoutPanel();
        TableLayoutPanel tableLayoutPanel2 = new TableLayoutPanel();
        TableLayoutPanel tableLayoutPanel3 = new TableLayoutPanel();
        public Form1()
        {
            InitializeComponent();
            label5.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                label5.Show();

                tableLayoutPanel2.ColumnCount = Convert.ToInt32(textBox1.Text);
                tableLayoutPanel2.Location = new Point(20, 20);
                tableLayoutPanel2.RowCount = 1;
                tableLayoutPanel2.Size = new Size(400, 25);
                this.Controls.Add(tableLayoutPanel2);

                for (int i = tableLayoutPanel2.RowCount * tableLayoutPanel2.ColumnCount; i > 0; i--)
                    tableLayoutPanel2.Controls.Add(new System.Windows.Forms.TextBox()
                    {
                        Size = new Size(50, 200),
                        Text = "0"
                    });

                tableLayoutPanel1.ColumnCount = Convert.ToInt32(textBox1.Text); //В 1ом текстбоксе количество столбцов
                tableLayoutPanel1.Location = new Point(20, 50); //Координаты подгоните куда нужно
                tableLayoutPanel1.RowCount = Convert.ToInt32(textBox2.Text); //Во 2ом текстбоксе количество строк
                tableLayoutPanel1.Size = new Size(400, 1000); //Размер незнаю как расчитать, поэтому ставлю стразу большой
                this.Controls.Add(tableLayoutPanel1);

                for (int i = tableLayoutPanel1.RowCount * tableLayoutPanel1.ColumnCount; i > 0; i--) //Сколько в таблице ячеек
                {
                    tableLayoutPanel1.Controls.Add(new System.Windows.Forms.TextBox()
                    {
                        Size = new Size(50, 200),
                        Text = "0"
                    });

                }

                tableLayoutPanel3.ColumnCount = 1; //В 1ом текстбоксе количество столбцов
                tableLayoutPanel3.Location = new Point(420, 50); //Координаты подгоните куда нужно
                tableLayoutPanel3.RowCount = Convert.ToInt32(textBox2.Text); //Во 2ом текстбоксе количество строк
                tableLayoutPanel3.Size = new Size(55, 400); //Размер незнаю как расчитать, поэтому ставлю стразу большой
                this.Controls.Add(tableLayoutPanel3);
                for (int i = 0; i < Convert.ToInt16(textBox2.Text); i++)
                     tableLayoutPanel3.Controls.Add( new ComboBox()
                    {
                        Size = new Size(50, 200),
                        Text = "<=/=/>=",
                        Items= {">=","=","<="},
                    });
            }
            else
            {
                MessageBox.Show("Заполните пустые поля!", "Ошибка!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var n = Convert.ToInt16(textBox1.Text);
            var m = Convert.ToInt16(textBox2.Text);
            double[,] table = new double[m, n];
            List<double> F = new List<double>();
            List<string> eq = new List<string>();
            for(int i=0;i<n;i++)
            {
                F.Add(Convert.ToDouble(tableLayoutPanel2.Controls[i].Text));
            }

            int col = 0;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    table[i, j] = Convert.ToDouble(tableLayoutPanel1.Controls[col].Text);
                    col++;
                }
            }
            for (int i = 0; i < m; i++)
            {
                eq.Add(tableLayoutPanel3.Controls[i].Text);
            }

            double[] result = new double[n - 1];
            double[,] table_result;
            Simplex S = new Simplex(n, m, comboBox1.Text, table,eq,F);
            textBox3.Text = S.GetSolveString();
            textBox4.Text = S.GetAnsString();
        }
    }
}
