﻿using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Merge.Test
{
    [TestFixture]
    public class DiffTests
    {
        [Test]
        public void Should_fail_when_original_is_null()
        {
            Action action = () => Diff.GetLinesDifference(null, new string[0]);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Should_fail_when_target_is_null()
        {
            Action action = () => Diff.GetLinesDifference(new string[0], null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void should_return_empty_diff_when_two_lines_is_empty()
        {
            var difference = Diff.GetLinesDifference(new string[0], new string[0]);

            difference.Should().BeEmpty();
        }

        [Test]
        public void should_mark_all_target_lines_as_new_when_original_is_empty()
        {
            var linesDifference = Diff.GetLinesDifference(new string[0], StringGenerator.RandomStrings(count: 5, enableWhitespaces: true));

            linesDifference.Should().HaveCount(5);
            linesDifference.Should().OnlyContain(d => d.Type == DifferenceType.Added);
        }

        [Test]
        public void should_mark_all_original_lines_as_deleted_when_target_is_empty()
        {
            var original = StringGenerator.RandomStrings(count: 5, enableWhitespaces: true);
            var target = new string[0];

            var linesDifference = Diff.GetLinesDifference(original, target);

            linesDifference.Should().HaveCount(5);
            linesDifference.Should().OnlyContain(d => d.Type == DifferenceType.Deleted);
        }

        [Test]
        public void should_mark_all_target_lines_as_equals_when_target_has_no_changes()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToArray();

            var linesDifference = Diff.GetLinesDifference(original, target);

            linesDifference.Should().HaveCount(10);
            linesDifference.Should().OnlyContain(d => d.Type == DifferenceType.Equals);
        }

        [Test]
        public void should_find_all_new_lines()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToList();
            target.InsertRange(3, StringGenerator.RandomStrings(count: 3, enableWhitespaces: true));
            target.InsertRange(9, StringGenerator.RandomStrings(count: 2, enableWhitespaces: true));

            var linesDifference = Diff.GetLinesDifference(original, target.ToArray());

            linesDifference.Should().HaveCount(target.Count);
            linesDifference[3].Type.Should().Be(DifferenceType.Added);
            linesDifference[4].Type.Should().Be(DifferenceType.Added);
            linesDifference[5].Type.Should().Be(DifferenceType.Added);
            linesDifference[9].Type.Should().Be(DifferenceType.Added);
            linesDifference[10].Type.Should().Be(DifferenceType.Added);
            linesDifference.Where(x => x.Type == DifferenceType.Equals).Should().HaveSameCount(original);
        }

        [Test]
        public void should_find_all_removed_lines()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToList();
            target.RemoveRange(6, 2);
            target.RemoveRange(3, 3);

            var linesDifference = Diff.GetLinesDifference(original, target.ToArray());

            linesDifference.Should().HaveCount(original.Length);
            linesDifference[3].Type.Should().Be(DifferenceType.Deleted);
            linesDifference[4].Type.Should().Be(DifferenceType.Deleted);
            linesDifference[5].Type.Should().Be(DifferenceType.Deleted);
            linesDifference[6].Type.Should().Be(DifferenceType.Deleted);
            linesDifference[7].Type.Should().Be(DifferenceType.Deleted);
            linesDifference.Where(x => x.Type == DifferenceType.Equals).Should().HaveSameCount(target);
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

            var linesDifference = Diff.GetLinesDifference(original, target.ToArray());

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
        }
    }
}