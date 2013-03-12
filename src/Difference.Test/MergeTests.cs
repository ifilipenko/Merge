using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Merge.Test
{
    [TestFixture]
    public class MergeTests
    {
        [TestCase("original")]
        [TestCase("first")]
        [TestCase("second")]
        public void should_fail_when_file_is_null(string file)
        {
            var original = file == "original" ? null : new string[0];
            var file1 = file == "first" ? null : new string[0];
            var file2 = file == "second" ? null : new string[0];

            Action action = () => Merge.Execute(original, file1, file2);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void should_merge_not_crossed_changes()
        {
            var original = StringGenerator.GenerateStrings(count: 10, enableWhitespaces: true);
            var target1 = original.ToList();
            var target2 = original.ToList();

            target1[3] = StringGenerator.GenerateString(enableWhitespaces: true);
            target1[4] = StringGenerator.GenerateString(enableWhitespaces: true);
            target1.InsertRange(5, StringGenerator.GenerateStrings(1, enableWhitespaces: true));

            target2[6] = StringGenerator.GenerateString(enableWhitespaces: true);
            target2[7] = StringGenerator.GenerateString(enableWhitespaces: true);
            target2.RemoveRange(8, 1);

            var expected = original.ToList();
            expected[3] = target1[3];
            expected[4] = target2[4];
            expected[6] = target2[6];
            expected[7] = target2[7];
            expected.RemoveRange(8, 1);
            expected.Insert(5, target1[5]);

            var mergedText = Merge.Execute(original, target1.ToArray(), target2.ToArray());
            var mergedLines = mergedText.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            mergedLines.Should().BeEquivalentTo(expected);
        }
    }
}