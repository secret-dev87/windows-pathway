using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Pathway.Core.Abstract;
using Pathway.Core.Services;
using Pathway.Loader.Infrastructure;
using log4net;

namespace Pathway.Loader {
    public partial class Form1 : Form {
        private static readonly ILog Log = LogManager.GetLogger("PathwayLoader");

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            string connectionStringMain = "server=localhost;uid=root;pwd=Dev1121!;database=pmc";
            string connectionStringSystem = "server=localhost;uid=root;pwd=Dev1121!;database=pmc080627";
            var systemSerial = "080627";

            string uwsUnzipPath = "UMP60648_212581627201996647.402";

            var loader = new OpenUWSPathway(uwsUnzipPath, Log, connectionStringMain, connectionStringSystem, systemSerial);

            try
            {
                loader.CreateNewData();
                this.Focus();
                this.Activate();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}