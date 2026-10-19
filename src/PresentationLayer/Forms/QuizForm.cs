using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccessLayer;

namespace PresentationLayer.Forms
{
    public partial class QuizForm : Form
    {
        private readonly Category _category;
        private readonly string _userName;
        private readonly string _tezina; // "easy"/"medium"/"hard" ili "" ako Any
        private readonly ResultRepository _resultRepo;
        private readonly QuizEngine _engine;
        private readonly Stopwatch _sw = new Stopwatch();
        private readonly List<RadioButton> _radioButtons = new List<RadioButton>();

        public QuizForm(
            Category category,
            string userName,
            IEnumerable<QuizQuestion> questions,
            ResultRepository resultRepo,
            string tezina)
        {
            InitializeComponent();

            _category = category ?? throw new ArgumentNullException(nameof(category));
            _userName = (userName ?? "").Trim();
            _tezina = (tezina ?? "").Trim();
            _resultRepo = resultRepo ?? throw new ArgumentNullException(nameof(resultRepo));
            _engine = new QuizEngine(questions ?? throw new ArgumentNullException(nameof(questions)));

            // naziv forme
            Text = "Kviz - " + _category.category;

            Shown += (s, e) =>
            {
                _sw.Start();
                ShowCurrentQuestion();
            };

            btnNext.Click += async (s, e) => await NextAsync();
        }

        private void ShowCurrentQuestion()
        {
            var q = _engine.Current;
            if (q == null) return;

            lblFeedback.Text = "";
            lblProgress.Text = $"Pitanje {_engine.CurrentIndex + 1}/{_engine.TotalQuestions} | Bodovi: {_engine.Score}";
            lblQuestion.Text = q.Text;

            panelAnswers.Controls.Clear();
            _radioButtons.Clear();

            foreach (var ans in q.Answers)
            {
                var rb = new RadioButton
                {
                    Text = ans,
                    AutoSize = true,
                    Margin = new Padding(6),
                    MaximumSize = new Size(680, 0)
                };
                _radioButtons.Add(rb);
                panelAnswers.Controls.Add(rb);
            }
        }

        private async Task NextAsync()
        {
            var selected = _radioButtons.FirstOrDefault(x => x.Checked)?.Text;
            if (string.IsNullOrWhiteSpace(selected))
            {
                MessageBox.Show("Odaberi odgovor.");
                return;
            }

            bool correct = _engine.SubmitAnswer(selected);
            lblFeedback.Text = correct ? "✅ Točno!" : "❌ Netočno!";

            if (_engine.IsFinished)
            {
                _sw.Stop();
                int playTimeMs = (int)_sw.ElapsedMilliseconds;

                // spremanje u dbo.TRIVIA_DOKIC (ime, bodovi, vrijeme, kategorija, tezina)
                var row = new LeaderboardRow
                {
                    name = _userName,
                    score = _engine.Score,
                    time = playTimeMs,
                    category = _category.category,
                    difficulty = string.IsNullOrWhiteSpace(_tezina) ? "Any" : _tezina
                };

                await _resultRepo.SaveResultAsync(row);

                MessageBox.Show(
                    $"Kviz završen!\n\nTočno: {_engine.CorrectCount}\nNetočno: {_engine.IncorrectCount}\nBodovi: {_engine.Score}\nVrijeme: {_sw.Elapsed}",
                    "Rezultat");

                Close();
                return;
            }

            ShowCurrentQuestion();
        }
    }
}
