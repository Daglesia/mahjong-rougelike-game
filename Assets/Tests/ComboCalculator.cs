using System.Collections.Generic;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class ComboCalculatorTests
    {
        private static List<CardType> Cards(CardType a, CardType b, CardType c)
        {
            return new List<CardType> { a, b, c };
        }

        // ---------------------------------------------------------------
        // IsSequence
        // ---------------------------------------------------------------

        // Valid sequences (including unordered input) for each suit
        [TestCase(CardType.Bamboo1, CardType.Bamboo2, CardType.Bamboo3, TestName = "Sequence: Bamboo 1-2-3")]
        [TestCase(CardType.Bamboo3, CardType.Bamboo1, CardType.Bamboo2, TestName = "Sequence: Bamboo unordered 3-1-2")]
        [TestCase(CardType.Bamboo7, CardType.Bamboo8, CardType.Bamboo9, TestName = "Sequence: Bamboo 7-8-9")]
        [TestCase(CardType.Thing1, CardType.Thing2, CardType.Thing3, TestName = "Sequence: Thing 1-2-3")]
        [TestCase(CardType.Thing9, CardType.Thing7, CardType.Thing8, TestName = "Sequence: Thing unordered 9-7-8")]
        [TestCase(CardType.Shock1, CardType.Shock2, CardType.Shock3, TestName = "Sequence: Shock 1-2-3")]
        [TestCase(CardType.Shock5, CardType.Shock6, CardType.Shock4, TestName = "Sequence: Shock unordered 5-6-4")]
        [TestCase(CardType.Bamboo1, CardType.Bamboo2, CardType.UltimateJoker, TestName = "Sequence: joker completes end (B1-B2-J)")]
        [TestCase(CardType.Bamboo1, CardType.UltimateJoker, CardType.Bamboo3, TestName = "Sequence: joker fills gap (B1-J-B3)")]
        [TestCase(CardType.UltimateJoker, CardType.Thing8, CardType.Thing9, TestName = "Sequence: joker as 7 (J-T8-T9)")]
        [TestCase(CardType.Shock9, CardType.UltimateJoker, CardType.Shock8, TestName = "Sequence: unordered with joker")]
        [TestCase(CardType.Shock5, CardType.UltimateJoker, CardType.UltimateJoker, TestName = "Sequence: one natural, two jokers")]
        public void IsSequence_Valid_ReturnsTrue(CardType a, CardType b, CardType c)
        {
            Assert.IsTrue(ComboCalculator.IsSequence(Cards(a, b, c)));
        }

        // Invalid sequences
        [TestCase(CardType.Bamboo1, CardType.Bamboo2, CardType.Bamboo4, TestName = "Not sequence: gap in Bamboo")]
        [TestCase(CardType.Bamboo1, CardType.Bamboo3, CardType.Bamboo5, TestName = "Not sequence: spaced by 2")]
        [TestCase(CardType.Bamboo2, CardType.Bamboo2, CardType.Bamboo3, TestName = "Not sequence: duplicate card")]
        [TestCase(CardType.Bamboo2, CardType.Bamboo2, CardType.Bamboo2, TestName = "Not sequence: all same")]
        [TestCase(CardType.Bamboo1, CardType.Thing2, CardType.Bamboo3, TestName = "Not sequence: mixed Bamboo/Thing")]
        [TestCase(CardType.Thing1, CardType.Shock2, CardType.Thing3, TestName = "Not sequence: mixed Thing/Shock")]
        [TestCase(CardType.Bamboo1, CardType.Thing2, CardType.Shock3, TestName = "Not sequence: all three suits")]
        // Three different suits, same rank
        [TestCase(CardType.Bamboo1, CardType.Thing1, CardType.Shock1, TestName = "Not sequence: three suits, same rank 1")]
        [TestCase(CardType.Bamboo5, CardType.Thing5, CardType.Shock5, TestName = "Not sequence: three suits, same rank 5")]
        [TestCase(CardType.Bamboo9, CardType.Thing9, CardType.Shock9, TestName = "Not sequence: three suits, same rank 9")]

        // Three different suits, consecutive ranks (every ordering of suits)
        [TestCase(CardType.Bamboo1, CardType.Thing2, CardType.Shock3, TestName = "Not sequence: B1-T2-S3")]
        [TestCase(CardType.Shock1, CardType.Bamboo2, CardType.Thing3, TestName = "Not sequence: S1-B2-T3")]
        [TestCase(CardType.Thing1, CardType.Shock2, CardType.Bamboo3, TestName = "Not sequence: T1-S2-B3")]
        [TestCase(CardType.Thing4, CardType.Bamboo5, CardType.Shock6, TestName = "Not sequence: T4-B5-S6")]

        // Two suits mixed
        [TestCase(CardType.Bamboo1, CardType.Bamboo2, CardType.Thing3, TestName = "Not sequence: two Bamboo + one Thing")]
        [TestCase(CardType.Shock4, CardType.Thing5, CardType.Thing6, TestName = "Not sequence: one Shock + two Thing")]

        // Raw enum values are consecutive across a suit boundary
        [TestCase(CardType.Shock8, CardType.Shock9, CardType.Bamboo1, TestName = "Not sequence: crosses Shock/Bamboo boundary")]
        [TestCase(CardType.Shock9, CardType.Bamboo1, CardType.Bamboo2, TestName = "Not sequence: crosses Shock/Bamboo boundary (1+2)")]
        [TestCase(CardType.Bamboo8, CardType.Bamboo9, CardType.Thing1, TestName = "Not sequence: crosses Bamboo/Thing boundary")]
        [TestCase(CardType.Thing8, CardType.Thing9, CardType.East, TestName = "Not sequence: crosses Thing/Wind boundary")]

        // Honors never form sequences
        [TestCase(CardType.East, CardType.South, CardType.West, TestName = "Not sequence: winds in a row")]
        [TestCase(CardType.Red, CardType.Green, CardType.White, TestName = "Not sequence: dragons in a row")]

        // Wind/Dragon jokers can't be suit tiles
        [TestCase(CardType.Bamboo1, CardType.Bamboo2, CardType.WindJoker, TestName = "Not sequence: WindJoker in suit sequence")]
        [TestCase(CardType.Bamboo1, CardType.Bamboo2, CardType.DragonJoker, TestName = "Not sequence: DragonJoker in suit sequence")]
        [TestCase(CardType.Bamboo1, CardType.WindJoker, CardType.DragonJoker, TestName = "Not sequence: Wind + Dragon joker")]

        // Ultimate joker can't fix a bad sequence
        [TestCase(CardType.Bamboo1, CardType.Bamboo5, CardType.UltimateJoker, TestName = "Not sequence: naturals too far apart")]
        [TestCase(CardType.Bamboo2, CardType.Bamboo2, CardType.UltimateJoker, TestName = "Not sequence: duplicate natural + joker")]
        [TestCase(CardType.Bamboo1, CardType.Thing2, CardType.UltimateJoker, TestName = "Not sequence: mixed suits + joker")]
        [TestCase(CardType.Shock9, CardType.Bamboo1, CardType.UltimateJoker, TestName = "Not sequence: suit boundary + joker")]
        [TestCase(CardType.East, CardType.South, CardType.UltimateJoker, TestName = "Not sequence: honors + joker")]
        [TestCase(CardType.WindJoker, CardType.DragonJoker, CardType.UltimateJoker, TestName = "Not sequence: all jokers incl. Wind/Dragon")]
        public void IsSequence_Invalid_ReturnsFalse(CardType a, CardType b, CardType c)
        {
            Assert.IsFalse(ComboCalculator.IsSequence(Cards(a, b, c)));
        }

        // ---------------------------------------------------------------
        // IsTriplet
        // ---------------------------------------------------------------

        [TestCase(CardType.Bamboo5, CardType.Bamboo5, CardType.Bamboo5, TestName = "Triplet: Bamboo 5x3")]
        [TestCase(CardType.Thing1, CardType.Thing1, CardType.Thing1, TestName = "Triplet: Thing 1x3")]
        [TestCase(CardType.Shock9, CardType.Shock9, CardType.Shock9, TestName = "Triplet: Shock 9x3")]
        [TestCase(CardType.Bamboo1, CardType.Bamboo1, CardType.Bamboo1, TestName = "Triplet: Bamboo 1x3")]
        [TestCase(CardType.Thing9, CardType.Thing9, CardType.Thing9, TestName = "Triplet: Thing 9x3")]
        [TestCase(CardType.Shock1, CardType.Shock1, CardType.Shock1, TestName = "Triplet: Shock 1x3")]
        [TestCase(CardType.East, CardType.East, CardType.East, TestName = "Triplet: East x3")]
        [TestCase(CardType.Red, CardType.Red, CardType.Red, TestName = "Triplet: Red x3")]

        [TestCase(CardType.East, CardType.East, CardType.WindJoker, TestName = "Triplet: Wind + WindJoker")]
        [TestCase(CardType.East, CardType.WindJoker, CardType.WindJoker, TestName = "Triplet: Wind + 2 WindJokers")]
        [TestCase(CardType.North, CardType.WindJoker, CardType.UltimateJoker, TestName = "Triplet: Wind + WindJoker + UltimateJoker")]
        [TestCase(CardType.Red, CardType.Red, CardType.DragonJoker, TestName = "Triplet: Dragon + DragonJoker")]
        [TestCase(CardType.White, CardType.DragonJoker, CardType.UltimateJoker, TestName = "Triplet: Dragon + DragonJoker + UltimateJoker")]
        [TestCase(CardType.East, CardType.East, CardType.UltimateJoker, TestName = "Triplet: Wind + UltimateJoker")]
        [TestCase(CardType.Bamboo5, CardType.Bamboo5, CardType.UltimateJoker, TestName = "Triplet: Bamboo + UltimateJoker")]
        [TestCase(CardType.Bamboo5, CardType.UltimateJoker, CardType.Bamboo5, TestName = "Triplet: UltimateJoker in the middle")]
        [TestCase(CardType.WindJoker, CardType.WindJoker, CardType.WindJoker, TestName = "Triplet: three WindJokers")]
        [TestCase(CardType.DragonJoker, CardType.DragonJoker, CardType.UltimateJoker, TestName = "Triplet: 2 DragonJokers + UltimateJoker")]
        public void IsTriplet_ThreeIdentical_ReturnsTrue(CardType a, CardType b, CardType c)
        {
            Assert.IsTrue(ComboCalculator.IsTriplet(Cards(a, b, c)));
        }

        [TestCase(CardType.Bamboo5, CardType.Bamboo5, CardType.Bamboo6, TestName = "Not triplet: two match, one differs")]
        [TestCase(CardType.Bamboo5, CardType.Bamboo6, CardType.Bamboo5, TestName = "Not triplet: differs in middle")]
        [TestCase(CardType.Bamboo1, CardType.Bamboo2, CardType.Bamboo3, TestName = "Not triplet: all different")]
        [TestCase(CardType.Bamboo5, CardType.Thing5, CardType.Shock5, TestName = "Not triplet: same number, different suits")]
        [TestCase(CardType.Bamboo1, CardType.Thing1, CardType.Shock1, TestName = "Not triplet: rank 1 in three suits")]
        [TestCase(CardType.Bamboo9, CardType.Thing9, CardType.Shock9, TestName = "Not triplet: rank 9 in three suits")]
        [TestCase(CardType.East, CardType.South, CardType.West, TestName = "Not triplet: three different winds")]

        // Specific jokers can't replace the wrong category
        [TestCase(CardType.Bamboo5, CardType.Bamboo5, CardType.WindJoker, TestName = "Not triplet: suit tile + WindJoker")]
        [TestCase(CardType.Bamboo5, CardType.Bamboo5, CardType.DragonJoker, TestName = "Not triplet: suit tile + DragonJoker")]
        [TestCase(CardType.Red, CardType.Red, CardType.WindJoker, TestName = "Not triplet: Dragon + WindJoker")]
        [TestCase(CardType.East, CardType.East, CardType.DragonJoker, TestName = "Not triplet: Wind + DragonJoker")]
        [TestCase(CardType.Bamboo5, CardType.WindJoker, CardType.UltimateJoker, TestName = "Not triplet: suit tile + WindJoker + Ultimate")]

        // Naturals must still match
        [TestCase(CardType.East, CardType.South, CardType.UltimateJoker, TestName = "Not triplet: different winds + Ultimate")]
        [TestCase(CardType.East, CardType.South, CardType.WindJoker, TestName = "Not triplet: different winds + WindJoker")]
        [TestCase(CardType.Bamboo5, CardType.Thing5, CardType.UltimateJoker, TestName = "Not triplet: same rank, different suits + Ultimate")]

        // Jokers that can't represent the same tile
        [TestCase(CardType.WindJoker, CardType.DragonJoker, CardType.UltimateJoker, TestName = "Not triplet: Wind + Dragon + Ultimate jokers")]
        [TestCase(CardType.East, CardType.WindJoker, CardType.DragonJoker, TestName = "Not triplet: Wind + WindJoker + DragonJoker")]
        [TestCase(CardType.WindJoker, CardType.WindJoker, CardType.DragonJoker, TestName = "Not triplet: 2 WindJokers + DragonJoker")]
        public void IsTriplet_NotIdentical_ReturnsFalse(CardType a, CardType b, CardType c)
        {
            Assert.IsFalse(ComboCalculator.IsTriplet(Cards(a, b, c)));
        }

        // ---------------------------------------------------------------
        // ComboValid
        // ---------------------------------------------------------------

        [TestCase(CardType.Thing3, CardType.Thing3, CardType.Thing3, ExpectedResult = true, TestName = "ComboValid: triplet")]
        [TestCase(CardType.Shock4, CardType.Shock5, CardType.Shock6, ExpectedResult = true, TestName = "ComboValid: sequence")]
        [TestCase(CardType.Shock6, CardType.Shock4, CardType.Shock5, ExpectedResult = true, TestName = "ComboValid: unordered sequence")]
        [TestCase(CardType.Bamboo1, CardType.Bamboo2, CardType.Bamboo4, ExpectedResult = false, TestName = "ComboValid: neither (gap)")]
        [TestCase(CardType.Bamboo1, CardType.Thing2, CardType.Shock3, ExpectedResult = false, TestName = "ComboValid: neither (mixed suits)")]
        [TestCase(CardType.Bamboo2, CardType.Bamboo2, CardType.Bamboo3, ExpectedResult = false, TestName = "ComboValid: neither (pair + one)")]
        [TestCase(CardType.Bamboo5, CardType.Thing5, CardType.Shock5, ExpectedResult = false, TestName = "ComboValid: neither (same rank, three suits)")]
        [TestCase(CardType.Bamboo1, CardType.Thing2, CardType.Shock3, ExpectedResult = false, TestName = "ComboValid: neither (consecutive ranks, three suits)")]
        [TestCase(CardType.Shock9, CardType.Bamboo1, CardType.Bamboo2, ExpectedResult = false, TestName = "ComboValid: neither (crosses suit boundary)")]
        [TestCase(CardType.East, CardType.East, CardType.East, ExpectedResult = true, TestName = "ComboValid: honor triplet")]
        [TestCase(CardType.East, CardType.South, CardType.West, ExpectedResult = false, TestName = "ComboValid: neither (winds in a row)")]

        [TestCase(CardType.Bamboo1, CardType.Bamboo3, CardType.UltimateJoker, ExpectedResult = true, TestName = "ComboValid: sequence with joker")]
        [TestCase(CardType.East, CardType.East, CardType.WindJoker, ExpectedResult = true, TestName = "ComboValid: wind triplet with WindJoker")]
        [TestCase(CardType.Red, CardType.DragonJoker, CardType.UltimateJoker, ExpectedResult = true, TestName = "ComboValid: dragon triplet with two jokers")]
        [TestCase(CardType.Bamboo1, CardType.Bamboo2, CardType.WindJoker, ExpectedResult = false, TestName = "ComboValid: WindJoker can't complete a suit")]
        [TestCase(CardType.East, CardType.South, CardType.WindJoker, ExpectedResult = false, TestName = "ComboValid: different winds + WindJoker")]
        [TestCase(CardType.WindJoker, CardType.DragonJoker, CardType.UltimateJoker, ExpectedResult = false, TestName = "ComboValid: Wind + Dragon + Ultimate jokers")]
        public bool ComboValid_Cases(CardType a, CardType b, CardType c)
        {
            return ComboCalculator.ComboValid(Cards(a, b, c));
        }
    }
}