using System.Windows.Forms;

namespace WordPad
{
    public partial class SearchForm : Form
    {
        public SearchForm()
        {
            InitializeComponent();
        }

        public string GetSearchString()
        {
            return tbSearch.Text;
        }
    }
}
