﻿using System;
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
        public void should_return_empty_diff_when_two_lines_is_empty()
        {
            var difference = new DifferenceMap(new string[0], new string[0]);
            difference.Ranges.Should().BeEmpty();
        }

        [Test]
        public void should_mark_all_target_lines_as_new_when_original_is_empty()
        {
            var difference = new DifferenceMap(new string[0], StringGenerator.RandomStrings(count: 5, enableWhitespaces: true));

            difference.Ranges.Should().HaveCount(1);
            difference.Ranges[0].From.Should().Be(0);
            difference.Ranges[0].To.Should().Be(4);
            difference.Ranges[0].DifferenceType.Should().Be(DifferenceType.Added);
        }

        [Test]
        public void should_mark_all_original_lines_as_deleted_when_target_is_empty()
        {
            var original = StringGenerator.RandomStrings(count: 5, enableWhitespaces: true);
            var target = new string[0];

            var difference = new DifferenceMap(original, target);

            difference.Ranges.Should().HaveCount(1);
            difference.Ranges[0].From.Should().Be(0);
            difference.Ranges[0].To.Should().Be(4);
            difference.Ranges[0].DifferenceType.Should().Be(DifferenceType.Deleted);
        }

        [Test]
        public void should_mark_all_target_lines_as_equals_when_target_has_no_changes()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToArray();

            var difference = new DifferenceMap(original, target);

            difference.Ranges.Should().HaveCount(0);
        }

        [Test]
        public void should_find_all_new_lines()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToList();
            target.InsertRange(3, StringGenerator.RandomStrings(count: 3, enableWhitespaces: true));
            target.InsertRange(9, StringGenerator.RandomStrings(count: 2, enableWhitespaces: true));

            var difference = new DifferenceMap(original, target.ToArray());

            difference.Ranges.Should().HaveCount(2);

            var firstDiff  = difference.Ranges[0];
            firstDiff.From.Should().Be(3);
            firstDiff.To.Should().Be(5);
            firstDiff.DifferenceType.Should().Be(DifferenceType.Added);

            var secondDiff = difference.Ranges[1];
            secondDiff.From.Should().Be(9);
            secondDiff.To.Should().Be(10);
            secondDiff.DifferenceType.Should().Be(DifferenceType.Added);
        }

        [Test]
        public void should_fill_all_diffs_with_new_lines()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToList();
            target.InsertRange(3, StringGenerator.RandomStrings(count: 3, enableWhitespaces: true));
            target.InsertRange(9, StringGenerator.RandomStrings(count: 2, enableWhitespaces: true));

            var difference = new DifferenceMap(original, target.ToArray());

            difference.Ranges.Should()
                      .NotBeEmpty().And
                      .OnlyContain(r => r.AddedLines.Count() == r.Length);
        }

        [Test]
        public void should_find_all_removed_lines()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToList();
            target.RemoveRange(6, 3);
            target.RemoveRange(3, 2);

            var difference = new DifferenceMap(original, target.ToArray());

            difference.Ranges.Should().HaveCount(2);

            var firstDiff = difference.Ranges[0];
            firstDiff.From.Should().Be(3);
            firstDiff.To.Should().Be(4);
            firstDiff.DifferenceType.Should().Be(DifferenceType.Deleted);

            var secondDiff = difference.Ranges[1];
            secondDiff.From.Should().Be(6);
            secondDiff.To.Should().Be(8);
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

            difference.Ranges.Should().HaveCount(4);

            var diff1 = difference.Ranges[0];
            diff1.From.Should().Be(3);
            diff1.To.Should().Be(4);
            diff1.DifferenceType.Should().Be(DifferenceType.Deleted);

            var diff2 = difference.Ranges[1];
            diff2.From.Should().Be(3);
            diff2.To.Should().Be(4);
            diff2.DifferenceType.Should().Be(DifferenceType.Added);

            var diff3 = difference.Ranges[2];
            diff3.From.Should().Be(6);
            diff3.To.Should().Be(7);
            diff3.DifferenceType.Should().Be(DifferenceType.Deleted);

            var diff4 = difference.Ranges[3];
            diff4.From.Should().Be(6);
            diff4.To.Should().Be(7);
            diff4.DifferenceType.Should().Be(DifferenceType.Added);
        }

        [Test]
        public void GetDiffPerLine_should_return_diffs_per_line_lines()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToList();

            target[3] = StringGenerator.RandomString(enableWhitespaces: true);
            target[4] = StringGenerator.RandomString(enableWhitespaces: true);

            target[6] = StringGenerator.RandomString(enableWhitespaces: true);
            target[7] = StringGenerator.RandomString(enableWhitespaces: true);

            var difference = new DifferenceMap(original, target.ToArray());
            var linesDifference = difference.GetDiffPerLine();

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

        [Test]
        public void Merge_should_merge_two_diffs()
        {
            var original = StringGenerator.Range(from: 1, count: 10, prefix: "line ").ToArray();
            var target1 = original.ToList();
            var target2 = original.ToList();

            target1[3] = "replaced line 3";
            target1[4] = "replaced line 4";
            target1.Insert(5, "inserted line 5");

            target2[6] = "replaced line 6";
            target2[7] = "replaced line 7";
            target2.RemoveRange(8, 1);

            var diff1 = new DifferenceMap(original, target1.ToArray());
            var diff2 = new DifferenceMap(original, target2.ToArray());

            var mergedDiff = DifferenceMap.Merge(diff1, diff2);

            mergedDiff.Ranges.Should().HaveCount(diff1.Ranges.Length + diff2.Ranges.Length);
            mergedDiff.Ranges.Should().Contain(diff1.Ranges);
            mergedDiff.Ranges.Should().Contain(diff2.Ranges);
        }

        [Test]
        public void Merge_should_fail_when_diffs_assigned_with_different_original_sequence()
        {
            var original = StringGenerator.Range(from: 1, count: 10, prefix: "line ");
            var original2 = StringGenerator.Range(from: 1, count: 10, prefix: "line ");
            var target1 = original.ToList();
            var target2 = original2.ToList();

            var diff1 = new DifferenceMap(original.ToArray(), target1.ToArray());
            var diff2 = new DifferenceMap(original.ToArray(), target2.ToArray());

            Action action = () => DifferenceMap.Merge(diff1, diff2);

            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Merge_should_have_diffs_in_right_order()
        {
            var original = StringGenerator.Range(from: 1, count: 10, prefix: "line ");
            var target1 = original.ToList();
            var target2 = original.ToList();

            target1[3] = "replaced line 3";
            target1[4] = "replaced line 4";
            target1.Insert(5, "inserted line 5");

            target2[6] = "replaced line 6";
            target2[7] = "replaced line 7";
            target2.RemoveRange(8, 1);

            var diff1 = new DifferenceMap(original, target1.ToArray());
            var diff2 = new DifferenceMap(original, target2.ToArray());

            var mergedDiff = DifferenceMap.Merge(diff1, diff2);

            var ranges = mergedDiff.Ranges;
            ranges[0].Should().Be(diff1.Ranges[0]);
            ranges[1].Should().Be(diff1.Ranges[1]);

            ranges[2].Should().Be(diff2.Ranges[0]);
            ranges[3].Should().Be(diff2.Ranges[1]);
        }

        [Test]
        public void Merge_should_find_conflicts()
        {
            var original = StringGenerator.Range(from: 1, count: 10, prefix: "line ").ToArray();
            var target1 = original.ToList();
            var target2 = original.ToList();

            target1[3] = "replaced line 3";
            target1[4] = "replaced line 4";
            target1.Insert(5, "inserted line 5");

            target2[5] = "replaced line 5";
            target2[7] = "replaced line 7";
            target2.RemoveRange(8, 1);

            var diff1 = new DifferenceMap(original, target1.ToArray());
            var diff2 = new DifferenceMap(original, target2.ToArray());

            var mergedDiff = DifferenceMap.Merge(diff1, diff2);

            mergedDiff.HasConflicts.Should().BeTrue();
            mergedDiff.Ranges.Where(x => x.HasConflict).Should().HaveCount(1);
        }

        [Test]
        public void Patch_should_apply_diff_changes_to_original()
        {
            var original = StringGenerator.Range(from: 1, count: 10, prefix: "line ").ToArray();
            var target = original.ToList();

            target[3] = "replaced line 3";
            target[4] = "replaced line 4";
            target.RemoveRange(7, 2);
            target.Insert(5, "inserted line 5");

            var diff = new DifferenceMap(original, target.ToArray());
            var patched = diff.PatchOriginal();

            patched.Should().BeEquivalentTo(target);
        }
    }
}