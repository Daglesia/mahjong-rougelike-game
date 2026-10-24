using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    public class ScoreClausesTests
    {
        private GameObject go;
        private ScoreManager manager;

        [SetUp]
        public void SetUp()
        {
            go = new GameObject("ScoreManager");
            manager = go.AddComponent<ScoreManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(go);
        }

        private static List<CardType> Cards(CardType a, CardType b, CardType c) =>
            new List<CardType> { a, b, c };

        // ---------------------------------------------------------------
        // Total score per combo (all default clauses stacked)
        // ---------------------------------------------------------------

        // Triplets: 30
        [TestCase(CardType.Bamboo5, CardType.Bamboo5, CardType.Bamboo5, 30, TestName = "Score: suit triplet = 30")]
        [TestCase(CardType.Thing1, CardType.Thing1, CardType.Thing1, 30, TestName = "Score: terminal triplet = 30")]
        [TestCase(CardType.Shock9, CardType.Shock9, CardType.Shock9, 30, TestName = "Score: Shock 9 triplet = 30")]
        [TestCase(CardType.Bamboo5, CardType.Bamboo5, CardType.UltimateJoker, 30, TestName = "Score: suit triplet w/ Ultimate = 30 (no honor bonus)")]
        [TestCase(CardType.Bamboo5, CardType.UltimateJoker, CardType.UltimateJoker, 30, TestName = "Score: 1 natural + 2 Ultimate = triplet only, 30")]
        [TestCase(CardType.UltimateJoker, CardType.UltimateJoker, CardType.UltimateJoker, 30, TestName = "Score: 3 Ultimate = 30 (neutral)")]

        // Wind / Dragon triplets: 30 + 30 = 60
        [TestCase(CardType.East, CardType.East, CardType.East, 60, TestName = "Score: wind triplet = 60")]
        [TestCase(CardType.Red, CardType.Red, CardType.Red, 60, TestName = "Score: dragon triplet = 60")]
        [TestCase(CardType.East, CardType.East, CardType.WindJoker, 60, TestName = "Score: wind + WindJoker = 60")]
        [TestCase(CardType.North, CardType.WindJoker, CardType.UltimateJoker, 60, TestName = "Score: wind + WindJoker + Ultimate = 60")]
        [TestCase(CardType.Red, CardType.Red, CardType.DragonJoker, 60, TestName = "Score: dragon + DragonJoker = 60")]
        [TestCase(CardType.East, CardType.UltimateJoker, CardType.UltimateJoker, 60, TestName = "Score: wind + 2 Ultimate = 60")]
        [TestCase(CardType.WindJoker, CardType.WindJoker, CardType.WindJoker, 60, TestName = "Score: 3 WindJokers = 60")]
        [TestCase(CardType.DragonJoker, CardType.DragonJoker, CardType.UltimateJoker, 60, TestName = "Score: 2 DragonJokers + Ultimate = 60")]

        // Edge sequences (start at 1 or end at 9): 20
        [TestCase(CardType.Bamboo1, CardType.Bamboo2, CardType.Bamboo3, 20, TestName = "Score: sequence 1-2-3 = 20")]
        [TestCase(CardType.Bamboo9, CardType.Bamboo7, CardType.Bamboo8, 20, TestName = "Score: unordered 7-8-9 = 20")]
        [TestCase(CardType.Thing2, CardType.Thing3, CardType.UltimateJoker, 20, TestName = "Score: 2-3-J can be 1-2-3 = 20")]
        [TestCase(CardType.Shock8, CardType.Shock9, CardType.UltimateJoker, 20, TestName = "Score: 8-9-J can be 7-8-9 = 20")]
        [TestCase(CardType.Shock1, CardType.Shock3, CardType.UltimateJoker, 20, TestName = "Score: 1-J-3 = 20")]

        // Regular sequences: 10
        [TestCase(CardType.Bamboo4, CardType.Bamboo5, CardType.Bamboo6, 10, TestName = "Score: sequence 4-5-6 = 10")]
        [TestCase(CardType.Bamboo2, CardType.Bamboo3, CardType.Bamboo4, 10, TestName = "Score: sequence 2-3-4 = 10")]
        [TestCase(CardType.Bamboo6, CardType.Bamboo7, CardType.Bamboo8, 10, TestName = "Score: sequence 6-7-8 = 10")]
        [TestCase(CardType.Thing2, CardType.Thing4, CardType.UltimateJoker, 10, TestName = "Score: 2-J-4 = 10 (window must be 2-4)")]
        [TestCase(CardType.Thing5, CardType.Thing6, CardType.UltimateJoker, 10, TestName = "Score: 5-6-J = 10 (can't reach an edge)")]

        // Invalid combos: 0
        [TestCase(CardType.Bamboo1, CardType.Bamboo2, CardType.Bamboo4, 0, TestName = "Score: gap = 0")]
        [TestCase(CardType.Bamboo1, CardType.Thing2, CardType.Shock3, 0, TestName = "Score: mixed suits = 0")]
        [TestCase(CardType.East, CardType.South, CardType.West, 0, TestName = "Score: winds in a row = 0")]
        [TestCase(CardType.Bamboo5, CardType.Bamboo5, CardType.WindJoker, 0, TestName = "Score: suit + WindJoker = 0")]
        [TestCase(CardType.WindJoker, CardType.DragonJoker, CardType.UltimateJoker, 0, TestName = "Score: Wind + Dragon jokers = 0")]
        public void Award_ReturnsExpectedPoints(CardType a, CardType b, CardType c, int expected)
        {
            int gained = manager.Award(Cards(a, b, c));

            Assert.AreEqual(expected, gained);
            Assert.AreEqual(expected, manager.Score);
        }

        // ---------------------------------------------------------------
        // Individual clauses
        // ---------------------------------------------------------------

        [Test]
        public void TripletClause_AwardsConfiguredPoints()
        {
            var clause = new TripletClause { points = 45 };

            Assert.IsTrue(clause.TryScore(Cards(CardType.Bamboo5, CardType.Bamboo5, CardType.Bamboo5), out var entry));
            Assert.AreEqual(45, entry.Points);
        }

        [Test]
        public void TripletClause_DoesNotApplyToSequence()
        {
            Assert.IsFalse(new TripletClause().TryScore(Cards(CardType.Bamboo1, CardType.Bamboo2, CardType.Bamboo3), out _));
        }

        [Test]
        public void HonorTripletClause_DoesNotApplyToSuitTripletWithUltimateJoker()
        {
            var cards = Cards(CardType.Bamboo5, CardType.Bamboo5, CardType.UltimateJoker);
            Assert.IsFalse(new HonorTripletClause().TryScore(cards, out _));
        }

        [Test]
        public void HonorTripletClause_DoesNotApplyToThreeUltimateJokers()
        {
            var cards = Cards(CardType.UltimateJoker, CardType.UltimateJoker, CardType.UltimateJoker);
            Assert.IsFalse(new HonorTripletClause().TryScore(cards, out _));
        }

        [Test]
        public void HonorTripletClause_AppliesToWindTriplet()
        {
            Assert.IsTrue(new HonorTripletClause().TryScore(Cards(CardType.West, CardType.West, CardType.West), out _));
        }

        [Test]
        public void EdgeAndPlainSequenceClauses_AreMutuallyExclusive()
        {
            var edge = new EdgeSequenceClause();
            var plain = new PlainSequenceClause();

            var sequences = new[]
            {
                Cards(CardType.Bamboo1, CardType.Bamboo2, CardType.Bamboo3),
                Cards(CardType.Bamboo4, CardType.Bamboo5, CardType.Bamboo6),
                Cards(CardType.Bamboo7, CardType.Bamboo8, CardType.Bamboo9),
                Cards(CardType.Bamboo2, CardType.Bamboo3, CardType.UltimateJoker),
                Cards(CardType.Bamboo5, CardType.Bamboo6, CardType.UltimateJoker),
            };

            foreach (var seq in sequences)
            {
                bool e = edge.TryScore(seq, out _);
                bool p = plain.TryScore(seq, out _);
                Assert.AreNotEqual(e, p, $"Exactly one sequence clause should apply to {string.Join(",", seq)}");
            }
        }

        [Test]
        public void SequenceClauses_DoNotApplyToTriplets()
        {
            var cards = Cards(CardType.Bamboo5, CardType.UltimateJoker, CardType.UltimateJoker);

            Assert.IsFalse(new EdgeSequenceClause().TryScore(cards, out _));
            Assert.IsFalse(new PlainSequenceClause().TryScore(cards, out _));
        }

        // ---------------------------------------------------------------
        // ScoreManager behaviour
        // ---------------------------------------------------------------

        [Test]
        public void Award_AccumulatesAcrossCalls()
        {
            manager.Award(Cards(CardType.Bamboo5, CardType.Bamboo5, CardType.Bamboo5));   // 30
            manager.Award(Cards(CardType.East, CardType.East, CardType.East));            // 60
            manager.Award(Cards(CardType.Thing1, CardType.Thing2, CardType.Thing3));      // 20

            Assert.AreEqual(110, manager.Score);
        }

        [Test]
        public void Award_RaisesScoreChangedWithNewTotal()
        {
            var received = new List<int>();
            manager.ScoreChanged += received.Add;

            manager.Award(Cards(CardType.Bamboo5, CardType.Bamboo5, CardType.Bamboo5));
            manager.Award(Cards(CardType.Red, CardType.Red, CardType.Red));

            CollectionAssert.AreEqual(new[] { 30, 90 }, received);
        }

        [Test]
        public void Award_InvalidCombo_DoesNotChangeScore()
        {
            manager.Award(Cards(CardType.Bamboo5, CardType.Bamboo5, CardType.Bamboo5));
            manager.Award(Cards(CardType.Bamboo1, CardType.Thing2, CardType.Shock3));

            Assert.AreEqual(30, manager.Score);
        }

        [Test]
        public void AddClause_StacksWithExistingClauses()
        {
            manager.AddClause(new FlatBonusClause(5));

            int gained = manager.Award(Cards(CardType.Bamboo5, CardType.Bamboo5, CardType.Bamboo5));

            Assert.AreEqual(35, gained);
        }

        [Test]
        public void RemoveClause_StopsContributing()
        {
            var bonus = new FlatBonusClause(5);
            manager.AddClause(bonus);
            manager.RemoveClause(bonus);

            int gained = manager.Award(Cards(CardType.Bamboo5, CardType.Bamboo5, CardType.Bamboo5));

            Assert.AreEqual(30, gained);
        }

        // Test double: adds a flat bonus to any valid combo
        private class FlatBonusClause : IScoreClause
        {
            private readonly int bonus;
            public FlatBonusClause(int bonus) { this.bonus = bonus; }

            public bool TryScore(IReadOnlyList<CardType> cards, out ScoreEntry entry)
            {
                entry = new ScoreEntry("Bonus", bonus);
                return ComboCalculator.ComboValid(cards);
            }
        }
    }
}