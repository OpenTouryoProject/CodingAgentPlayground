using System;
using System.Web.UI;

namespace WebApplication1
{
    public partial class Form0 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnTest_Click(object sender, EventArgs e)
        {
            lblResult.Text = DbHelper.TestConnection();
        }
    }
}
