using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Merge.Test
{
    [TestFixture]
    public class DifferenceMapTests
    {
        [Test]
        public void Should_fail_when_original_is_null()
        {
            Action action = () => new DifferenceMap(null, new string[0]);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Should_fail_when_target_is_null()
        {
            Action action = () => new DifferenceMap(new string[0], null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void GetFullDifference_should_return_empty_diff_when_two_lines_is_empty()
        {
            var difference = new DifferenceMap(new string[0], new string[0]);
            difference.Differences.Should().BeEmpty();
        }

        [Test]
        public void should_mark_all_target_lines_as_new_when_original_is_empty()
        {
            var difference = new DifferenceMap(new string[0], StringGenerator.RandomStrings(count: 5, enableWhitespaces: true));

            difference.Differences.Should().HaveCount(1);
            difference.Differences[0].From.Should().Be(0);
            difference.Differences[0].To.Should().Be(5);
            difference.Differences[0].DifferenceType.Should().Be(DifferenceType.Added);
        }

        [Test]
        public void should_mark_all_original_lines_as_deleted_when_target_is_empty()
        {
            var original = StringGenerator.RandomStrings(count: 5, enableWhitespaces: true);
            var target = new string[0];

            var difference = new DifferenceMap(original, target);

            difference.Differences.Should().HaveCount(1);
            difference.Differences[0].From.Should().Be(0);
            difference.Differences[0].To.Should().Be(5);
            difference.Differences[0].DifferenceType.Should().Be(DifferenceType.Deleted);
        }

        [Test]
        public void should_mark_all_target_lines_as_equals_when_target_has_no_changes()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToArray();

            var difference = new DifferenceMap(original, target);

            difference.Differences.Should().HaveCount(0);
        }

        [Test]
        public void should_find_all_new_lines()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToList();
            target.InsertRange(3, StringGenerator.RandomStrings(count: 3, enableWhitespaces: true));
            target.InsertRange(9, StringGenerator.RandomStrings(count: 2, enableWhitespaces: true));

            var difference = new DifferenceMap(original, target.ToArray());

            difference.Differences.Should().HaveCount(2);

            var firstDiff  = difference.Differences[0];
            firstDiff.From.Should().Be(3);
            firstDiff.To.Should().Be(5);
            firstDiff.DifferenceType.Should().Be(DifferenceType.Added);

            var secondDiff = difference.Differences[1];
            secondDiff.From.Should().Be(9);
            secondDiff.To.Should().Be(10);
            secondDiff.DifferenceType.Should().Be(DifferenceType.Added);
        }

        [Test]
        public void should_find_all_removed_lines()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToList();
            target.RemoveRange(6, 2);
            target.RemoveRange(3, 3);

            var difference = new DifferenceMap(original, target.ToArray());

            difference.Differences.Should().HaveCount(2);

            var firstDiff = difference.Differences[0];
            firstDiff.From.Should().Be(3);
            firstDiff.To.Should().Be(5);
            firstDiff.DifferenceType.Should().Be(DifferenceType.Deleted);

            var secondDiff = difference.Differences[1];
            secondDiff.From.Should().Be(6);
            secondDiff.To.Should().Be(7);
            secondDiff.DifferenceType.Should().Be(DifferenceType.Deleted);
        }

        [Test]
        public void should_find_all_replaced_lines()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToList();

            target[3] = StringGenerator.RandomString(enableWhitespaces: true);
            target[4] = StringGenerator.RandomString(enableWhitespaces: true);

            target[6] = StringGenerator.RandomString(enableWhitespaces: true);
            target[7] = StringGenerator.RandomString(enableWhitespaces: true);

            var difference = new DifferenceMap(original, target.ToArray());

            difference.Differences.Should().HaveCount(4);

            var diff1 = difference.Differences[0];
            diff1.From.Should().Be(3);
            diff1.To.Should().Be(4);
            diff1.DifferenceType.Should().Be(DifferenceType.Deleted);

            var diff2 = difference.Differences[1];
            diff2.From.Should().Be(5);
            diff2.To.Should().Be(6);
            diff2.DifferenceType.Should().Be(DifferenceType.Added);

            var diff3 = difference.Differences[2];
            diff3.From.Should().Be(8);
            diff3.To.Should().Be(9);
            diff3.DifferenceType.Should().Be(DifferenceType.Deleted);

            var diff4 = difference.Differences[3];
            diff4.From.Should().Be(10);
            diff4.To.Should().Be(11);
            diff4.DifferenceType.Should().Be(DifferenceType.Added);
        }

        /**
         linesDifference.Should().HaveCount(original.Length + 4);
            linesDifference[3].Type.Should().Be(DifferenceType.Deleted);
            linesDifference[4].Type.Should().Be(DifferenceType.Deleted);
            linesDifference[5].Type.Should().Be(DifferenceType.Added);
            linesDifference[6].Type.Should().Be(DifferenceType.Added);

            linesDifference[8].Type.Should().Be(DifferenceType.Deleted);
            linesDifference[9].Type.Should().Be(DifferenceType.Deleted);
            linesDifference[10].Type.Should().Be(DifferenceType.Added);
            linesDifference[11].Type.Should().Be(DifferenceType.Added);
            linesDifference.Where(x => x.Type == DifferenceType.Equals).Should().HaveCount(6);
         
          
         */
    }
}