using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace multimedia1
{
    public partial class CustomPictureView : Panel
    {


        public CustomPictureView() : base()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.Opaque, true);
        }

        public CustomPictureView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
