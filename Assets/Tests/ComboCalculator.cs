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
        public void IsTriplet_ThreeIdentical_ReturnsTrue(CardType a, CardType b, CardType c)
        {
            Assert.IsTrue(ComboCalculator.IsTriplet(Cards(a, b, c)));
        }

        [TestCase(CardType.Bamboo5, CardType.Bamboo5, CardType.Bamboo6, TestName = "Not triplet: two match, one differs")]
        [TestCase(CardType.Bamboo5, CardType.Bamboo6, CardType.Bamboo5, TestName = "Not triplet: differs in middle")]
        [TestCase(CardType.Bamboo1, CardType.Bamboo2, CardType.Bamboo3, TestName = "Not triplet: all different")]
        [TestCase(CardType.Bamboo5, CardType.Thing5, CardType.Shock5, TestName = "Not triplet: same number, different suits")]
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
        public bool ComboValid_Cases(CardType a, CardType b, CardType c)
        {
            return ComboCalculator.ComboValid(Cards(a, b, c));
        }
    }
}