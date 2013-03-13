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
            Action action = () => new Diff(null, new string[0]);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Should_fail_when_target_is_null()
        {
            Action action = () => new Diff(new string[0], null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void should_return_empty_diff_when_two_lines_is_empty()
        {
            var difference = new Diff(new string[0], new string[0]);
            difference.Ranges.Should().BeEmpty();
        }

        [Test]
        public void should_mark_all_target_lines_as_new_when_original_is_empty()
        {
            var difference = new Diff(new string[0], StringGenerator.RandomStrings(count: 5, enableWhitespaces: true));

            difference.Ranges.Should().HaveCount(1);
            difference.Ranges[0].From.Should().Be(0);
            difference.Ranges[0].To.Should().Be(4);
            difference.Ranges[0].DifferenceType.Should().Be(DifferenceType.Add);
        }

        [Test]
        public void should_mark_all_original_lines_as_deleted_when_target_is_empty()
        {
            var original = StringGenerator.RandomStrings(count: 5, enableWhitespaces: true);
            var target = new string[0];

            var difference = new Diff(original, target);

            difference.Ranges.Should().HaveCount(1);
            difference.Ranges[0].From.Should().Be(0);
            difference.Ranges[0].To.Should().Be(4);
            difference.Ranges[0].DifferenceType.Should().Be(DifferenceType.Delete);
        }

        [Test]
        public void should_mark_all_target_lines_as_equals_when_target_has_no_changes()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToArray();

            var difference = new Diff(original, target);

            difference.Ranges.Should().HaveCount(0);
        }

        [Test]
        public void should_find_all_new_lines()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToList();
            target.InsertRange(3, StringGenerator.RandomStrings(count: 3, enableWhitespaces: true));
            target.InsertRange(9, StringGenerator.RandomStrings(count: 2, enableWhitespaces: true));

            var difference = new Diff(original, target.ToArray());

            difference.Ranges.Should().HaveCount(2);

            var firstDiff  = difference.Ranges[0];
            firstDiff.From.Should().Be(3);
            firstDiff.To.Should().Be(5);
            firstDiff.DifferenceType.Should().Be(DifferenceType.Add);

            var secondDiff = difference.Ranges[1];
            secondDiff.From.Should().Be(9);
            secondDiff.To.Should().Be(10);
            secondDiff.DifferenceType.Should().Be(DifferenceType.Add);
        }

        [Test]
        public void should_fill_all_diffs_with_new_lines()
        {
            var original = StringGenerator.RandomStrings(count: 10, enableWhitespaces: true);
            var target = original.ToList();
            target.InsertRange(3, StringGenerator.RandomStrings(count: 3, enableWhitespaces: true));
            target.InsertRange(9, StringGenerator.RandomStrings(count: 2, enableWhitespaces: true));

            var difference = new Diff(original, target.ToArray());

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

            var difference = new Diff(original, target.ToArray());

            difference.Ranges.Should().HaveCount(2);

            var firstDiff = difference.Ranges[0];
            firstDiff.From.Should().Be(3);
            firstDiff.To.Should().Be(4);
            firstDiff.DifferenceType.Should().Be(DifferenceType.Delete);

            var secondDiff = difference.Ranges[1];
            secondDiff.From.Should().Be(6);
            secondDiff.To.Should().Be(8);
            secondDiff.DifferenceType.Should().Be(DifferenceType.Delete);
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

            var difference = new Diff(original, target.ToArray());

            difference.Ranges.Should().HaveCount(2);

            var diff1 = difference.Ranges[0];
            diff1.From.Should().Be(3);
            diff1.To.Should().Be(4);
            diff1.DifferenceType.Should().Be(DifferenceType.Replace);

            var diff2 = difference.Ranges[1];
            diff2.From.Should().Be(6);
            diff2.To.Should().Be(7);
            diff2.DifferenceType.Should().Be(DifferenceType.Replace);
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

            var difference = new Diff(original, target.ToArray());
            var linesDifference = difference.GetDiffPerLine();

            linesDifference.Should().HaveCount(original.Length + 4);
            linesDifference[3].Type.Should().Be(Difference.TypeEnum.Deleted);
            linesDifference[4].Type.Should().Be(Difference.TypeEnum.Deleted);
            linesDifference[5].Type.Should().Be(Difference.TypeEnum.Added);
            linesDifference[6].Type.Should().Be(Difference.TypeEnum.Added);

            linesDifference[8].Type.Should().Be(Difference.TypeEnum.Deleted);
            linesDifference[9].Type.Should().Be(Difference.TypeEnum.Deleted);
            linesDifference[10].Type.Should().Be(Difference.TypeEnum.Added);
            linesDifference[11].Type.Should().Be(Difference.TypeEnum.Added);
            linesDifference.Where(x => x.Type == Difference.TypeEnum.Equals).Should().HaveCount(6);
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

            var diff1 = new Diff(original, target1.ToArray());
            var diff2 = new Diff(original, target2.ToArray());

            var mergedDiff = Diff.Merge(diff1, diff2);

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

            var diff1 = new Diff(original.ToArray(), target1.ToArray());
            var diff2 = new Diff(original.ToArray(), target2.ToArray());

            Action action = () => Diff.Merge(diff1, diff2);

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

            var diff1 = new Diff(original, target1.ToArray());
            var diff2 = new Diff(original, target2.ToArray());

            var mergedDiff = Diff.Merge(diff1, diff2);

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

            target2[5] = "conflicted!!!";
            target2[7] = "replaced line 7";
            target2.RemoveRange(8, 1);

            var diff1 = new Diff(original, target1.ToArray());
            var diff2 = new Diff(original, target2.ToArray());

            var mergedDiff = Diff.Merge(diff1, diff2);

            mergedDiff.HasConflicts.Should().BeTrue();
            mergedDiff.Ranges.Where(x => x.HasConflict).Should().HaveCount(1);
        }

        [Test]
        public void Merge_should_cut_conflicted_part_as_separated_diff()
        {
            var original = StringGenerator.Range(from: 1, count: 10, prefix: "line ").ToArray();
            var target1 = original.ToList();
            var target2 = original.ToList();

            target1[3] = "replaced line 3";
            target1[4] = "replaced line 4";
            target1.Insert(5, "inserted line 5");

            target2[5] = "conflicted!!!";
            target2[7] = "replaced line 7";
            target2.RemoveRange(8, 1);

            var diff1 = new Diff(original, target1.ToArray());
            var diff2 = new Diff(original, target2.ToArray());

            var mergedDiff = Diff.Merge(diff1, diff2);

            mergedDiff.Ranges.Should().HaveCount(6);
            mergedDiff.Ranges[1].Length.Should().Be(2);
            mergedDiff.Ranges[2].HasConflict.Should().BeTrue();
            mergedDiff.Ranges[2].DifferenceType.Should().Be(DifferenceType.Delete);
            mergedDiff.Ranges[2].From.Should().Be(5);
            mergedDiff.Ranges[2].To.Should().Be(5);
            mergedDiff.Ranges[2].ConflictedWith.AddedLines.Single().Entry.Should().Be("inserted line 5");
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

            var diff = new Diff(original, target.ToArray());
            var patched = diff.PatchOriginal();

            patched.Should().BeEquivalentTo(target);
        }

        [Test]
        public void Patch_should_wrap_conflicted_Parts()
        {
            var original = StringGenerator.Range(from: 1, count: 10, prefix: "line ").ToArray();
            var target1 = original.ToList();
            var target2 = original.ToList();

            target1[3] = "replaced line 3";
            target1[4] = "replaced line 4";
            target1.Insert(5, "inserted line 5");

            target2[5] = "conflicted";
            target2[7] = "replaced line 7";
            target2.RemoveRange(8, 1);

            var diff1 = new Diff(original, target1.ToArray());
            var diff2 = new Diff(original, target2.ToArray());

            var mergedDiff = Diff.Merge(diff1, diff2);

            var patched = mergedDiff.PatchOriginal();

            patched[5].Should().Be("<<<");
            patched[6].Should().Be("inserted line 5");
            patched[7].Should().Be("---");
            patched[8].Should().Be("conflicted");
            patched[9].Should().Be(">>>");
        }
    }
}