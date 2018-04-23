using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraEditors;

namespace TestLookUpIncrementalSearch {
    public partial class Form1 : DevExpress.XtraEditors.XtraForm {
        public Form1() {
            InitializeComponent();

            RepositoryItemCustomLookUpEdit ritem = new RepositoryItemCustomLookUpEdit();
            ritem.DataSource = GetData(10);
            ritem.ValueMember = "Name";
            ritem.DisplayMember = "Name";
            ritem.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.OnlyInPopup;
            ritem.AutoSearchColumnIndex = 3;
            gridControl1.RepositoryItems.Add(ritem);

            gridControl1.DataSource = GetMainDataSource(10);
            gridView1.Columns["Name"].ColumnEdit = ritem;

        }


        DataTable GetData(int count) {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("Info", typeof(string));
            Random r = new Random();
            for(int i = 0; i < count / 4; i++) {
                dt.Rows.Add(i, "Name" + i, r.Next(100), "abc" + i);
            }
            int rowCount = dt.Rows.Count;
            for(int i = rowCount; i < rowCount + count / 4; i++) {
                dt.Rows.Add(i, "Name" + i, r.Next(100), "bcd" + i);
            }
            rowCount = dt.Rows.Count;
            for(int i = rowCount; i < rowCount + count / 4; i++) {
                dt.Rows.Add(i, "Name" + i, r.Next(100), "def" + i);
            }
            rowCount = dt.Rows.Count;
            for(int i = rowCount; i < count; i++) {
                dt.Rows.Add(i, "Name" + i, r.Next(100), "efg" + i);
            }
            return dt;
        }

        List<MyObject> GetMainDataSource(int count) {
            List<MyObject> list = new List<MyObject>();
            for (int i = 0; i < count; i++) {
                MyObject m = new MyObject() { ID = i, Name = "Name" + i };
                   list.Add(m);
            }
            return list;
        }

    }

    public class MyObject {
        public int ID { set; get; }
        public string Name { set; get; }
    }
}
