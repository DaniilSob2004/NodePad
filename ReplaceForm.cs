using System.Windows.Forms;

namespace WordPad
{
    public partial class ReplaceForm : Form
    {
        public ReplaceForm()
        {
            InitializeComponent();
        }

        public string GetOldString()
        {
            return tbOldString.Text;
        }

        public string GetNewString()
        {
            return tbNewString.Text;
        }

        public bool GetReplaceAll()
        {
            return checkBoxReplaceAll.Checked;
        }
    }
}
