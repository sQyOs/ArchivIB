using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;
using System.Windows.Controls.Primitives;

namespace ArchivIB
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OleDbConnection cn = new OleDbConnection();
        OleDbCommand cmd = new OleDbCommand();
        OleDbDataReader dr;
        List<String> dataDiagList = new List<String>();
        public MainWindow()
        {
            InitializeComponent();
            cn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Program Files\ArchivIB\archiv.accdb;Persist Security Info=False;";
            cmd.Connection = cn;
            this.DataContext = dataDiagList;
            tb_diag.Loaded += delegate
            {
                TextBox textBox = tb_diag.Template.FindName("PART_EditableTextBox", tb_diag) as TextBox;
                Popup popup = tb_diag.Template.FindName("PART_Popup", tb_diag) as Popup;
                if (textBox != null)
                {
                    textBox.TextChanged += delegate
                    {
                        popup.IsOpen = true;
                        tb_diag.Items.Filter += a =>
                        {
                            if (a.ToString().StartsWith(textBox.Text))
                            {
                                return true;
                            }
                            return false;
                        };
                    };
                }
            };
        }

        private void loadDiagData()
        {
            try
            {
                cn.Open();
                cmd.CommandText = "select distinct Диагноз from ФИО";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    dataDiagList.Add(dr[0].ToString());
                }
                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("" + ex);
            }
        }

        int lastList()
        {
            int maxList = 1;
            cmd.CommandText = @"select count(Код) from ФИО";
            try
            {
                cn.Open();
                maxList = (int)cmd.ExecuteScalar() / Convert.ToUInt16(tbCountRowShow.Text) + 1;
                cn.Close();
                if (maxList < 1)
                    maxList = 1;
                }
            catch
            {
                cn.Close();
                maxList = 1;
            }
            return maxList;
        }

        //datenull
        private void ifDateNull(DatePicker _dataPic)
        {
            if (_dataPic.SelectedDate == null)
            {
                MessageBox.Show("yeah");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbNumberList.Text = lastList().ToString();
            loadDataGrid();
            loadDiagData();
            //for (Int32 i = 0; i < 100; i++)
            //{
            //    dataDiagList.Add(i.ToString() + "Item");
            //    dataDiagList.Add(i.ToString() + "Item");
            //    dataDiagList.Add(i.ToString() + "Item");
            //    dataDiagList.Add(i.ToString() + "Item");
            //    dataDiagList.Add(i.ToString() + "Item");
            //}
        }


        //функция загрузки данных в дата грид
        private void loadDataGrid()
        {
            try
            {
                int x = (Convert.ToUInt16(tbNumberList.Text) - 1) * Convert.ToUInt16(tbCountRowShow.Text);
                if (x < 1)
                    cmd.CommandText = @"select top " + tbCountRowShow.Text + " Код, Отделение, ФИО, [Номер ИБ], Диагноз, [Дата поступления], [Дата выписки], Умер from ФИО ORDER BY Код ASC";
                else
                    cmd.CommandText = @"select top " + tbCountRowShow.Text + " Код, Отделение, ФИО, [Номер ИБ], Диагноз, [Дата поступления], [Дата выписки], Умер from ФИО where Код > " + x + " ORDER BY Код ASC";
                cn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                cn.Close();
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("Error" + ex);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string dateIn;
            string dateOut;
            if (dp_DateIn?.SelectedDate == null)
                dateIn = "null";
            else
                dateIn = "'" + dp_DateIn.SelectedDate.ToString() + "'";

            if (dp_DateOut?.SelectedDate == null)
                dateOut = "null";
            else
                dateOut = "'" + dp_DateOut.SelectedDate.ToString() + "'";
            cmd.CommandText = @"insert into ФИО (Отделение, ФИО, [Номер ИБ], Диагноз, [Дата поступления], [Дата выписки]) values('" + tb_depart.Text + "', '" + tb_fio.Text + "', '" + tb_ib.Text + "', '" + tb_diag.Text + "', " + dateIn +", "+ dateOut+")";
            try
            {
                cn.Open();
                //MessageBox.Show(cmd.CommandText);
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("Error" + ex);
            }
            loadDataGrid();
        }


        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                tB_ID.Text = row[0].ToString();
                tb_depart.Text = row[1].ToString();
                tb_fio.Text = row[2].ToString();
                tb_ib.Text = row[3].ToString();
                tb_diag.Text = row[4].ToString();
                dp_DateIn.Text = row[5].ToString();
                dp_DateOut.Text = row[6].ToString();
                cb_IsDeath.IsChecked = false;
                TimeSpan? _kd = (dp_DateOut.SelectedDate - dp_DateIn.SelectedDate);
                tb_kd.Text =  _kd?.Days.ToString();
            }
            catch (Exception ex)
            {
                //cn.Close();
                //MessageBox.Show("Error" + ex);
                clear();
            }
        }
        private void clear()
        {
            cmd.CommandText = "select top 1 Отделение from ФИО order by Код DESC";
            cn.Open();
            string s = (string)cmd.ExecuteScalar();
            cn.Close();
            tB_ID.Text = "";
            tb_depart.Text = s;
            tb_fio.Clear();
            tb_ib.Clear();
            tb_diag.Text = "";
            dp_DateIn.Text = "";
            dp_DateOut.Text = "";
            tb_kd.Text = null;
            cb_IsDeath.IsChecked = false;
        }

        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
            }
        }

        private void btn_next_page_Click(object sender, RoutedEventArgs e)
        {
            tbNumberList.Text = (Convert.ToUInt16(tbNumberList.Text) + 1).ToString();
            loadDataGrid();
        }

        private void btn_last_page_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToUInt16(tbNumberList.Text) > 1)
            {
                tbNumberList.Text = (Convert.ToUInt16(tbNumberList.Text) - 1).ToString();
                loadDataGrid();
            }
            
        }

        private void tb_depart_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
