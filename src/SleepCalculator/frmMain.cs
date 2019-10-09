using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SleepCalculator
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            Cycle.SelectedIndex = 2;
            TimeInSleep.Value = 14;
            int h = DateTime.Now.TimeOfDay.Hours;
            int m = DateTime.Now.TimeOfDay.Minutes;
            Hours.Value = h;
            Minutes.Value = m;
        }
        #region
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        #endregion

        List<Sleep> lst;
        public void Hienthi()
        {
            DataTable dt = ToDataTable<Sleep>(lst);
            dtView.DataSource = dt;
            dtView.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dt.Columns["STT"].ColumnName = "STT";
            dt.Columns["TimeStart"].ColumnName = "Bắt đầu";
            dt.Columns["Cycle"].ColumnName = "Chu kỳ";
            dt.Columns["TimeGetUp"].ColumnName = "Thức dậy";

            int i = 0;
            foreach (DataGridViewColumn col in dtView.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                dtView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                i++;
            }
            dtView.Refresh();
        }

        private void getTimeNow_Click(object sender, EventArgs e)
        {
            int h = DateTime.Now.TimeOfDay.Hours;
            int m = DateTime.Now.TimeOfDay.Minutes;
            Hours.Value = h;
            Minutes.Value = m;
        }

        private void Calculator_Click(object sender, EventArgs e)
        {
            lst = new List<Sleep>();
            int h = decimal.ToInt32(Hours.Value);
            int m = decimal.ToInt32(Minutes.Value);
            int cycle = int.Parse(Cycle.GetItemText(this.Cycle.SelectedItem));
            double timeCycle = 1.5 * cycle * 60;
            int timetosleep = decimal.ToInt32(TimeInSleep.Value);
            int getup = h * 60 + (int)timeCycle + m + timetosleep;
            if (getup > 24 * 60)
            {
                getup = h * 60 + (int)timeCycle + m + timetosleep - 24 * 60;
            }
            string s = string.Format("Bạn nên thức dậy lúc {0:00}:{1:00}", getup / 60, getup % 60);
            switch(cycle)
            {
                case 3:
                    txtKetQua.ForeColor = Color.Red;
                    break;
                case 4:
                    txtKetQua.ForeColor = Color.Orange;
                    break;
                case 5:
                    txtKetQua.ForeColor = Color.Green;
                    break;
                case 6:
                    txtKetQua.ForeColor = Color.LawnGreen;
                    break;
            }
            txtKetQua.Text = s;
            int c = 3;
            for (int i = 0; i < 4; i++)
            {
                double cyc = 1.5 * c * 60;
                int getuplst = h * 60 + (int)cyc + m + timetosleep;
                if (getuplst > 24 * 60)
                {
                    getuplst = h * 60 + (int)cyc + m + timetosleep - 24 * 60;
                }
                lst.Add(new Sleep
                {
                    STT = i + 1,
                    TimeStart = string.Format("{0:00}:{1:00}", h, m),
                    Cycle = c.ToString(),
                    TimeGetUp = string.Format("{0:00}:{1:00}", getuplst / 60, getuplst % 60)
                });
                c++;
            }
            Hienthi();
        }

        private void Cycle_KeyPress(object sender, KeyPressEventArgs e)
        {

            e.Handled = true;

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://"+linkLabel1.Text);
        }
    }
}
