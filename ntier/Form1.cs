using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Reporting.WinForms;
using System.IO;

namespace NTier
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {



            
        }




        private void button1_Click(object sender, EventArgs e)
        {

            var _tier = NTier.Request.utility.createBussinessTierFromXmlForWin(@"D:\2016\HRDMS\HRDMS\appConfig\appConfig.xml");
            var cmd = new clsCmd();
            cmd.setValue("Njoin_id", 1);
            var rpt = _tier.getSQLReport("test", cmd);

            using (FileStream fs = new FileStream("output.pdf", FileMode.Create))
            {
                rpt.render("pdf",fs);
            }
        }

    }
}
