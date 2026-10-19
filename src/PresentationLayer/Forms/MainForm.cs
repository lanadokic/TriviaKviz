using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccessLayer;

namespace PresentationLayer.Forms
{
    public partial class MainForm : Form
    {
        private readonly OpenTdbClient _api;
        private readonly ResultRepository _resultRepo;

        public MainForm(OpenTdbClient api, ResultRepository resultRepo)
        {
            InitializeComponent();

            _api = api;
            _resultRepo = resultRepo;

            cmbDifficulty.Items.Add(new ComboItem("", "Any"));
            cmbDifficulty.Items.Add(new ComboItem("easy", "Easy"));
            cmbDifficulty.Items.Add(new ComboItem("medium", "Medium"));
            cmbDifficulty.Items.Add(new ComboItem("hard", "Hard"));
            cmbDifficulty.SelectedIndex = 0;

            cmbType.Items.Add(new ComboItem("", "Any"));
            cmbType.Items.Add(new ComboItem("multiple", "Multiple Choice"));
            cmbType.Items.Add(new ComboItem("boolean", "True / False"));
            cmbType.SelectedIndex = 0;

            Load += async (s, e) => await LoadCategoriesFromDbAsync();
            btnRefresh.Click += async (s, e) => await RefreshCategoriesFromApiAsync();
            btnStart.Click += async (s, e) => await StartQuizAsync();
            btnLeaderboard.Click += (s, e) => ShowLeaderboard();
        }

        private async Task LoadCategoriesFromDbAsync()
        {
            try
            {
                var cats = await _resultRepo.GetCategoriesAsync();

                cmbCategory.DataSource = null;
                cmbCategory.DisplayMember = "category";
                cmbCategory.ValueMember = "Id";
                cmbCategory.DataSource = cats;

                lblStatus.Text = cats.Count == 0
                    ? "Baza nema kategorije. Klikni 'Osvježi kategorije (API)'."
                    : "Učitano kategorija: " + cats.Count;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Greška: " + ex.Message;
            }
        }

        private async Task RefreshCategoriesFromApiAsync()
        {
            SetBusy(true);
            try
            {
                lblStatus.Text = "Dohvaćam kategorije...";

                
                var catsFromApi = await _api.FetchCategoriesAsync();

               
                await _resultRepo.UpsertCategoriesAsync(catsFromApi);

                await LoadCategoriesFromDbAsync();
                lblStatus.Text = "Kategorije osvježene.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Greška: " + ex.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task StartQuizAsync()
        {
            var cat = cmbCategory.SelectedItem as Category;
            if (cat == null) { MessageBox.Show("Odaberi kategoriju."); return; }

            var userName = (txtName.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(userName)) { MessageBox.Show("Unesi ime igrača."); return; }

            int amount = (int)numAmount.Value;
            var diff = (cmbDifficulty.SelectedItem as ComboItem)?.Value ?? "";
            var type = (cmbType.SelectedItem as ComboItem)?.Value ?? "";

            SetBusy(true);
            try
            {
                lblStatus.Text = "Dohvaćam pitanja...";

                
                var questions = await _api.FetchQuestionsAsync(amount, cat.Id, diff, type);

                // QuizForm treba vratiti rezultat (bodovi + vrijeme),
                //ovdje spremamo ime + bodovi + vrijeme + kategorija + tezina
                using (var quiz = new QuizForm(cat, userName, questions, _resultRepo, diff))
                {
                    Hide();
                    quiz.ShowDialog(this);
                    Show();
                }

                lblStatus.Text = "Kviz završen. Rezultat spremljen.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Greška: " + ex.Message;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ShowLeaderboard()
        {
            using (var f = new LeaderboardForm(_resultRepo))
                f.ShowDialog(this);
        }

        private void SetBusy(bool busy)
        {
            UseWaitCursor = busy;
            btnRefresh.Enabled = !busy;
            btnStart.Enabled = !busy;
            btnLeaderboard.Enabled = !busy;
        }

        private sealed class ComboItem
        {
            public string Value { get; }
            public string Text { get; }
            public ComboItem(string value, string text) { Value = value; Text = text; }
            public override string ToString() => Text;
        }
    }
}
