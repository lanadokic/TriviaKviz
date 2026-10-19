using System;
using System.Collections.Generic;
using System.Linq;

namespace PresentationLayer
{
    public sealed class QuizEngine
    {
        private readonly List<QuizQuestion> _questions;
        private int _index;

        public QuizEngine(IEnumerable<QuizQuestion> questions)
        {
            _questions = questions.ToList();
            _index = 0;
            Score = 0;
            CorrectCount = 0;
            IncorrectCount = 0;
        }

        public int Score { get; private set; }
        public int CorrectCount { get; private set; }
        public int IncorrectCount { get; private set; }
        public int TotalQuestions => _questions.Count;
        public int CurrentIndex => _index;
        public QuizQuestion Current => (_index >= 0 && _index < _questions.Count) ? _questions[_index] : null;
        public bool IsFinished => _index >= _questions.Count;

        public bool SubmitAnswer(string answer)
        {
            if (Current == null) throw new InvalidOperationException("Nema aktivnog pitanja.");
            bool correct = string.Equals((answer ?? "").Trim(), (Current.CorrectAnswer ?? "").Trim(), StringComparison.OrdinalIgnoreCase);

            if (correct) { CorrectCount++; Score += 1; }
            else { IncorrectCount++; }

            _index++;
            return correct;
        }
    }
}
