using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Pathway.Core.Abstract;
using Pathway.Core.Services;
using Pathway.Loader.Infrastructure;
using Pathway.Core.Concrete;
using log4net;

namespace Pathway.Loader {
    public partial class Form1 : Form {
        private static readonly ILog Log = LogManager.GetLogger("PathwayLoader");

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, System.EventArgs e) {
        }
    }
}