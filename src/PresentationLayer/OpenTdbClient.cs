using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using DataAccessLayer;

namespace PresentationLayer
{
    public sealed class OpenTdbClient
    {
        private readonly HttpClient _http;
        private readonly JavaScriptSerializer _json = new JavaScriptSerializer();

        public OpenTdbClient(HttpClient httpClient)
        {
            _http = httpClient;
            _http.Timeout = TimeSpan.FromSeconds(20);
        }

        public async Task<List<Category>> FetchCategoriesAsync()
        {
            var json = await _http.GetStringAsync("https://opentdb.com/api_category.php");
            var dto = _json.Deserialize<CategoryResponse>(json) ?? new CategoryResponse();

            return dto.trivia_categories
                .Select(x => new Category
                {
                    Id = x.id,
                    category = WebUtility.HtmlDecode(x.name)
                })
                .ToList();
        }

        public async Task<List<QuizQuestion>> FetchQuestionsAsync(int amount, int categoryId, string difficulty, string type)
        {
            amount = Math.Max(1, Math.Min(50, amount));
            var url = BuildApiUrl(amount, categoryId, difficulty, type);

            var json = await _http.GetStringAsync(url);
            var dto = _json.Deserialize<QuestionResponse>(json) ?? new QuestionResponse();

            if (dto.response_code != 0)
                throw new InvalidOperationException("OpenTDB response_code=" + dto.response_code + ". Promijeni parametre ili pričekaj (rate limit).");

            return dto.results.Select(MapQuestion).ToList();
        }

        private static string BuildApiUrl(int amount, int apiCategoryId, string difficulty, string type)
        {
            var qs = new List<string> { "amount=" + amount, "category=" + apiCategoryId };
            if (!string.IsNullOrWhiteSpace(difficulty)) qs.Add("difficulty=" + difficulty);
            if (!string.IsNullOrWhiteSpace(type)) qs.Add("type=" + type);
            return "https://opentdb.com/api.php?" + string.Join("&", qs);
        }

        private static QuizQuestion MapQuestion(QuestionDto q)
        {
            var correct = WebUtility.HtmlDecode(q.correct_answer);
            var answers = q.incorrect_answers.Select(WebUtility.HtmlDecode).ToList();
            answers.Add(correct);

            var rng = new Random();
            for (int i = answers.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                var tmp = answers[i];
                answers[i] = answers[j];
                answers[j] = tmp;
            }

            return new QuizQuestion
            {
                Text = WebUtility.HtmlDecode(q.question),
                CorrectAnswer = correct,
                Answers = answers,
                Type = q.type
            };
        }

        private sealed class CategoryResponse
        {
            public List<CategoryDto> trivia_categories { get; set; } = new List<CategoryDto>();
        }
        private sealed class CategoryDto { public int id { get; set; } public string name { get; set; } = ""; }

        private sealed class QuestionResponse
        {
            public int response_code { get; set; }
            public List<QuestionDto> results { get; set; } = new List<QuestionDto>();
        }
        private sealed class QuestionDto
        {
            public string type { get; set; } = "";
            public string question { get; set; } = "";
            public string correct_answer { get; set; } = "";
            public List<string> incorrect_answers { get; set; } = new List<string>();
        }
    }

    public sealed class QuizQuestion
    {
        public string Text { get; set; } = "";
        public IList<string> Answers { get; set; } = new List<string>();
        public string CorrectAnswer { get; set; } = "";
        public string Type { get; set; } = "";
    }
}
