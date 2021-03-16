using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private DataSet dataSet = null;
        private bool newRowAdding = false;

        //ClassForThread forThread = new ClassForThread();
        //Thread myThread = new Thread(new ParameterizedThreadStart(CheckConnect2));


        public static void CheckConnect2(object obj)
        {
            ClassForThread  c = (ClassForThread)obj;
            
            
            Thread.Sleep(c.c*1000);
            string s = c.c.ToString();
            //CheckConnect1(c.a, c.b, s);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();

            LoadData();
            CheckConnect();

        }

        private void LoadData()
        {
            try
            {
                sqlDataAdapter = new SqlDataAdapter("SELECT*, 'Delete' AS [DELETE] FROM Site", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlDataAdapter);

                sqlBuilder.GetInsertCommand();
                sqlBuilder.GetUpdateCommand();
                sqlBuilder.GetDeleteCommand();

                dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, "Site");
                dataGridView1.DataSource = dataSet.Tables["Site"];

                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[5,i] = linkCell;
                }
            }

            catch(Exception ex)
            {
                MessageBox.Show("Error");
            }

            
        }

        private void ReloadData()
        {
            try
            {
                dataSet.Tables["Site"].Clear();
                sqlDataAdapter.Fill(dataSet, "Site");
                dataGridView1.DataSource = dataSet.Tables["Site"];

                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[0, i] = linkCell;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error");
            }
            CheckConnect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();

                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;
                            dataGridView1.Rows.RemoveAt(rowIndex);
                            dataSet.Tables["Site"].Rows[rowIndex].Delete();
                            sqlDataAdapter.Update(dataSet, "Site");
                        }
                    }
                    else if (task == "Insert")
                    {
                        int rowIndex = dataGridView1.Rows.Count - 2;
                        DataRow row = dataSet.Tables["Site"].NewRow();

                        row["NameSite"] = dataGridView1.Rows[rowIndex].Cells["NameSite"].Value;
                        row["UrlSite"] = dataGridView1.Rows[rowIndex].Cells["UrlSite"].Value;
                        row["IntervalCheck"] = dataGridView1.Rows[rowIndex].Cells["IntervalCheck"].Value;

                        dataSet.Tables["Site"].Rows.Add(row);
                        dataSet.Tables["Site"].Rows.RemoveAt(dataSet.Tables["Site"].Rows.Count-1);
                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count-2);
                        dataGridView1.Rows[e.RowIndex].Cells[5].Value = "Delete";

                        sqlDataAdapter.Update(dataSet, "Site");
                        newRowAdding = false;
                    }
                    else if (task == "Update")

                    {
                        int r = e.RowIndex;

                        dataSet.Tables["Site"].Rows[r]["NameSite"] = dataGridView1.Rows[r].Cells["NameSite"].Value;
                        dataSet.Tables["Site"].Rows[r]["UrlSite"] = dataGridView1.Rows[r].Cells["UrlSite"].Value;
                        dataSet.Tables["Site"].Rows[r]["IntervalCheck"] = dataGridView1.Rows[r].Cells["IntervalCheck"].Value;

                        sqlDataAdapter.Update(dataSet, "Site");

                        dataGridView1.Rows[e.RowIndex].Cells[5].Value = "Delete";
                    }

                    ReloadData();

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error");
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if(newRowAdding == false)
                {
                    newRowAdding = true;
                    int lastRow = dataGridView1.Rows.Count - 2;
                    DataGridViewRow row = dataGridView1.Rows[lastRow];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[5, lastRow] = linkCell;
                    row.Cells["Delete"].Value = "Insert";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error");
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(newRowAdding == false)
                {
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                    DataGridViewRow editingRow = dataGridView1.Rows[rowIndex];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[5, rowIndex] = linkCell;
                    editingRow.Cells["Delete"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error");
            }
        }

        private void CheckConnect()
        {
            try
            {
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    string a = dataGridView1[2, i].Value.ToString();
                    string t = dataGridView1[3, i].Value.ToString();
                    
                    CheckConnect1(a,i,t);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("На некоторых сайтах подключение не установлено");
                
            }
        }

        private void CheckConnect1(string a, int i, string t)
        {
            int tim = int.Parse(t);
            if (CheckNet.isOnline(a))
            {

                dataGridView1[4, i].Value = "is true";

            }
            else
            {
                dataGridView1[4, i].Value = "is false";
            }

            //forThread.a = a;
            //forThread.b = i;
            //forThread.c = tim;

           // myThread.Start(forThread);
        }

        

    }
}
