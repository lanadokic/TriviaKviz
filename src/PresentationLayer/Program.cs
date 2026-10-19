using System;
using System.Configuration;
using System.Net.Http;
using System.Windows.Forms;
using DataAccessLayer;
using PresentationLayer.Forms;

namespace PresentationLayer
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var cs = ConfigurationManager.ConnectionStrings["TriviaDb"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
            {
                MessageBox.Show(
                    "Nedostaje connection string 'TriviaDb' u App.config.",
                    "Greška",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var db = new Db(cs);

            // Jedini repo koji nam sad treba
            var resultRepo = new ResultRepository(db);

            // OpenTDB API klijent
            var api = new OpenTdbClient(new HttpClient());

            Application.Run(new MainForm(api, resultRepo));
        }
    }
}
