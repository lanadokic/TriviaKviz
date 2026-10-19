using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccessLayer;

namespace PresentationLayer.Forms
{
    public partial class LeaderboardForm : Form
    {
        private readonly ResultRepository _resultRepo;

        public LeaderboardForm(ResultRepository resultRepo)
        {
            InitializeComponent();

            _resultRepo = resultRepo;

            Load += async (s, e) => await InitAsync();
            btnRefresh.Click += async (s, e) => await RefreshAsync();
            cmbCategory.SelectedIndexChanged += async (s, e) => await RefreshAsync();
        }

        private async Task InitAsync()
        {
            var cats = await _resultRepo.GetCategoriesAsync();

            cmbCategory.Items.Clear();
            cmbCategory.Items.Add(new Category { Id = 0, category = "Sve kategorije" });

            foreach (var c in cats)
                cmbCategory.Items.Add(c);

            cmbCategory.DisplayMember = "kategorija";
            cmbCategory.SelectedIndex = 0;

            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            var rows = await _resultRepo.GetLeaderboardAsync(50);

            var selected = cmbCategory.SelectedItem as Category;
            var selectedName = selected?.category;

            // Filter na klijentskoj strani (jer u tablici TRIVIA_CAT_DOKIC je kategorija string)
            if (!string.IsNullOrWhiteSpace(selectedName) && selectedName != "Sve kategorije")
                rows = rows.Where(r => r.category == selectedName).ToList();

            grid.DataSource = rows.Select((r, i) => new
            {
                Rank = i + 1,
                Igrac = r.name,
                Kategorija = r.category,
                Tezina = r.difficulty,
                Bodovi = r.score,
                Vrijeme = TimeSpan.FromMilliseconds(r.time).ToString(@"mm\:ss\.fff")
            }).ToList();
        }
    }
}
