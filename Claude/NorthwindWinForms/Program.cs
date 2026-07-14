using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NorthwindWinForms
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // フェーズ2以降: 受注詳細画面（編集モード）の Form1 を起動する。
            // （DB接続疎通確認は Form0 で実施可能）
            Application.Run(new Form1());
        }
    }
}
