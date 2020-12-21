using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace DotNext.Buffers
{
    using static IO.StreamSource;

    [ExcludeFromCodeCoverage]
    public sealed class SparseBufferWriterTests : Test
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void WriteSequence(bool copyMemory)
        {
            using var writer = new SparseBufferWriter<byte>();
            var sequence = ToReadOnlySequence(new ReadOnlyMemory<byte>(RandomBytes(5000)), 1000);
            writer.Write(in sequence, copyMemory);
            Equal(sequence.ToArray(), writer.ToReadOnlySequence().ToArray());
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public static void StressTest(int totalSize)
        {
            using var writer = new SparseBufferWriter<byte>();
            using var output = writer.AsStream();
            var data = RandomBytes(2048);
            for (int remaining = totalSize, take; remaining > 0; remaining -= take)
            {
                take = Math.Min(remaining, data.Length);
                output.Write(data, 0, take);
                remaining -= take;
            }
        }
    }
}