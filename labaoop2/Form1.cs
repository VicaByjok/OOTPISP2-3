﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using labaoop2.Classes;
using labaoop2.Serializers;
namespace labaoop2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Type[] alltypes = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type currtype in alltypes)
            {
                if (currtype.FullName.Contains("Classes"))
                    comboBox1.Items.Add(currtype.Name);
                if (currtype.FullName.Contains("Serializers") && !currtype.FullName.Contains("ISerializer"))
                {
                    object someseril = Activator.CreateInstance(currtype);
                    comboBox2.Items.Add(someseril);
                }
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        public object buffobject;
        static bool StartSeril(ISerializer seril, List<object> listobj, string path, Form1 form)
        {
            return seril.Serialize(listobj, path, form);
        }
        static bool StartDeseril(ISerializer seril, string path, Form1 form)
        {
            return seril.Deserialize(path, form);
        }
        public void ShowObject(object obj)
        {
            buffobject = obj;
            object myobj = obj;
            Type myType = myobj.GetType();
            dataGridView1.Columns.Clear();
            PropertyInfo[] myPropertyInfo;
            myPropertyInfo = myType.GetProperties();

            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                if (myPropertyInfo[i].PropertyType.Name.ToString() == "String" || myPropertyInfo[i].PropertyType.Name.ToString() == "Int32")
                {
                    dataGridView1.Columns.Add(myPropertyInfo[i].Name, myPropertyInfo[i].Name);
                }
                else
                {
                    if (myPropertyInfo[i].PropertyType.Name.ToString() == "Boolean")
                    {
                        dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn());
                        dataGridView1.Columns[i].HeaderText = myPropertyInfo[i].Name;
                    }
                    else
                    {
                        dataGridView1.Columns.Add(new DataGridViewButtonColumn());
                        dataGridView1.Columns[i].HeaderText = myPropertyInfo[i].Name;
                    }
                }
            }
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            int k = 0;
            dataGridView1.Rows.Add();
            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(myobj, null);
                dataGridView1[k, 0].Value = propValue;
                k++;
            }
        }
        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                ShowObject(listBox1.SelectedItem);
            }
        }
        public void UpdateListBox(ListBox listb)
        {
            object[] listitems = new object[listBox1.Items.Count];
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listitems[i] = listBox1.Items[i];
            }
            listb.Items.Clear();
            listb.Items.AddRange(listitems);
        }
        private void Update_Click(object sender, EventArgs e)
        {
            if (buffobject != null)
            {
                object myobj = buffobject;
                Type myType = myobj.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
                int k = 0;
                foreach (PropertyInfo prop in props)
                {
                    if (prop.PropertyType.Name.ToString() == "String")
                        try
                        { prop.SetValue(myobj, dataGridView1[k, 0].Value); }
                        catch
                        { MessageBox.Show("Неверный формат", "Ошибка", MessageBoxButtons.OK); }
                    else if (prop.PropertyType.Name.ToString() == "Int32")
                        try
                        { prop.SetValue(myobj, Convert.ToInt32(dataGridView1[k, 0].Value.ToString())); }
                        catch
                        { MessageBox.Show("Неверный формат", "Ошибка", MessageBoxButtons.OK); }
                    else if (prop.PropertyType.Name.ToString() == "Boolean")
                        try
                        { prop.SetValue(myobj, dataGridView1[k, 0].Value); }
                        catch
                        { MessageBox.Show("Неверный формат", "Ошибка", MessageBoxButtons.OK); }
                    k++;
                }
               UpdateListBox(listBox1);
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if ((listBox1.SelectedIndex != -1))
            {
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                dataGridView1.Columns.Clear();
            }
        }

        private void Create_Click(object sender, EventArgs e)
        {
            Type MyType = Type.GetType("labaoop2.Classes." + comboBox1.Text);
            object myobj = Activator.CreateInstance(MyType,textBox1.Text);
            listBox1.Items.Add(myobj);
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {            
            if (dataGridView1.CurrentCell.OwningColumn.GetType()==(new DataGridViewButtonColumn()).GetType())
            {
                object obj = dataGridView1.CurrentCell.Value;
                buffobject = obj;
                ShowObject(obj);
            }
        }

        private void Seril_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<object> list = new List<object>();
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    list.Add(listBox1.Items[i]);
                }
                StartSeril((ISerializer)comboBox2.SelectedItem, list, saveFileDialog1.FileName, (Form1)FindForm());
            }
        }

        private void Deseril_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Clear();
                StartDeseril((ISerializer)comboBox2.SelectedItem, openFileDialog1.FileName, (Form1)FindForm());
            }
        }
    }
}
