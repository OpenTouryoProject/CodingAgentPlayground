using System;
using System.Windows.Forms;

namespace NorthwindWinForms
{
    /// <summary>
    /// フェーズ0のDB接続疎通確認用テストフォーム。
    /// 「接続テスト」ボタンで DbHelper.TestConnection() を実行し結果を表示する。
    /// </summary>
    public partial class Form0 : Form
    {
        public Form0()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                int count = DbHelper.TestConnection();
                lblResult.Text = "接続成功: Customers 件数 = " + count;
            }
            catch (Exception ex)
            {
                // ハンドリングした例外は MessageBox で表示する方針（AGENTS.md）
                lblResult.Text = "接続失敗";
                MessageBox.Show(
                    "データベースへの接続に失敗しました。" + Environment.NewLine + ex.Message,
                    "接続エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
