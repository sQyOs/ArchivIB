using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace testDB
{
    public partial class Form1 : Form
    {
        OleDbConnection cn = new OleDbConnection();
        public Form1()
        {
            InitializeComponent();
            cn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\archiv\Csharp\archiv.accdb";

        }
        OleDbCommand cmd = new OleDbCommand();
        OleDbDataReader dr;
        bool bac = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "testDataSet.table1". При необходимости она может быть перемещена или удалена.
            this.table1TableAdapter.Fill(this.testDataSet.table1);
            dataGridView1.ClearSelection();
            tbList.Text = "1";
            tbCoRec.Text = "5";
            cmd.Connection = cn;
            fill_combo(comboBox1, "user", "table1");
            loadata();
            
            {
                //sqlConnTestDB = new SqlConnection(cStr);

                //SqlDataReader sqlReader = null;
                //SqlCommand command = new SqlCommand("SELECT * FROM [table1]", sqlConnTestDB);
                //try
                //{
                //    sqlReader = command.ExecuteReader();
                //    while (sqlReader.Read())
                //    {
                //        listBox1.Items.Add(Convert.ToString(sqlReader["id"]) + " " + Convert.ToString(sqlReader["user"]) + " " + Convert.ToString(sqlReader["log"]) + " " + Convert.ToString(sqlReader["pas"]) + " " + Convert.ToString(sqlReader["date_b"]));
                //    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
                //finally
                //{
                //    if (sqlReader != null)
                //        sqlReader.Close();
                //}


                //var ds = new DataSet();
                //var adapter = new OleDbDataAdapter("SELECT * FROM [table1]", cn);
                //cn.Open();
                //adapter.Fill(ds);
                //cn.Close();
                //var dataS = this.testDataSet.table1.Rows[0]["log"].ToString();
                //listBox1.DataSource = this.testDataSet.table1;
            }
        }



        //заполнение комбо бокса уникальными значениями поля таблицы (имя комбо бокса, имя поля таблицы, имя таблицы)
        private void fill_combo(ComboBox e, string pole, String table) 
        {
            e.Items.Clear();
            try
            {
                cn.Open();
                cmd.CommandText = "select distinct " + pole + " from " + table;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    e.Items.Add(dr[pole]);
                }
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex);
            }
        }

        //ввод на лету в комбобоксе звать в апдейте бокса, 
        //публичную булеву переменную менять в отлове нажатия бэкспейса в томже боксе
        //(имя комбо бокса, булева переменная, после какого символа начинать искать)
        private void fly_type(ComboBox cb, bool check, int startFind) 
        {
            int notIn = cb.FindString(comboBox1.Text);
            if (cb.Text.Length > startFind && notIn > 0 && !check)
            {
                int i = cb.Text.Length;
                cb.SelectedIndex = cb.FindString(cb.Text);
                cb.SelectionStart = i;
                cb.SelectionLength = cb.Text.Length;
            }
            else
            {
                if (check)
                    check = false;
            }
        }



        private void loadata()
        {
            try
            {
                listBox1.Items.Clear();
                //cmd.CommandText = "select * from (select ID.rnum as rnum, ID.* from table1 ID) where rnum > 3 and rnum <= 7";
                int x = (Convert.ToUInt16(tbList.Text) - 1) * Convert.ToUInt16(tbCoRec.Text);
                if (x < 1)
                {
                    cmd.CommandText = "SELECT TOP " + tbCoRec.Text + " * FROM [table1] ORDER BY id DESC";
                }
                else
                {
                    cmd.CommandText = "SELECT TOP " + tbCoRec.Text + " * FROM [table1] WHERE id NOT IN (SELECT TOP "+x+" id FROM [table1] ORDER BY id DESC) ORDER BY id DESC";
                }
                cn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                BindingSource ds = new BindingSource();
                da.Fill(dt);
                ds.DataSource = dt;
                dataGridView1.DataSource = dt;
                bindingNavigator1.BindingSource = ds;
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        listBox1.Items.Add(Convert.ToString(dr[0]) + "\t" + Convert.ToString(dr[1]) + "\t" + Convert.ToString(dr[2]) + "\t" + Convert.ToString(dr[3]) + "\t" + Convert.ToString(dr[4])?.Substring(0,10));
                    }
                }
                dr.Close();
                cn.Close();
            }
            catch (Exception e)
            {
                cn.Close();
                MessageBox.Show(e.Message.ToString());
            }
        }

        private void saveRecord_Click(object sender, EventArgs e)
        {
            if (tB_date.Text == "  .  .")
            {
                this.table1TableAdapter.Insert(comboBox1.Text, tB_log.Text, tB_pas.Text, null);
            }
            else
            {
                this.table1TableAdapter.Insert(comboBox1.Text, tB_log.Text, tB_pas.Text, DateTime.Parse(tB_date.Text));
            }
            this.table1TableAdapter.Update(this.testDataSet.table1);
            this.table1TableAdapter.Fill(this.testDataSet.table1);
            int notIn = comboBox1.FindString(comboBox1.Text);
            if (notIn < 1)
            {
                fill_combo(comboBox1, "user", "table1");
            }
            loadata();
            comboBox1.Text = "";
            //tB_user.Clear();
            tB_log.Clear();
            tB_pas.Clear();
            tB_date.Clear();
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.table1TableAdapter.Update(this.testDataSet.table1);
            this.table1TableAdapter.Fill(this.testDataSet.table1);
            loadata();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //tB_user.Focus();
            comboBox1.Focus();
        }


        private void saveRecord_MouseClick(object sender, MouseEventArgs e)
        {
            //tB_user.Focus();
            comboBox1.Focus();
        }


        private void bListPlus_Click(object sender, EventArgs e)
        {
            tbList.Text = Convert.ToString(Convert.ToUInt16(tbList.Text) + 1);
            loadata();
        }

        private void bListMinus_Click(object sender, EventArgs e)
        {
            if (Convert.ToUInt16(tbList.Text) > 1)
            {
                tbList.Text = Convert.ToString(Convert.ToUInt16(tbList.Text) - 1);
                loadata();
            }
        }

        private void tbList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar>=31 && e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar!=127)
            {
                e.Handled = true; 
            }
        }

        private void tbCoRec_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 31 && e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 127)
            {
                e.Handled = true;
            }
        }

        private void bNew_KeyPress(object sender, KeyPressEventArgs e)
        {
            //dataGridView1.Rows[dataGridView1.NewRowIndex].
            comboBox1.Text = "";
            //tB_user.Clear();
            tB_log.Clear();
            tB_pas.Clear();
            tB_date.Clear();
            comboBox1.Focus();
            //tB_user.Focus();
            //this.table1TableAdapter.Update(this.testDataSet.table1);
            //this.table1TableAdapter.Fill(this.testDataSet.table1);
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            fly_type(comboBox1, bac, 1);
            //int notIn = comboBox1.FindString(comboBox1.Text);
            //if (comboBox1.Text.Length > 2 && notIn > 0 && !bac)
            //{
            //    int i = comboBox1.Text.Length;
            //    comboBox1.SelectedIndex = comboBox1.FindString(comboBox1.Text);
            //    comboBox1.SelectionStart = i;
            //    comboBox1.SelectionLength = comboBox1.Text.Length;
            //}
            //else
            //{
            //    if (bac)
            //        bac=false;
            //}
        }
        
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //e.Handled = false;
            if (e.KeyChar == 8)
                bac = true;
            if (e.KeyChar == (char)Keys.Enter)
                tB_log.Focus();
            //int notIn = comboBox1.FindString(comboBox1.Text);
            //if (comboBox1.Text.Length > 2 && notIn > 0 && e.KeyChar!=8)
            //{
            //    int i = comboBox1.Text.Length;
            //    comboBox1.SelectedIndex = comboBox1.FindString(comboBox1.Text);
            //    comboBox1.SelectionStart = i;
            //    comboBox1.SelectionLength = comboBox1.Text.Length;
            //}
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                comboBox1.Text = row.Cells[1].Value.ToString();
                tB_log.Text = row.Cells[2].Value.ToString();
                tB_pas.Text = row.Cells[3].Value.ToString();
                tB_date.Text = row.Cells[4].Value.ToString();
            }
        }

        private void bNew_Click(object sender, EventArgs e)
        {
            comboBox1.Text = "";
            //tB_user.Clear();
            tB_log.Clear();
            tB_pas.Clear();
            tB_date.Clear();
            comboBox1.Focus();
            //tB_user.Focus();
        }

        private void tB_log_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                tB_pas.Focus();
        }

        private void tB_pas_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                tB_date.Focus();
        }

        private void tB_date_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                saveRecord.Focus();
        }

        private void saveRecord_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                comboBox1.Focus();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}

