using DemoLINQ.XMLclass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DemoLINQ
{
    public partial class MainForm : Form
    {
        private ArrayOfCD arrCD;
        private ArrayOfPRODUCER arrPRODUCER;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            using (FileStream file = new FileStream("cd_catalog _1.xml", FileMode.Open))
            {
                XmlSerializer xml = new XmlSerializer(typeof(ArrayOfCD));
                arrCD = (ArrayOfCD)xml.Deserialize(file);
                dataGridView1.DataSource = arrCD.CD;
            }

            using (FileStream file = new FileStream("cd_catalog _2.xml", FileMode.Open))
            {
                XmlSerializer xml = new XmlSerializer(typeof(ArrayOfPRODUCER));
                arrPRODUCER = (ArrayOfPRODUCER)xml.Deserialize(file);
                dataGridView2.DataSource = arrPRODUCER.PRODUCER;
            }
        }

        private void запрос1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var artist = (from a in arrCD.CD where a.YEAR > 1991  select new { a.TITLE, a.ARTIST, a.YEAR }).ToArray();
            var artist = arrCD.CD.Where(p => p.YEAR > 1991).Select(p => new { p.TITLE, p.ARTIST, p.YEAR }).ToArray();
            dataGridView3.DataSource = artist;
        }

        private void запрос2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var country = ((from a in arrCD.CD select new { a.COUNTRY }).Union(from a in arrCD.CD select new { a.COUNTRY })).ToArray();
            var country = arrCD.CD.GroupBy(p => p.COUNTRY).Select(p => new { COUNTRY = p.Key }).ToArray();
            dataGridView3.DataSource = country;
        }

        private void запрос3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var title = (from a in arrCD.CD where a.COUNTRY.StartsWith("USA") orderby a.YEAR select new { a.TITLE, a.COUNTRY, a.YEAR }).ToArray();
            var title = arrCD.CD.Where(p => p.COUNTRY.StartsWith("USA")).OrderBy(p => p.YEAR).Select(p => new { p.TITLE, p.COUNTRY, p.YEAR }).ToArray();
            dataGridView3.DataSource = title;
        }

        private void запрос4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var value = (from a in arrCD.CD group a by a.COUNTRY into gr select new { COUNTRY = gr.Key, PRICE = gr.Sum(s => s.PRICE) }).ToArray();
            var value = arrCD.CD.GroupBy(p => p.COUNTRY).Select(p => new { COUNTRY = p.Key, PRICE = p.Sum(s => s.PRICE) }).ToArray();
            dataGridView3.DataSource = value;
        }

        private void запрос5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var company = (from a in arrCD.CD
            //               group a by new { a.COMPANY, a.YEAR } into gr
            //               orderby gr.Key.YEAR
            //               select new { COMPANY = gr.Key.COMPANY, YEAR = gr.Key.YEAR, COUNT_TITLE = gr.Count() }).ToArray();
            var company = arrCD.CD.GroupBy(p => new { p.COMPANY, p.YEAR} ).OrderBy(b => b.Key.YEAR)
                .Select(p => new { COMPANY = p.Key.COMPANY, YEAR = p.Key.YEAR, COUNT_TITLE = p.Count() }).ToArray();
            dataGridView3.DataSource = company;
        }

        private void запрос6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var fee = (from a in arrCD.CD
            //           join b in arrPRODUCER.PRODUCER
            //           on a.PRODUCER equals b.ID
            //           where b.FEE == arrPRODUCER.PRODUCER.Max(m => m.FEE)
            //           select new { a.TITLE, b.NAME, b.FEE}).ToArray();
            var fee = arrCD.CD.Join(arrPRODUCER.PRODUCER, a => a.PRODUCER, b => b.ID, (a, b) => new { Table1 = a, Table2 = b })
                .Where(b => b.Table2.FEE == arrPRODUCER.PRODUCER.Max(m => m.FEE)).Select(p => new { p.Table1.TITLE, p.Table2.NAME, p.Table2.FEE }).ToArray();
            dataGridView3.DataSource = fee;
        }

        private void запрос7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var count = (from a in arrCD.CD
            //             join b in arrPRODUCER.PRODUCER
            //             on a.PRODUCER equals b.ID
            //             where a.YEAR >= 1988 && a.YEAR <= 1990
            //             orderby a.YEAR
            //             group b by b.NAME into gr
            //             select new { PRODUCER = gr.Key, COUNT = gr.Count()} ).ToArray();
            var count = arrCD.CD.Join(arrPRODUCER.PRODUCER, p => p.PRODUCER, c => c.ID, (p, c) => new { Table1 = p, Table2 = c })
                .Where(p => p.Table1.YEAR >= 1988 && p.Table1.YEAR <= 1990).OrderBy(p => p.Table1.YEAR).GroupBy(p => p.Table2.NAME)
                .Select(gr => new { PRODUCER = gr.Key, COUNT = gr.Count() }).ToArray();
            dataGridView3.DataSource = count;
        }

        private void запрос8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var surname = ((from b in arrPRODUCER.PRODUCER
            //               orderby b.DATE descending
            //               select new { b.NAME, b.DATE }).Take(1)).ToArray();
            var surname = arrPRODUCER.PRODUCER.OrderBy(p => p.DATE).Select(p => new { p.NAME, p.DATE }).Take(1).ToArray();
            dataGridView3.DataSource = surname;
        }

        private void запрос9ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var cheap = ((from a in arrCD.CD
            //             join b in arrPRODUCER.PRODUCER
            //             on a.PRODUCER equals b.ID
            //             orderby a.PRICE
            //             select new { a.TITLE, a.ARTIST, b.NAME, a.PRICE }).Take(1)).ToArray();
            var cheap = arrCD.CD.Join(arrPRODUCER.PRODUCER, a => a.PRODUCER, b => b.ID, (a, b) => new { Table1 = a, Table2 = b })
                .OrderBy(p => p.Table1.PRICE).Select(p => new { p.Table1.TITLE, p.Table1.ARTIST, p.Table2.NAME, p.Table1.PRICE }).Take(1).ToArray();
            dataGridView3.DataSource = cheap;
        }

        private void запрос10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var full = (from a in arrCD.CD
            //              join b in arrPRODUCER.PRODUCER
            //              on a.PRODUCER equals b.ID
            //              orderby a.YEAR, a.PRICE, a.TITLE
            //              select new { a.TITLE, a.ARTIST, a.COUNTRY, a.COMPANY, a.PRICE, a.YEAR, b.NAME, b.DATE, b.FEE }).ToArray();
            var full = arrCD.CD.Join(arrPRODUCER.PRODUCER, p => p.PRODUCER, c => c.ID, (p, c) => new { Table1 = p, Table2 = c })
                .OrderBy(p => p.Table1.YEAR).ThenBy(p => p.Table1.PRICE).ThenBy(p => p.Table1.TITLE).Select(p => new
                {
                    p.Table1.TITLE,
                    p.Table1.ARTIST,
                    p.Table1.COUNTRY,
                    p.Table1.COMPANY,
                    p.Table1.PRICE,
                    p.Table1.YEAR,
                    p.Table2.NAME,
                    p.Table2.DATE,
                    p.Table2.FEE
                }).ToArray();
            dataGridView3.DataSource = full;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Домашняя работа, Технология ADO.NET");
        }
    }
}
