using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form0 : Form
    {
        public Form0()
        {
            InitializeComponent();
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            string message;
            bool success = DbHelper.TestConnection(out message);

            lblResult.Text = message;

            if (success)
            {
                MessageBox.Show(message, "疎通テスト結果", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(message, "疎通テスト結果", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
