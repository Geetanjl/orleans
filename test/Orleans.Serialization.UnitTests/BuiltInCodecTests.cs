using CsCheck;
using Orleans.Serialization.Buffers;
using Orleans.Serialization.Cloning;
using Orleans.Serialization.Codecs;
using Orleans.Serialization.Serializers;
using Orleans.Serialization.TestKit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Xunit;
using Microsoft.FSharp.Collections;
using Xunit.Abstractions;

namespace Orleans.Serialization.UnitTests
{
    internal static class CsCheckAdaptor
    {
        public static Action<Action<TValue>> ToValueProvider<TValue>(this Gen<TValue> gen) => value => gen.Sample(value);
    }

    [GenerateSerializer]
    public enum MyEnum : short
    {
        None,
        One,
        Two
    }

    public class EnumTests : FieldCodecTester<MyEnum, IFieldCodec<MyEnum>>
    {
        public EnumTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override IFieldCodec<MyEnum> CreateCodec() => ServiceProvider.GetRequiredService<ICodecProvider>().GetCodec<MyEnum>();
        protected override MyEnum CreateValue() => (MyEnum)Random.Next((int)MyEnum.None, (int)MyEnum.Two);
        protected override MyEnum[] TestValues => new[] { MyEnum.None, MyEnum.One, MyEnum.Two, (MyEnum)(-1), (MyEnum)10_000};
        protected override void Configure(ISerializerBuilder builder)
        {
            builder.Services.RemoveAll(typeof(IFieldCodec<MyEnum>));
        }

        protected override Action<Action<MyEnum>> ValueProvider => Gen.Int.Select(v => (MyEnum)v).ToValueProvider();
    }

    public class EnumCopierTests : CopierTester<MyEnum, IDeepCopier<MyEnum>>
    {
        public EnumCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override IDeepCopier<MyEnum> CreateCopier() => ServiceProvider.GetRequiredService<ICodecProvider>().GetDeepCopier<MyEnum>();
        protected override MyEnum CreateValue() => (MyEnum)Random.Next((int)MyEnum.None, (int)MyEnum.Two);
        protected override MyEnum[] TestValues => new[] { MyEnum.None, MyEnum.One, MyEnum.Two, (MyEnum)(-1), (MyEnum)10_000};
        protected override void Configure(ISerializerBuilder builder)
        {
            builder.Services.RemoveAll(typeof(IFieldCodec<MyEnum>));
        }

        protected override Action<Action<MyEnum>> ValueProvider => Gen.Int.Select(v => (MyEnum)v).ToValueProvider();
    }

    public class DayOfWeekTests : FieldCodecTester<DayOfWeek, IFieldCodec<DayOfWeek>>
    {
        public DayOfWeekTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override IFieldCodec<DayOfWeek> CreateCodec() => ServiceProvider.GetRequiredService<ICodecProvider>().GetCodec<DayOfWeek>();
        protected override DayOfWeek CreateValue() => (DayOfWeek)Random.Next((int)DayOfWeek.Sunday, (int)DayOfWeek.Saturday);
        protected override DayOfWeek[] TestValues => new[] { DayOfWeek.Monday, DayOfWeek.Sunday, (DayOfWeek)(-1), (DayOfWeek)10_000};

        protected override Action<Action<DayOfWeek>> ValueProvider => Gen.Int.Select(v => (DayOfWeek)v).ToValueProvider();
    }

    public class DayOfWeekCopierTests : CopierTester<DayOfWeek, IDeepCopier<DayOfWeek>>
    {
        public DayOfWeekCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override IDeepCopier<DayOfWeek> CreateCopier() => ServiceProvider.GetRequiredService<ICodecProvider>().GetDeepCopier<DayOfWeek>();
        protected override DayOfWeek CreateValue() => (DayOfWeek)Random.Next((int)DayOfWeek.Sunday, (int)DayOfWeek.Saturday);
        protected override DayOfWeek[] TestValues => new[] { DayOfWeek.Monday, DayOfWeek.Sunday, (DayOfWeek)(-1), (DayOfWeek)10_000};

        protected override Action<Action<DayOfWeek>> ValueProvider => Gen.Int.Select(v => (DayOfWeek)v).ToValueProvider();
    }

    public class NullableIntTests : FieldCodecTester<int?, IFieldCodec<int?>>
    {
        public NullableIntTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override IFieldCodec<int?> CreateCodec() => ServiceProvider.GetRequiredService<ICodecProvider>().GetCodec<int?>();
        protected override int? CreateValue() => TestValues[Random.Next(TestValues.Length)];
        protected override int?[] TestValues => new int?[] { null, 1, 2, -3 };
    }

    public class NullableIntCopierTests : CopierTester<int?, IDeepCopier<int?>>
    {
        public NullableIntCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override IDeepCopier<int?> CreateCopier() => ServiceProvider.GetRequiredService<ICodecProvider>().GetDeepCopier<int?>();
        protected override int? CreateValue() => TestValues[Random.Next(TestValues.Length)];
        protected override int?[] TestValues => new int?[] { null, 1, 2, -3 };
    }

    public class DateTimeTests : FieldCodecTester<DateTime, DateTimeCodec>
    {
        public DateTimeTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override DateTime CreateValue() => DateTime.UtcNow;
        protected override DateTime[] TestValues => new[] { DateTime.MinValue, DateTime.MaxValue, new DateTime(1970, 1, 1, 0, 0, 0) };
        protected override Action<Action<DateTime>> ValueProvider => Gen.DateTime.ToValueProvider();
    }

    public class DateTimeCopierTests : CopierTester<DateTime, IDeepCopier<DateTime>>
    {
        public DateTimeCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override DateTime CreateValue() => DateTime.UtcNow;
        protected override DateTime[] TestValues => new[] { DateTime.MinValue, DateTime.MaxValue, new DateTime(1970, 1, 1, 0, 0, 0) };
        protected override Action<Action<DateTime>> ValueProvider => Gen.DateTime.ToValueProvider();
    }

    public class DateOnlyTests : FieldCodecTester<DateOnly, DateOnlyCodec>
    {
        public DateOnlyTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override DateOnly CreateValue() => DateOnly.FromDateTime(DateTime.UtcNow);
        protected override DateOnly[] TestValues => new[] { DateOnly.MinValue, DateOnly.MaxValue, new DateOnly(1970, 1, 1), CreateValue() };
        protected override Action<Action<DateOnly>> ValueProvider => assert => Gen.Date.Sample(dt => assert(DateOnly.FromDateTime(dt)));
    }

    public class TimeOnlyTests : FieldCodecTester<TimeOnly, TimeOnlyCodec>
    {
        public TimeOnlyTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override TimeOnly CreateValue() => TimeOnly.FromDateTime(DateTime.UtcNow);
        protected override TimeOnly[] TestValues => new[] { TimeOnly.MinValue, TimeOnly.MaxValue, TimeOnly.FromTimeSpan(TimeSpan.Zero), CreateValue() };
        protected override Action<Action<TimeOnly>> ValueProvider => assert => Gen.Date.Sample(dt => assert(TimeOnly.FromDateTime(dt)));
    }

    public class DateOnlyCopierTests : CopierTester<DateOnly, IDeepCopier<DateOnly>>
    {
        public DateOnlyCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override DateOnly CreateValue() => DateOnly.FromDateTime(DateTime.UtcNow);
        protected override DateOnly[] TestValues => new[] { DateOnly.MinValue, DateOnly.MaxValue, new DateOnly(1970, 1, 1), CreateValue() };
        protected override Action<Action<DateOnly>> ValueProvider => assert => Gen.Date.Sample(dt => assert(DateOnly.FromDateTime(dt)));
    }

    public class TimeOnlyCopierTests : CopierTester<TimeOnly, IDeepCopier<TimeOnly>>
    {
        public TimeOnlyCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override TimeOnly CreateValue() => TimeOnly.FromDateTime(DateTime.UtcNow);
        protected override TimeOnly[] TestValues => new[] { TimeOnly.MinValue, TimeOnly.MaxValue, TimeOnly.FromTimeSpan(TimeSpan.Zero), CreateValue() };
        protected override Action<Action<TimeOnly>> ValueProvider => assert => Gen.Date.Sample(dt => assert(TimeOnly.FromDateTime(dt)));
    }

    public class TimeSpanTests : FieldCodecTester<TimeSpan, TimeSpanCodec>
    {
        public TimeSpanTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override TimeSpan CreateValue() => TimeSpan.FromMilliseconds(Guid.NewGuid().GetHashCode());
        protected override TimeSpan[] TestValues => new[] { TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.Zero, TimeSpan.FromSeconds(12345) };
        protected override Action<Action<TimeSpan>> ValueProvider => Gen.TimeSpan.ToValueProvider();
    }

    public class TimeSpanCopierTests : CopierTester<TimeSpan, IDeepCopier<TimeSpan>>
    {
        public TimeSpanCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override TimeSpan CreateValue() => TimeSpan.FromMilliseconds(Guid.NewGuid().GetHashCode());
        protected override TimeSpan[] TestValues => new[] { TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.Zero, TimeSpan.FromSeconds(12345) };
        protected override Action<Action<TimeSpan>> ValueProvider => Gen.TimeSpan.ToValueProvider();
    }

    public class DateTimeOffsetTests : FieldCodecTester<DateTimeOffset, DateTimeOffsetCodec>
    {
        public DateTimeOffsetTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override DateTimeOffset CreateValue() => DateTime.UtcNow;
        protected override DateTimeOffset[] TestValues => new[]
        {
            DateTimeOffset.MinValue,
            DateTimeOffset.MaxValue,
            new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0), TimeSpan.FromHours(11.5)),
            new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0), TimeSpan.FromHours(-11.5)),
        };

        protected override Action<Action<DateTimeOffset>> ValueProvider => Gen.DateTimeOffset.ToValueProvider();
    }

    public class DateTimeOffsetCopierTests : CopierTester<DateTimeOffset, IDeepCopier<DateTimeOffset>>
    {
        public DateTimeOffsetCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override DateTimeOffset CreateValue() => DateTime.UtcNow;
        protected override DateTimeOffset[] TestValues => new[]
        {
            DateTimeOffset.MinValue,
            DateTimeOffset.MaxValue,
            new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0), TimeSpan.FromHours(11.5)),
            new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0), TimeSpan.FromHours(-11.5)),
        };

        protected override Action<Action<DateTimeOffset>> ValueProvider => Gen.DateTimeOffset.ToValueProvider();
    }

    public class VersionTests : FieldCodecTester<Version, VersionCodec>
    {
        public VersionTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Version CreateValue() => new();
        protected override Version[] TestValues => new[]
        {
            new Version(),
            new Version(1, 2),
            new Version(1, 2, 3),
            new Version(1, 2, 3, 4),
            new Version("1.2"),
            new Version("1.2.3"),
            new Version("1.2.3.4")
        };

        protected override bool Equals(Version left, Version right) => left == right && (left is null || left.GetHashCode() == right.GetHashCode());
    }

    public class VersionCopierTests : CopierTester<Version, IDeepCopier<Version>>
    {
        public VersionCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Version CreateValue() => new();
        protected override Version[] TestValues => new[]
        {
            new Version(),
            new Version(1, 2),
            new Version(1, 2, 3),
            new Version(1, 2, 3, 4),
            new Version("1.2"),
            new Version("1.2.3"),
            new Version("1.2.3.4")
        };

        protected override bool Equals(Version left, Version right) => left == right && (left is null || left.GetHashCode() == right.GetHashCode());

        protected override bool IsImmutable => true;
    }

    public class BitVector32Tests: FieldCodecTester<BitVector32, BitVector32Codec>
    {
        public BitVector32Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override BitVector32 CreateValue() => new(Random.Next());

        protected override BitVector32[] TestValues => new[]
        {
            new BitVector32(0),
            new BitVector32(100),
            new BitVector32(-100),
            CreateValue(),
            CreateValue(),
            CreateValue()
        };

        protected override bool Equals(BitVector32 left, BitVector32 right) => left.Equals(right) && left.GetHashCode() == right.GetHashCode();
    }

    public class BitVector32CopierTests: CopierTester<BitVector32, IDeepCopier<BitVector32>>
    {
        public BitVector32CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override BitVector32 CreateValue() => new(Random.Next());

        protected override BitVector32[] TestValues => new[]
        {
            new BitVector32(0),
            new BitVector32(100),
            new BitVector32(-100),
            CreateValue(),
            CreateValue(),
            CreateValue()
        };

        protected override bool Equals(BitVector32 left, BitVector32 right) => left.Equals(right) && left.GetHashCode() == right.GetHashCode();
    }

    public class Tuple1Tests : FieldCodecTester<Tuple<string>, TupleCodec<string>>
    {
        public Tuple1Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string> CreateValue() => Tuple.Create(Guid.NewGuid().ToString());

        protected override Tuple<string>[] TestValues => new[]
        {
            null,
            Tuple.Create<string>(null),
            Tuple.Create<string>(string.Empty),
            Tuple.Create<string>("foobar")
        };
    }

    public class Tuple1CopierTests : CopierTester<Tuple<string>, TupleCopier<string>>
    {
        public Tuple1CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string> CreateValue() => Tuple.Create(Guid.NewGuid().ToString());

        protected override Tuple<string>[] TestValues => new[]
        {
            null,
            Tuple.Create<string>(null),
            Tuple.Create<string>(string.Empty),
            Tuple.Create<string>("foobar")
        };

        protected override bool IsImmutable => true;
    }

    public class Tuple2Tests : FieldCodecTester<Tuple<string, string>, TupleCodec<string, string>>
    {
        public Tuple2Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string> CreateValue() => Tuple.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        protected override Tuple<string, string>[] TestValues => new[]
        {
            null,
            Tuple.Create<string, string>(null, null),
            Tuple.Create<string, string>(string.Empty, "foo"),
            Tuple.Create<string, string>("foo", "bar"),
            Tuple.Create<string, string>("foo", "foo"),
        };
    }

    public class Tuple2CopierTests : CopierTester<Tuple<string, string>, TupleCopier<string, string>>
    {
        public Tuple2CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string> CreateValue() => Tuple.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        protected override Tuple<string, string>[] TestValues => new[]
        {
            null,
            Tuple.Create<string, string>(null, null),
            Tuple.Create<string, string>(string.Empty, "foo"),
            Tuple.Create<string, string>("foo", "bar"),
            Tuple.Create<string, string>("foo", "foo"),
        };

        protected override bool IsImmutable => true;
    }

    public class Tuple3Tests : FieldCodecTester<Tuple<string, string, string>, TupleCodec<string, string, string>>
    {
        public Tuple3Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string, string> CreateValue() => Tuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override Tuple<string, string, string>[] TestValues => new[]
        {
            null,
            Tuple.Create(default(string), default(string), default(string)),
            Tuple.Create(string.Empty, string.Empty, "foo"),
            Tuple.Create("foo", "bar", "baz"),
            Tuple.Create("foo", "foo", "foo")
        };
    }

    public class Tuple3CopierTests : CopierTester<Tuple<string, string, string>, TupleCopier<string, string, string>>
    {
        public Tuple3CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string, string> CreateValue() => Tuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override Tuple<string, string, string>[] TestValues => new[]
        {
            null,
            Tuple.Create(default(string), default(string), default(string)),
            Tuple.Create(string.Empty, string.Empty, "foo"),
            Tuple.Create("foo", "bar", "baz"),
            Tuple.Create("foo", "foo", "foo")
        };

        protected override bool IsImmutable => true;
    }

    public class Tuple4Tests : FieldCodecTester<Tuple<string, string, string, string>, TupleCodec<string, string, string, string>>
    {
        public Tuple4Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string, string, string> CreateValue() => Tuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override Tuple<string, string, string, string>[] TestValues => new[]
        {
            null,
            Tuple.Create(default(string), default(string), default(string), default(string)),
            Tuple.Create(string.Empty, string.Empty, string.Empty, "foo"),
            Tuple.Create("foo", "bar", "baz", "4"),
            Tuple.Create("foo", "foo", "foo", "foo")
        };
    }

    public class Tuple4CopierTests : CopierTester<Tuple<string, string, string, string>, TupleCopier<string, string, string, string>>
    {
        public Tuple4CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string, string, string> CreateValue() => Tuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override Tuple<string, string, string, string>[] TestValues => new[]
        {
            null,
            Tuple.Create(default(string), default(string), default(string), default(string)),
            Tuple.Create(string.Empty, string.Empty, string.Empty, "foo"),
            Tuple.Create("foo", "bar", "baz", "4"),
            Tuple.Create("foo", "foo", "foo", "foo")
        };

        protected override bool IsImmutable => true;
    }

    public class Tuple5Tests : FieldCodecTester<Tuple<string, string, string, string, string>, TupleCodec<string, string, string, string, string>>
    {
        public Tuple5Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string, string, string, string> CreateValue() => Tuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override Tuple<string, string, string, string, string>[] TestValues => new[]
        {
            null,
            Tuple.Create(default(string), default(string), default(string), default(string), default(string)),
            Tuple.Create(string.Empty, string.Empty, string.Empty,string.Empty, "foo"),
            Tuple.Create("foo", "bar", "baz", "4", "5"),
            Tuple.Create("foo", "foo", "foo", "foo", "foo")
        };
    }

    public class Tuple5CopierTests : CopierTester<Tuple<string, string, string, string, string>, TupleCopier<string, string, string, string, string>>
    {
        public Tuple5CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string, string, string, string> CreateValue() => Tuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override Tuple<string, string, string, string, string>[] TestValues => new[]
        {
            null,
            Tuple.Create(default(string), default(string), default(string), default(string), default(string)),
            Tuple.Create(string.Empty, string.Empty, string.Empty,string.Empty, "foo"),
            Tuple.Create("foo", "bar", "baz", "4", "5"),
            Tuple.Create("foo", "foo", "foo", "foo", "foo")
        };

        protected override bool IsImmutable => true;
    }

    public class Tuple6Tests : FieldCodecTester<Tuple<string, string,string, string, string, string>, TupleCodec<string, string, string, string, string, string>>
    {
        public Tuple6Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string, string, string, string, string> CreateValue() => Tuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override Tuple<string, string, string, string, string, string>[] TestValues => new[]
        {
            null,
            Tuple.Create(default(string), default(string), default(string), default(string), default(string), default(string)),
            Tuple.Create(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "foo"),
            Tuple.Create("foo", "bar", "baz", "4", "5", "6"),
            Tuple.Create("foo", "foo", "foo", "foo", "foo", "foo")
        };
    }

    public class Tuple6CopierTests : CopierTester<Tuple<string, string,string, string, string, string>, TupleCopier<string, string, string, string, string, string>>
    {
        public Tuple6CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string, string, string, string, string> CreateValue() => Tuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override Tuple<string, string, string, string, string, string>[] TestValues => new[]
        {
            null,
            Tuple.Create(default(string), default(string), default(string), default(string), default(string), default(string)),
            Tuple.Create(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "foo"),
            Tuple.Create("foo", "bar", "baz", "4", "5", "6"),
            Tuple.Create("foo", "foo", "foo", "foo", "foo", "foo")
        };

        protected override bool IsImmutable => true;
    }

    public class Tuple7Tests : FieldCodecTester<Tuple<string, string, string, string, string, string, string>, TupleCodec<string, string, string, string, string, string, string>>
    {
        public Tuple7Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string, string, string, string, string, string> CreateValue() => Tuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override Tuple<string, string, string, string, string, string, string>[] TestValues => new[]
        {
            null,
            Tuple.Create(default(string), default(string), default(string), default(string), default(string), default(string), default(string)),
            Tuple.Create(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "foo"),
            Tuple.Create("foo", "bar", "baz", "4", "5", "6", "7"),
            Tuple.Create("foo", "foo", "foo", "foo", "foo", "foo", "foo")
        };
    }

    public class Tuple7CopierTests : CopierTester<Tuple<string, string, string, string, string, string, string>, TupleCopier<string, string, string, string, string, string, string>>
    {
        public Tuple7CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string, string, string, string, string, string> CreateValue() => Tuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override Tuple<string, string, string, string, string, string, string>[] TestValues => new[]
        {
            null,
            Tuple.Create(default(string), default(string), default(string), default(string), default(string), default(string), default(string)),
            Tuple.Create(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "foo"),
            Tuple.Create("foo", "bar", "baz", "4", "5", "6", "7"),
            Tuple.Create("foo", "foo", "foo", "foo", "foo", "foo", "foo")
        };

        protected override bool IsImmutable => true;
    }

    public class Tuple8Tests : FieldCodecTester<Tuple<string, string, string, string, string, string, string, Tuple<string>>, TupleCodec<string, string, string, string, string, string, string, Tuple<string>>>
    {
        public Tuple8Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string, string, string, string, string, string, Tuple<string>> CreateValue() => new(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            new Tuple<string>(Guid.NewGuid().ToString()));

        protected override Tuple<string, string, string, string, string, string, string, Tuple<string>>[] TestValues => new[]
        {
            null,
            new Tuple<string, string, string, string, string, string, string, Tuple<string>>(default, default, default, default, default, default, default, new Tuple<string>(default)),
            new Tuple<string, string, string, string, string, string, string, Tuple<string>>(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "foo", Tuple.Create("foo")),
            new Tuple<string, string, string, string, string, string, string, Tuple<string>>("foo", "bar", "baz", "4", "5", "6", "7", Tuple.Create("8")),
            new Tuple<string, string, string, string, string, string, string, Tuple<string>>("foo", "foo", "foo", "foo", "foo", "foo", "foo", Tuple.Create("foo"))
        };
    }

    public class Tuple8CopierTests : CopierTester<Tuple<string, string, string, string, string, string, string, Tuple<string>>, TupleCopier<string, string, string, string, string, string, string, Tuple<string>>>
    {
        public Tuple8CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Tuple<string, string, string, string, string, string, string, Tuple<string>> CreateValue() => new(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            new Tuple<string>(Guid.NewGuid().ToString()));

        protected override Tuple<string, string, string, string, string, string, string, Tuple<string>>[] TestValues => new[]
        {
            null,
            new Tuple<string, string, string, string, string, string, string, Tuple<string>>(default, default, default, default, default, default, default, new Tuple<string>(default)),
            new Tuple<string, string, string, string, string, string, string, Tuple<string>>(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "foo", Tuple.Create("foo")),
            new Tuple<string, string, string, string, string, string, string, Tuple<string>>("foo", "bar", "baz", "4", "5", "6", "7", Tuple.Create("8")),
            new Tuple<string, string, string, string, string, string, string, Tuple<string>>("foo", "foo", "foo", "foo", "foo", "foo", "foo", Tuple.Create("foo"))
        };

        protected override bool IsImmutable => true;
    }

    public class ValueTuple1Tests : FieldCodecTester<ValueTuple<string>, ValueTupleCodec<string>>
    {
        public ValueTuple1Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string> CreateValue() => ValueTuple.Create(Guid.NewGuid().ToString());

        protected override ValueTuple<string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create<string>(null),
            ValueTuple.Create<string>(string.Empty),
            ValueTuple.Create<string>("foobar")
        };
    }

    public class ValueTuple1CopierTests : CopierTester<ValueTuple<string>, ValueTupleCopier<string>>
    {
        public ValueTuple1CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string> CreateValue() => ValueTuple.Create(Guid.NewGuid().ToString());

        protected override ValueTuple<string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create<string>(null),
            ValueTuple.Create<string>(string.Empty),
            ValueTuple.Create<string>("foobar")
        };
    }

    public class ValueTuple2Tests : FieldCodecTester<ValueTuple<string, string>, ValueTupleCodec<string, string>>
    {
        public ValueTuple2Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string> CreateValue() => ValueTuple.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        protected override ValueTuple<string, string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create<string, string>(null, null),
            ValueTuple.Create<string, string>(string.Empty, "foo"),
            ValueTuple.Create<string, string>("foo", "bar"),
            ValueTuple.Create<string, string>("foo", "foo"),
        };
    }

    public class ValueTuple2CopierTests : CopierTester<ValueTuple<string, string>, ValueTupleCopier<string, string>>
    {
        public ValueTuple2CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string> CreateValue() => ValueTuple.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        protected override ValueTuple<string, string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create<string, string>(null, null),
            ValueTuple.Create<string, string>(string.Empty, "foo"),
            ValueTuple.Create<string, string>("foo", "bar"),
            ValueTuple.Create<string, string>("foo", "foo"),
        };
    }

    public class ValueTuple3Tests : FieldCodecTester<ValueTuple<string, string, string>, ValueTupleCodec<string, string, string>>
    {
        public ValueTuple3Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string, string> CreateValue() => ValueTuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override ValueTuple<string, string, string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create(default(string), default(string), default(string)),
            ValueTuple.Create(string.Empty, string.Empty, "foo"),
            ValueTuple.Create("foo", "bar", "baz"),
            ValueTuple.Create("foo", "foo", "foo")
        };
    }

    public class ValueTuple3CopierTests : CopierTester<ValueTuple<string, string, string>, ValueTupleCopier<string, string, string>>
    {
        public ValueTuple3CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string, string> CreateValue() => ValueTuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override ValueTuple<string, string, string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create(default(string), default(string), default(string)),
            ValueTuple.Create(string.Empty, string.Empty, "foo"),
            ValueTuple.Create("foo", "bar", "baz"),
            ValueTuple.Create("foo", "foo", "foo")
        };
    }

    public class ValueTuple4Tests : FieldCodecTester<ValueTuple<string, string, string, string>, ValueTupleCodec<string, string, string, string>>
    {
        public ValueTuple4Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string, string, string> CreateValue() => ValueTuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override ValueTuple<string, string, string, string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create(default(string), default(string), default(string), default(string)),
            ValueTuple.Create(string.Empty, string.Empty, string.Empty, "foo"),
            ValueTuple.Create("foo", "bar", "baz", "4"),
            ValueTuple.Create("foo", "foo", "foo", "foo")
        };
    }

    public class ValueTuple4CopierTests : CopierTester<ValueTuple<string, string, string, string>, ValueTupleCopier<string, string, string, string>>
    {
        public ValueTuple4CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string, string, string> CreateValue() => ValueTuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override ValueTuple<string, string, string, string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create(default(string), default(string), default(string), default(string)),
            ValueTuple.Create(string.Empty, string.Empty, string.Empty, "foo"),
            ValueTuple.Create("foo", "bar", "baz", "4"),
            ValueTuple.Create("foo", "foo", "foo", "foo")
        };
    }

    public class ValueTuple5Tests : FieldCodecTester<ValueTuple<string, string, string, string, string>, ValueTupleCodec<string, string, string, string, string>>
    {
        public ValueTuple5Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string, string, string, string> CreateValue() => ValueTuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override ValueTuple<string, string, string, string, string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create(default(string), default(string), default(string), default(string), default(string)),
            ValueTuple.Create(string.Empty, string.Empty, string.Empty,string.Empty, "foo"),
            ValueTuple.Create("foo", "bar", "baz", "4", "5"),
            ValueTuple.Create("foo", "foo", "foo", "foo", "foo")
        };
    }

    public class ValueTuple5CopierTests : CopierTester<ValueTuple<string, string, string, string, string>, ValueTupleCopier<string, string, string, string, string>>
    {
        public ValueTuple5CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string, string, string, string> CreateValue() => ValueTuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override ValueTuple<string, string, string, string, string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create(default(string), default(string), default(string), default(string), default(string)),
            ValueTuple.Create(string.Empty, string.Empty, string.Empty,string.Empty, "foo"),
            ValueTuple.Create("foo", "bar", "baz", "4", "5"),
            ValueTuple.Create("foo", "foo", "foo", "foo", "foo")
        };
    }

    public class ValueTuple6Tests : FieldCodecTester<ValueTuple<string, string,string, string, string, string>, ValueTupleCodec<string, string, string, string, string, string>>
    {
        public ValueTuple6Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string, string, string, string, string> CreateValue() => ValueTuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override ValueTuple<string, string, string, string, string, string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create(default(string), default(string), default(string), default(string), default(string), default(string)),
            ValueTuple.Create(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "foo"),
            ValueTuple.Create("foo", "bar", "baz", "4", "5", "6"),
            ValueTuple.Create("foo", "foo", "foo", "foo", "foo", "foo")
        };
    }

    public class ValueTuple6CopierTests : CopierTester<ValueTuple<string, string,string, string, string, string>, ValueTupleCopier<string, string, string, string, string, string>>
    {
        public ValueTuple6CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string, string, string, string, string> CreateValue() => ValueTuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override ValueTuple<string, string, string, string, string, string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create(default(string), default(string), default(string), default(string), default(string), default(string)),
            ValueTuple.Create(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "foo"),
            ValueTuple.Create("foo", "bar", "baz", "4", "5", "6"),
            ValueTuple.Create("foo", "foo", "foo", "foo", "foo", "foo")
        };
    }

    public class ValueTuple7Tests : FieldCodecTester<ValueTuple<string, string, string, string, string, string, string>, ValueTupleCodec<string, string, string, string, string, string, string>>
    {
        public ValueTuple7Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string, string, string, string, string, string> CreateValue() => ValueTuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override ValueTuple<string, string, string, string, string, string, string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create(default(string), default(string), default(string), default(string), default(string), default(string), default(string)),
            ValueTuple.Create(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "foo"),
            ValueTuple.Create("foo", "bar", "baz", "4", "5", "6", "7"),
            ValueTuple.Create("foo", "foo", "foo", "foo", "foo", "foo", "foo")
        };
    }

    public class ValueTuple7opierTests : CopierTester<ValueTuple<string, string, string, string, string, string, string>, ValueTupleCopier<string, string, string, string, string, string, string>>
    {
        public ValueTuple7opierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string, string, string, string, string, string> CreateValue() => ValueTuple.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        protected override ValueTuple<string, string, string, string, string, string, string>[] TestValues => new[]
        {
            default,
            ValueTuple.Create(default(string), default(string), default(string), default(string), default(string), default(string), default(string)),
            ValueTuple.Create(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "foo"),
            ValueTuple.Create("foo", "bar", "baz", "4", "5", "6", "7"),
            ValueTuple.Create("foo", "foo", "foo", "foo", "foo", "foo", "foo")
        };
    }

    public class ValueTuple8Tests : FieldCodecTester<ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>>, ValueTupleCodec<string, string, string, string, string, string, string, ValueTuple<string>>>
    {
        public ValueTuple8Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>> CreateValue() => new(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            ValueTuple.Create(Guid.NewGuid().ToString()));

        protected override ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>>[] TestValues => new[]
        {
            default,
            new ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>>(default, default, default, default, default, default, default, ValueTuple.Create(default(string))),
            new ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>>(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "foo", ValueTuple.Create("foo")),
            new ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>>("foo", "bar", "baz", "4", "5", "6", "7", ValueTuple.Create("8")),
            new ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>>("foo", "foo", "foo", "foo", "foo", "foo", "foo", ValueTuple.Create("foo"))
        };
    }

    public class ValueTuple8CopierTests : CopierTester<ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>>, ValueTupleCopier<string, string, string, string, string, string, string, ValueTuple<string>>>
    {
        public ValueTuple8CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>> CreateValue() => new(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            ValueTuple.Create(Guid.NewGuid().ToString()));

        protected override ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>>[] TestValues => new[]
        {
            default,
            new ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>>(default, default, default, default, default, default, default, ValueTuple.Create(default(string))),
            new ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>>(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "foo", ValueTuple.Create("foo")),
            new ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>>("foo", "bar", "baz", "4", "5", "6", "7", ValueTuple.Create("8")),
            new ValueTuple<string, string, string, string, string, string, string, ValueTuple<string>>("foo", "foo", "foo", "foo", "foo", "foo", "foo", ValueTuple.Create("foo"))
        };
    }

    public class BoolCodecTests : FieldCodecTester<bool, BoolCodec>
    {
        public BoolCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override bool CreateValue() => true;
        protected override bool Equals(bool left, bool right) => left == right;
        protected override bool[] TestValues => new[] { false, true };
    }

    public class BoolCopierTests : CopierTester<bool, IDeepCopier<bool>>
    {
        public BoolCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override bool CreateValue() => true;
        protected override bool Equals(bool left, bool right) => left == right;
        protected override bool[] TestValues => new[] { false, true };
    }

    public class StringCodecTests : FieldCodecTester<string, StringCodec>
    {
        public StringCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override string CreateValue() => Guid.NewGuid().ToString();
        protected override bool Equals(string left, string right) => StringComparer.Ordinal.Equals(left, right);
        protected override string[] TestValues => new[] { null, string.Empty, new string('*', 6), new string('x', 4097), "Hello, World!" };
    }

    public class StringCopierTests : CopierTester<string, IDeepCopier<string>>
    {
        public StringCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override string CreateValue() => Guid.NewGuid().ToString();
        protected override bool Equals(string left, string right) => StringComparer.Ordinal.Equals(left, right);
        protected override string[] TestValues => new[] { null, string.Empty, new string('*', 6), new string('x', 4097), "Hello, World!" };

        protected override bool IsImmutable => true;
    }

    public class ObjectCodecTests : FieldCodecTester<object, ObjectCodec>
    {
        public ObjectCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override object CreateValue() => new();
        protected override bool Equals(object left, object right) => ReferenceEquals(left, right) || typeof(object) == left?.GetType() && typeof(object) == right?.GetType();
        protected override object[] TestValues => new[] { null, new object() };
    }

    public class ObjectCopierTests : CopierTester<object, ObjectCopier>
    {
        public ObjectCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override object CreateValue() => new();
        protected override bool Equals(object left, object right) => ReferenceEquals(left, right) || typeof(object) == left?.GetType() && typeof(object) == right?.GetType();
        protected override object[] TestValues => new[] { null, new object() };
        protected override bool IsImmutable => true;
    }

    public class ByteArrayCodecTests : FieldCodecTester<byte[], ByteArrayCodec>
    {
        public ByteArrayCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override byte[] CreateValue() => Guid.NewGuid().ToByteArray();

        protected override bool Equals(byte[] left, byte[] right) => ReferenceEquals(left, right) || left.SequenceEqual(right);

        protected override byte[][] TestValues => new[]
        {
            null,
            Array.Empty<byte>(),
            Enumerable.Range(0, 4097).Select(b => unchecked((byte)b)).ToArray(), CreateValue(),
        };
    }

    public class ByteArrayCopierTests : CopierTester<byte[], ByteArrayCopier>
    {
        public ByteArrayCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override byte[] CreateValue() => Guid.NewGuid().ToByteArray();

        protected override bool Equals(byte[] left, byte[] right) => ReferenceEquals(left, right) || left.SequenceEqual(right);

        protected override byte[][] TestValues => new[]
        {
            null,
            Array.Empty<byte>(),
            Enumerable.Range(0, 4097).Select(b => unchecked((byte)b)).ToArray(), CreateValue(),
        };
    }

    public class ArrayCodecTests : FieldCodecTester<int[], ArrayCodec<int>>
    {
        public ArrayCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override int[] CreateValue() => Enumerable.Range(0, Random.Next(120) + 50).Select(_ => Guid.NewGuid().GetHashCode()).ToArray();
        protected override bool Equals(int[] left, int[] right) => ReferenceEquals(left, right) || left.SequenceEqual(right);
        protected override int[][] TestValues => new[] { null, Array.Empty<int>(), CreateValue(), CreateValue(), CreateValue() };
    }

    public class ArrayCopierTests : CopierTester<int[], ArrayCopier<int>>
    {
        public ArrayCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override int[] CreateValue() => Enumerable.Range(0, Random.Next(120) + 50).Select(_ => Guid.NewGuid().GetHashCode()).ToArray();
        protected override bool Equals(int[] left, int[] right) => ReferenceEquals(left, right) || left.SequenceEqual(right);
        protected override int[][] TestValues => new[] { null, Array.Empty<int>(), CreateValue(), CreateValue(), CreateValue() };
    }

    public class UInt128CodecTests : FieldCodecTester<UInt128, UInt128Codec>
    {
        public UInt128CodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override UInt128 CreateValue() => new (unchecked((ulong)Random.NextInt64()), unchecked((ulong)Random.NextInt64()));

        protected override UInt128[] TestValues => new UInt128[]
        {
            0,
            1,
            (UInt128)byte.MaxValue - 1,
            byte.MaxValue,
            (UInt128)byte.MaxValue + 1,
            (UInt128)ushort.MaxValue - 1,
            ushort.MaxValue,
            (UInt128)ushort.MaxValue + 1,
            (UInt128)uint.MaxValue - 1,
            uint.MaxValue,
            (UInt128)uint.MaxValue + 1,
            UInt128.MaxValue,
        };

        protected override Action<Action<UInt128>> ValueProvider => assert => Gen.ULong.Select(Gen.ULong).Sample(value => assert(new (value.V0, value.V1)));
    }

    public class UInt128CopierTests : CopierTester<UInt128, IDeepCopier<UInt128>>
    {
        public UInt128CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override UInt128 CreateValue() => new (unchecked((ulong)Random.NextInt64()), unchecked((ulong)Random.NextInt64()));

        protected override UInt128[] TestValues => new UInt128[]
        {
            0,
            1,
            (UInt128)byte.MaxValue - 1,
            byte.MaxValue,
            (UInt128)byte.MaxValue + 1,
            (UInt128)ushort.MaxValue - 1,
            ushort.MaxValue,
            (UInt128)ushort.MaxValue + 1,
            (UInt128)uint.MaxValue - 1,
            uint.MaxValue,
            (UInt128)uint.MaxValue + 1,
            UInt128.MaxValue,
        };

        protected override Action<Action<UInt128>> ValueProvider => assert => Gen.ULong.Select(Gen.ULong).Sample(value => assert(new (value.V0, value.V1)));
    }

    public class UInt64CodecTests : FieldCodecTester<ulong, UInt64Codec>
    {
        public UInt64CodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ulong CreateValue()
        {
            var msb = (ulong)Guid.NewGuid().GetHashCode() << 32;
            var lsb = (ulong)Guid.NewGuid().GetHashCode();
            return msb | lsb;
        }

        protected override ulong[] TestValues => new ulong[]
        {
            0,
            1,
            (ulong)byte.MaxValue - 1,
            byte.MaxValue,
            (ulong)byte.MaxValue + 1,
            (ulong)ushort.MaxValue - 1,
            ushort.MaxValue,
            (ulong)ushort.MaxValue + 1,
            (ulong)uint.MaxValue - 1,
            uint.MaxValue,
            (ulong)uint.MaxValue + 1,
            ulong.MaxValue,
        }
        .Concat(Enumerable.Range(1, 63).Select(i => 1ul << i))
        .ToArray();


        protected override Action<Action<ulong>> ValueProvider => Gen.ULong.ToValueProvider();
    }

    public class UInt64CopierTests : CopierTester<ulong, IDeepCopier<ulong>>
    {
        public UInt64CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ulong CreateValue()
        {
            var msb = (ulong)Guid.NewGuid().GetHashCode() << 32;
            var lsb = (ulong)Guid.NewGuid().GetHashCode();
            return msb | lsb;
        }

        protected override ulong[] TestValues => new ulong[]
        {
            0,
            1,
            (ulong)byte.MaxValue - 1,
            byte.MaxValue,
            (ulong)byte.MaxValue + 1,
            (ulong)ushort.MaxValue - 1,
            ushort.MaxValue,
            (ulong)ushort.MaxValue + 1,
            (ulong)uint.MaxValue - 1,
            uint.MaxValue,
            (ulong)uint.MaxValue + 1,
            ulong.MaxValue,
        };

        protected override Action<Action<ulong>> ValueProvider => Gen.ULong.ToValueProvider();
    }

    public class UInt32CodecTests : FieldCodecTester<uint, UInt32Codec>
    {
        public UInt32CodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override uint CreateValue() => (uint)Guid.NewGuid().GetHashCode();

        protected override uint[] TestValues => new uint[]
        {
            0,
            1,
            (uint)byte.MaxValue - 1,
            byte.MaxValue,
            (uint)byte.MaxValue + 1,
            (uint)ushort.MaxValue - 1,
            ushort.MaxValue,
            (uint)ushort.MaxValue + 1,
            uint.MaxValue,
        }
        .Concat(Enumerable.Range(1, 31).Select(i => 1u << i))
        .ToArray();

        protected override Action<Action<uint>> ValueProvider => Gen.UInt.ToValueProvider();
    }

    public class UInt32CopierTests : CopierTester<uint, IDeepCopier<uint>>
    {
        public UInt32CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override uint CreateValue() => (uint)Guid.NewGuid().GetHashCode();

        protected override uint[] TestValues => new uint[]
        {
            0,
            1,
            (uint)byte.MaxValue - 1,
            byte.MaxValue,
            (uint)byte.MaxValue + 1,
            (uint)ushort.MaxValue - 1,
            ushort.MaxValue,
            (uint)ushort.MaxValue + 1,
            uint.MaxValue,
        };

        protected override Action<Action<uint>> ValueProvider => Gen.UInt.ToValueProvider();
    }

    public class UInt16CodecTests : FieldCodecTester<ushort, UInt16Codec>
    {
        public UInt16CodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ushort CreateValue() => (ushort)Guid.NewGuid().GetHashCode();
        protected override ushort[] TestValues => new ushort[]
        {
            0,
            1,
            byte.MaxValue - 1,
            byte.MaxValue,
            byte.MaxValue + 1,
            ushort.MaxValue - 1,
            ushort.MaxValue,
        };

        protected override Action<Action<ushort>> ValueProvider => Gen.UShort.ToValueProvider();
    }

    public class UInt16CopierTests : CopierTester<ushort, IDeepCopier<ushort>>
    {
        public UInt16CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ushort CreateValue() => (ushort)Guid.NewGuid().GetHashCode();
        protected override ushort[] TestValues => new ushort[]
        {
            0,
            1,
            byte.MaxValue - 1,
            byte.MaxValue,
            byte.MaxValue + 1,
            ushort.MaxValue - 1,
            ushort.MaxValue,
        };

        protected override Action<Action<ushort>> ValueProvider => Gen.UShort.ToValueProvider();
    }

    public class ByteCodecTests : FieldCodecTester<byte, ByteCodec>
    {
        public ByteCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override byte CreateValue() => (byte)Guid.NewGuid().GetHashCode();
        protected override byte[] TestValues => new byte[] { 0, 1, byte.MaxValue - 1, byte.MaxValue };

        protected override Action<Action<byte>> ValueProvider => Gen.Byte.ToValueProvider();
    }

    public class ByteCopierTests : CopierTester<byte, IDeepCopier<byte>>
    {
        public ByteCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override byte CreateValue() => (byte)Guid.NewGuid().GetHashCode();
        protected override byte[] TestValues => new byte[] { 0, 1, byte.MaxValue - 1, byte.MaxValue };

        protected override Action<Action<byte>> ValueProvider => Gen.Byte.ToValueProvider();
    }

    public class Int128CodecTests : FieldCodecTester<Int128, Int128Codec>
    {
        public Int128CodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Int128 CreateValue() => new (unchecked((ulong)Random.NextInt64()), unchecked((ulong)Random.NextInt64()));

        protected override Int128[] TestValues => new Int128[]
        {
            0,
            1,
            (Int128)byte.MaxValue - 1,
            byte.MaxValue,
            (Int128)byte.MaxValue + 1,
            (Int128)ushort.MaxValue - 1,
            ushort.MaxValue,
            (Int128)ushort.MaxValue + 1,
            (Int128)uint.MaxValue - 1,
            uint.MaxValue,
            (Int128)uint.MaxValue + 1,
            Int128.MaxValue,
        };

        protected override Action<Action<Int128>> ValueProvider => assert => Gen.ULong.Select(Gen.ULong).Sample(value => assert(new (value.V0, value.V1)));
    }

    public class Int128CopierTests : CopierTester<Int128, IDeepCopier<Int128>>
    {
        public Int128CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Int128 CreateValue() => new Int128(unchecked((ulong)Random.NextInt64()), unchecked((ulong)Random.NextInt64()));

        protected override Int128[] TestValues => new Int128[]
        {
            0,
            1,
            (Int128)byte.MaxValue - 1,
            byte.MaxValue,
            (Int128)byte.MaxValue + 1,
            (Int128)ushort.MaxValue - 1,
            ushort.MaxValue,
            (Int128)ushort.MaxValue + 1,
            (Int128)uint.MaxValue - 1,
            uint.MaxValue,
            (Int128)uint.MaxValue + 1,
            Int128.MaxValue,
        };

        protected override Action<Action<Int128>> ValueProvider => assert => Gen.ULong.Select(Gen.ULong).Sample(value => assert(new (value.V0, value.V1)));
    }

    public class Int64CodecTests : FieldCodecTester<long, Int64Codec>
    {
        public Int64CodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override long CreateValue()
        {
            var msb = (ulong)Guid.NewGuid().GetHashCode() << 32;
            var lsb = (ulong)Guid.NewGuid().GetHashCode();
            return (long)(msb | lsb);
        }

        protected override long[] TestValues => new[]
        {
            long.MinValue,
            -1,
            0,
            1,
            (long)sbyte.MaxValue - 1,
            sbyte.MaxValue,
            (long)sbyte.MaxValue + 1,
            (long)short.MaxValue - 1,
            short.MaxValue,
            (long)short.MaxValue + 1,
            (long)int.MaxValue - 1,
            int.MaxValue,
            (long)int.MaxValue + 1,
            long.MaxValue,
        };

        protected override Action<Action<long>> ValueProvider => Gen.Long.ToValueProvider();
    }

    public class Int64CopierTests : CopierTester<long, IDeepCopier<long>>
    {
        public Int64CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override long CreateValue()
        {
            var msb = (ulong)Guid.NewGuid().GetHashCode() << 32;
            var lsb = (ulong)Guid.NewGuid().GetHashCode();
            return (long)(msb | lsb);
        }

        protected override long[] TestValues => new[]
        {
            long.MinValue,
            -1,
            0,
            1,
            (long)sbyte.MaxValue - 1,
            sbyte.MaxValue,
            (long)sbyte.MaxValue + 1,
            (long)short.MaxValue - 1,
            short.MaxValue,
            (long)short.MaxValue + 1,
            (long)int.MaxValue - 1,
            int.MaxValue,
            (long)int.MaxValue + 1,
            long.MaxValue,
        };

        protected override Action<Action<long>> ValueProvider => Gen.Long.ToValueProvider();
    }

    public class Int32CodecTests : FieldCodecTester<int, Int32Codec>
    {
        public Int32CodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override int CreateValue() => Guid.NewGuid().GetHashCode();

        protected override int[] TestValues => new[]
        {
            int.MinValue,
            -1,
            0,
            1,
            sbyte.MaxValue - 1,
            sbyte.MaxValue,
            sbyte.MaxValue + 1,
            short.MaxValue - 1,
            short.MaxValue,
            short.MaxValue + 1,
            int.MaxValue - 1,
            int.MaxValue,
        };

        protected override Action<Action<int>> ValueProvider => Gen.Int.ToValueProvider();

        [Fact]
        public void CanRoundTripViaSerializer_WriteReadByteByByte()
        {
            var serializer = ServiceProvider.GetRequiredService<Serializer<int>>();

            foreach (var original in TestValues)
            {
                var buffer = new TestMultiSegmentBufferWriter(maxAllocationSize: 8);

                using var writerSession = SessionPool.GetSession();
                var writer = Writer.Create(buffer, writerSession);
                for (var i = 0; i < 5; i++)
                {
                    serializer.Serialize(original, ref writer);
                }

                writer.Commit();
                using var readerSession = SessionPool.GetSession();
                var reader = Reader.Create(buffer.GetReadOnlySequence(maxSegmentSize: 1), readerSession);
                for (var i = 0; i < 5; i++)
                {
                    var deserialized = serializer.Deserialize(ref reader);

                    Assert.True(Equals(original, deserialized), $"Deserialized value \"{deserialized}\" must equal original value \"{original}\"");
                }
            }
        }
    }

    public class Int32CopierTests : CopierTester<int, IDeepCopier<int>>
    {
        public Int32CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override int CreateValue() => Guid.NewGuid().GetHashCode();

        protected override int[] TestValues => new[]
        {
            int.MinValue,
            -1,
            0,
            1,
            sbyte.MaxValue - 1,
            sbyte.MaxValue,
            sbyte.MaxValue + 1,
            short.MaxValue - 1,
            short.MaxValue,
            short.MaxValue + 1,
            int.MaxValue - 1,
            int.MaxValue,
        };

        protected override Action<Action<int>> ValueProvider => Gen.Int.ToValueProvider();
    }

    public class Int16CodecTests : FieldCodecTester<short, Int16Codec>
    {
        public Int16CodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override short CreateValue() => (short)Guid.NewGuid().GetHashCode();

        protected override short[] TestValues => new short[]
        {
            short.MinValue,
            -1,
            0,
            1,
            sbyte.MaxValue - 1,
            sbyte.MaxValue,
            sbyte.MaxValue + 1,
            short.MaxValue - 1,
            short.MaxValue
        };

        protected override Action<Action<short>> ValueProvider => Gen.Short.ToValueProvider();
    }

    public class Int16CopierTests : CopierTester<short, IDeepCopier<short>>
    {
        public Int16CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override short CreateValue() => (short)Guid.NewGuid().GetHashCode();

        protected override short[] TestValues => new short[]
        {
            short.MinValue,
            -1,
            0,
            1,
            sbyte.MaxValue - 1,
            sbyte.MaxValue,
            sbyte.MaxValue + 1,
            short.MaxValue - 1,
            short.MaxValue
        };

        protected override Action<Action<short>> ValueProvider => Gen.Short.ToValueProvider();
    }

    public class SByteCodecTests : FieldCodecTester<sbyte, SByteCodec>
    {
        public SByteCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override sbyte CreateValue() => (sbyte)Guid.NewGuid().GetHashCode();

        protected override sbyte[] TestValues => new sbyte[]
        {
            sbyte.MinValue,
            -1,
            0,
            1,
            sbyte.MaxValue - 1,
            sbyte.MaxValue
        };

        protected override Action<Action<sbyte>> ValueProvider => Gen.SByte.ToValueProvider();
    }

    public class SByteCopierTests : CopierTester<sbyte, IDeepCopier<sbyte>>
    {
        public SByteCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override sbyte CreateValue() => (sbyte)Guid.NewGuid().GetHashCode();

        protected override sbyte[] TestValues => new sbyte[]
        {
            sbyte.MinValue,
            -1,
            0,
            1,
            sbyte.MaxValue - 1,
            sbyte.MaxValue
        };

        protected override Action<Action<sbyte>> ValueProvider => Gen.SByte.ToValueProvider();
    }

    public class CharCodecTests : FieldCodecTester<char, CharCodec>
    {
        private int _createValueCount;

        public CharCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override char CreateValue() => (char)('!' + _createValueCount++ % ('~' - '!'));
        protected override char[] TestValues => new[]
        {
            (char)0,
            (char)1,
            (char)(byte.MaxValue - 1),
            (char)byte.MaxValue,
            (char)(byte.MaxValue + 1),
            (char)(ushort.MaxValue - 1),
            (char)ushort.MaxValue,
        };

        protected override Action<Action<char>> ValueProvider => Gen.Char.ToValueProvider();
    }

    public class CharCopierTests : CopierTester<char, IDeepCopier<char>>
    {
        private int _createValueCount;

        public CharCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override char CreateValue() => (char)('!' + _createValueCount++ % ('~' - '!'));
        protected override char[] TestValues => new[]
        {
            (char)0,
            (char)1,
            (char)(byte.MaxValue - 1),
            (char)byte.MaxValue,
            (char)(byte.MaxValue + 1),
            (char)(ushort.MaxValue - 1),
            (char)ushort.MaxValue,
        };

        protected override Action<Action<char>> ValueProvider => Gen.Char.ToValueProvider();
    }

    public class GuidCodecTests : FieldCodecTester<Guid, GuidCodec>
    {
        public GuidCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Guid CreateValue() => Guid.NewGuid();
        protected override Guid[] TestValues => new[]
        {
            Guid.Empty,
            Guid.Parse("4DEBD074-5DBB-45F6-ACB7-ED97D2AEE02F"),
            Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")
        };

        protected override Action<Action<Guid>> ValueProvider => Gen.Guid.ToValueProvider();
    }

    public class GuidCopierTests : CopierTester<Guid, IDeepCopier<Guid>>
    {
        public GuidCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Guid CreateValue() => Guid.NewGuid();
        protected override Guid[] TestValues => new[]
        {
            Guid.Empty,
            Guid.Parse("4DEBD074-5DBB-45F6-ACB7-ED97D2AEE02F"),
            Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")
        };

        protected override Action<Action<Guid>> ValueProvider => Gen.Guid.ToValueProvider();
    }

    public class TypeCodecTests : FieldCodecTester<Type, TypeSerializerCodec>
    {
        private readonly Type[] _values =
        {
            null,
            typeof(Dictionary<Guid, List<string>>),
            typeof(Type).MakeByRefType(),
            typeof(Guid),
            typeof(int).MakePointerType(),
            typeof(string[]),
            typeof(string[,]),
            typeof(string[,]).MakePointerType(),
            typeof(string[,]).MakeByRefType(),
            typeof(Dictionary<,>),
            typeof(List<>),
            typeof(string)
        };

        private int _valueIndex;

        public TypeCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Type CreateValue() => _values[_valueIndex++ % _values.Length];
        protected override Type[] TestValues => _values;
    }

    public class TypeCopierTests : CopierTester<Type, IDeepCopier<Type>>
    {
        private readonly Type[] _values =
        {
            null,
            typeof(Dictionary<Guid, List<string>>),
            typeof(Type).MakeByRefType(),
            typeof(Guid),
            typeof(int).MakePointerType(),
            typeof(string[]),
            typeof(string[,]),
            typeof(string[,]).MakePointerType(),
            typeof(string[,]).MakeByRefType(),
            typeof(Dictionary<,>),
            typeof(List<>),
            typeof(string)
        };

        private int _valueIndex;

        public TypeCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Type CreateValue() => _values[_valueIndex++ % _values.Length];
        protected override Type[] TestValues => _values;

        protected override bool IsImmutable => true;
    }

    public class FloatCodecTests : FieldCodecTester<float, FloatCodec>
    {
        public FloatCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override float CreateValue() => float.MaxValue * (float)Random.NextDouble() * Math.Sign(Guid.NewGuid().GetHashCode());
        protected override float[] TestValues => new[] { float.MinValue, 0, 1.0f, float.MaxValue };

        protected override Action<Action<float>> ValueProvider => Gen.Float.ToValueProvider();
    }

    public class FloatCopierTests : CopierTester<float, IDeepCopier<float>>
    {
        public FloatCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override float CreateValue() => float.MaxValue * (float)Random.NextDouble() * Math.Sign(Guid.NewGuid().GetHashCode());
        protected override float[] TestValues => new[] { float.MinValue, 0, 1.0f, float.MaxValue };

        protected override Action<Action<float>> ValueProvider => Gen.Float.ToValueProvider();
    }

    public class HalfCodecTests : FieldCodecTester<Half, HalfCodec>
    {
        public HalfCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Half CreateValue() => (Half)BitConverter.UInt16BitsToHalf((ushort)Random.Next(ushort.MinValue, ushort.MaxValue));
        protected override Half[] TestValues => new[] { Half.MinValue, (Half)0, (Half)1.0f, Half.Tau, Half.E, Half.Epsilon, Half.MaxValue };

        protected override Action<Action<Half>> ValueProvider => assert => Gen.UShort.Sample(value => assert(BitConverter.UInt16BitsToHalf(value)));
    }

    public class HalfCopierTests : CopierTester<Half, IDeepCopier<Half>>
    {
        public HalfCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Half CreateValue() => (Half)BitConverter.UInt16BitsToHalf((ushort)Random.Next(ushort.MinValue, ushort.MaxValue));
        protected override Half[] TestValues => new[] { Half.MinValue, (Half)0, (Half)1.0f, Half.Tau, Half.E, Half.Epsilon, Half.MaxValue };

        protected override Action<Action<Half>> ValueProvider => assert => Gen.UShort.Sample(value => assert(BitConverter.UInt16BitsToHalf(value)));
    }

    public class DoubleCodecTests : FieldCodecTester<double, DoubleCodec>
    {
        public DoubleCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override double CreateValue() => double.MaxValue * Random.NextDouble() * Math.Sign(Guid.NewGuid().GetHashCode());
        protected override double[] TestValues => new[] { double.MinValue, 0, 1.0, double.MaxValue };

        protected override Action<Action<double>> ValueProvider => Gen.Double.ToValueProvider();
    }

    public class DoubleCopierTests : CopierTester<double, IDeepCopier<double>>
    {
        public DoubleCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override double CreateValue() => double.MaxValue * Random.NextDouble() * Math.Sign(Guid.NewGuid().GetHashCode());
        protected override double[] TestValues => new[] { double.MinValue, 0, 1.0, double.MaxValue };

        protected override Action<Action<double>> ValueProvider => Gen.Double.ToValueProvider();
    }

    public class DecimalCodecTests : FieldCodecTester<decimal, DecimalCodec>
    {
        public DecimalCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override decimal CreateValue() => decimal.MaxValue * (decimal)Random.NextDouble() * Math.Sign(Guid.NewGuid().GetHashCode());
        protected override decimal[] TestValues => new[] { decimal.MinValue, 0, 1.0M, decimal.MaxValue };
        protected override Action<Action<decimal>> ValueProvider => Gen.Decimal.ToValueProvider();
    }

    public class DecimalCopierTests : CopierTester<decimal, IDeepCopier<decimal>>
    {
        public DecimalCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override decimal CreateValue() => decimal.MaxValue * (decimal)Random.NextDouble() * Math.Sign(Guid.NewGuid().GetHashCode());
        protected override decimal[] TestValues => new[] { decimal.MinValue, 0, 1.0M, decimal.MaxValue };
        protected override Action<Action<decimal>> ValueProvider => Gen.Decimal.ToValueProvider();
    }

    public class ListCodecTests : FieldCodecTester<List<int>, ListCodec<int>>
    {
        public ListCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override List<int> CreateValue()
        {
            var result = new List<int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result.Add(Random.Next());
            }

            return result;
        }

        protected override bool Equals(List<int> left, List<int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
        protected override List<int>[] TestValues => new[] { null, new List<int>(), CreateValue(), CreateValue(), CreateValue() };
    }

    public class ListCopierTests : CopierTester<List<int>, ListCopier<int>>
    {
        public ListCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override List<int> CreateValue()
        {
            var result = new List<int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result.Add(Random.Next());
            }

            return result;
        }

        protected override bool Equals(List<int> left, List<int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
        protected override List<int>[] TestValues => new[] { null, new List<int>(), CreateValue(), CreateValue(), CreateValue() };
    }

    public class QueueCodecTests : FieldCodecTester<Queue<int>, QueueCodec<int>>
    {
        public QueueCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Queue<int> CreateValue()
        {
            var result = new Queue<int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result.Enqueue(Random.Next());
            }

            return result;
        }

        protected override bool Equals(Queue<int> left, Queue<int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
        protected override Queue<int>[] TestValues => new[] { null, new Queue<int>(), CreateValue(), CreateValue(), CreateValue() };
    }

    public class QueueCopierTests : CopierTester<Queue<int>, QueueCopier<int>>
    {
        public QueueCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Queue<int> CreateValue()
        {
            var result = new Queue<int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result.Enqueue(Random.Next());
            }

            return result;
        }

        protected override bool Equals(Queue<int> left, Queue<int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
        protected override Queue<int>[] TestValues => new[] { null, new Queue<int>(), CreateValue(), CreateValue(), CreateValue() };
    }

    public class ConcurrentQueueCodecTests : FieldCodecTester<ConcurrentQueue<int>, ConcurrentQueueCodec<int>>
    {
        public ConcurrentQueueCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ConcurrentQueue<int> CreateValue()
        {
            var result = new ConcurrentQueue<int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result.Enqueue(Random.Next());
            }

            return result;
        }

        protected override bool Equals(ConcurrentQueue<int> left, ConcurrentQueue<int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
        protected override ConcurrentQueue<int>[] TestValues => new[] { null, new ConcurrentQueue<int>(), CreateValue(), CreateValue(), CreateValue() };
    }

    public class ConcurrentQueueCopierTests : CopierTester<ConcurrentQueue<int>, ConcurrentQueueCopier<int>>
    {
        public ConcurrentQueueCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ConcurrentQueue<int> CreateValue()
        {
            var result = new ConcurrentQueue<int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result.Enqueue(Random.Next());
            }

            return result;
        }

        protected override bool Equals(ConcurrentQueue<int> left, ConcurrentQueue<int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
        protected override ConcurrentQueue<int>[] TestValues => new[] { null, new ConcurrentQueue<int>(), CreateValue(), CreateValue(), CreateValue() };
    }

    public class DictionaryCodecTests : FieldCodecTester<Dictionary<string, int>, DictionaryCodec<string, int>>
    {
        public DictionaryCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Dictionary<string, int> CreateValue()
        {
            var result = new Dictionary<string, int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result[Random.Next().ToString()] = Random.Next();
            }

            return result;
        }

        protected override Dictionary<string, int>[] TestValues => new[] { null, new Dictionary<string, int>(), CreateValue(), CreateValue(), CreateValue() };
        protected override bool Equals(Dictionary<string, int> left, Dictionary<string, int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }

    public class DictionaryCopierTests : CopierTester<Dictionary<string, int>, DictionaryCopier<string, int>>
    {
        public DictionaryCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Dictionary<string, int> CreateValue()
        {
            var result = new Dictionary<string, int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result[Random.Next().ToString()] = Random.Next();
            }

            return result;
        }

        protected override Dictionary<string, int>[] TestValues => new[] { null, new Dictionary<string, int>(), CreateValue(), CreateValue(), CreateValue() };
        protected override bool Equals(Dictionary<string, int> left, Dictionary<string, int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }

    public class DictionaryWithComparerCodecTests : FieldCodecTester<Dictionary<string, int>, DictionaryCodec<string, int>>
    {
        protected override int[] MaxSegmentSizes => new[] { 1024 };
        private int _nextComparer;
        private IEqualityComparer<string>[] _comparers = new IEqualityComparer<string>[]
        {
            new CaseInsensitiveEqualityComparer(),
#if NETCOREAPP3_1_OR_GREATER
            StringComparer.Ordinal,
            StringComparer.OrdinalIgnoreCase,
            EqualityComparer<string>.Default,
#endif
#if NET6_0_OR_GREATER
            StringComparer.InvariantCulture,
            StringComparer.InvariantCultureIgnoreCase,
            StringComparer.CurrentCulture,
            StringComparer.CurrentCultureIgnoreCase,
#endif
        };

        public DictionaryWithComparerCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Dictionary<string, int> CreateValue()
        {
            var eqComparer = _comparers[_nextComparer++ % _comparers.Length];
            var result = new Dictionary<string, int>(eqComparer);
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                var key = Guid.NewGuid().ToString();
                result[key.ToLowerInvariant()] = Random.Next();
                result[key.ToUpperInvariant()] = Random.Next();
            }

            return result;
        }

        protected override Dictionary<string, int>[] TestValues => new[] {
            null,
            new Dictionary<string, int>(),
            CreateValue(),
            CreateValue(),
            CreateValue(),
            CreateValue(),
            CreateValue(),
            CreateValue(),
            CreateValue(),
            CreateValue()
        };

        protected override bool Equals(Dictionary<string, int> left, Dictionary<string, int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);

        [GenerateSerializer]
        public class CaseInsensitiveEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string left, string right)
            {
                if (left == null && right == null)
                {
                    return true;
                }
                else if (left == null || right == null)
                {
                    return false;
                }
                else if (left.ToUpper() == right.ToUpper())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(string s) => s.ToUpper().GetHashCode();
        }
    }

    public class DictionaryWithComparerCopierTests : CopierTester<Dictionary<string, int>, DictionaryCopier<string, int>>
    {
        public DictionaryWithComparerCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Dictionary<string, int> CreateValue()
        {
            var eqComparer = new DictionaryWithComparerCodecTests.CaseInsensitiveEqualityComparer();
            var result = new Dictionary<string, int>(eqComparer);
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result[Random.Next().ToString()] = Random.Next();
            }

            return result;
        }

        protected override Dictionary<string, int>[] TestValues => new[] { null, new Dictionary<string, int>(), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(Dictionary<string, int> left, Dictionary<string, int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }

    public class ConcurrentDictionaryCodecTests : FieldCodecTester<ConcurrentDictionary<string, int>, ConcurrentDictionaryCodec<string, int>>
    {
        public ConcurrentDictionaryCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ConcurrentDictionary<string, int> CreateValue()
        {
            var result = new ConcurrentDictionary<string, int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result[Random.Next().ToString()] = Random.Next();
            }

            return result;
        }

        protected override ConcurrentDictionary<string, int>[] TestValues => new[] { null, new ConcurrentDictionary<string, int>(), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(ConcurrentDictionary<string, int> left, ConcurrentDictionary<string, int> right)
        {
            // Order of the key-value pairs in the return value may not match the order of the key-value pairs in the surrogate
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }
            else if (left.Keys.Count != right.Keys.Count)
            {
                return false;
            }

            foreach (string k in left.Keys)
            {
                if (!(right.ContainsKey(k) && left[k] == right[k]))
                {
                    return false;
                }
            }
            
            return true;
        }
    }

    public class ConcurrentDictionaryCopierTests : CopierTester<ConcurrentDictionary<string, int>, ConcurrentDictionaryCopier<string, int>>
    {
        public ConcurrentDictionaryCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ConcurrentDictionary<string, int> CreateValue()
        {
            var result = new ConcurrentDictionary<string, int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result[Random.Next().ToString()] = Random.Next();
            }

            return result;
        }

        protected override ConcurrentDictionary<string, int>[] TestValues => new[] { null, new ConcurrentDictionary<string, int>(), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(ConcurrentDictionary<string, int> left, ConcurrentDictionary<string, int> right)
        {
            // Order of the key-value pairs in the return value may not match the order of the key-value pairs in the surrogate
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }
            else if (left.Keys.Count != right.Keys.Count)
            {
                return false;
            }

            foreach (string k in left.Keys)
            {
                if (!(right.ContainsKey(k) && left[k] == right[k]))
                {
                    return false;
                }
            }
            
            return true;
        }
    }

    public class ReadOnlyDictionaryCodecTests : FieldCodecTester<ReadOnlyDictionary<string, int>, ReadOnlyDictionaryCodec<string, int>>
    {
        public ReadOnlyDictionaryCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ReadOnlyDictionary<string, int> CreateValue()
        {
            var dict = new Dictionary<string, int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                dict[Random.Next().ToString()] = Random.Next();
            }

            return new ReadOnlyDictionary<string, int>(dict);
        }

        protected override ReadOnlyDictionary<string, int>[] TestValues => new[] { null, new ReadOnlyDictionary<string, int>(new Dictionary<string, int>()), CreateValue(), CreateValue(), CreateValue() };
        protected override bool Equals(ReadOnlyDictionary<string, int> left, ReadOnlyDictionary<string, int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }

    public class ReadOnlyDictionaryCopierTests : CopierTester<ReadOnlyDictionary<string, int>, ReadOnlyDictionaryCopier<string, int>>
    {
        public ReadOnlyDictionaryCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ReadOnlyDictionary<string, int> CreateValue()
        {
            var dict = new Dictionary<string, int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                dict[Random.Next().ToString()] = Random.Next();
            }

            return new ReadOnlyDictionary<string, int>(dict);
        }

        protected override ReadOnlyDictionary<string, int>[] TestValues => new[] { null, new ReadOnlyDictionary<string, int>(new Dictionary<string, int>()), CreateValue(), CreateValue(), CreateValue() };
        protected override bool Equals(ReadOnlyDictionary<string, int> left, ReadOnlyDictionary<string, int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }

    public class SortedDictionaryCodecTests : FieldCodecTester<SortedDictionary<string, int>, SortedDictionaryCodec<string, int>>
    {
        public SortedDictionaryCodecTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override SortedDictionary<string, int> CreateValue()
        {
            var result = new SortedDictionary<string, int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result[Random.Next().ToString()] = Random.Next();
            }

            return result;
        }

        protected override SortedDictionary<string, int>[] TestValues => new[] { null, new SortedDictionary<string, int>(), CreateValue(), CreateValue(), CreateValue() };
        protected override bool Equals(SortedDictionary<string, int> left, SortedDictionary<string, int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }

    public class SortedDictionaryCopierTests : CopierTester<SortedDictionary<string, int>, SortedDictionaryCopier<string, int>>
    {
        public SortedDictionaryCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override SortedDictionary<string, int> CreateValue()
        {
            var result = new SortedDictionary<string, int>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                result[Random.Next().ToString()] = Random.Next();
            }

            return result;
        }

        protected override SortedDictionary<string, int>[] TestValues => new[] { null, new SortedDictionary<string, int>(), CreateValue(), CreateValue(), CreateValue() };
        protected override bool Equals(SortedDictionary<string, int> left, SortedDictionary<string, int> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }

    public class IPAddressTests : FieldCodecTester<IPAddress, IPAddressCodec>
    {
        public IPAddressTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override IPAddress[] TestValues => new[] { null, IPAddress.Any, IPAddress.IPv6Any, IPAddress.IPv6Loopback, IPAddress.IPv6None, IPAddress.Loopback, IPAddress.Parse("123.123.10.3"), CreateValue() };

        protected override IPAddress CreateValue()
        {
            byte[] bytes;
            if (Random.Next(1) == 0)
            {
                bytes = new byte[4];
            }
            else
            {
                bytes = new byte[16];
            }
            Random.NextBytes(bytes);
            return new IPAddress(bytes);
        } 
    }

    public class IPAddressCopierTests : CopierTester<IPAddress, IDeepCopier<IPAddress>>
    {
        public IPAddressCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override IPAddress[] TestValues => new[] { null, IPAddress.Any, IPAddress.IPv6Any, IPAddress.IPv6Loopback, IPAddress.IPv6None, IPAddress.Loopback, IPAddress.Parse("123.123.10.3"), CreateValue() };

        protected override IPAddress CreateValue()
        {
            byte[] bytes;
            if (Random.Next(1) == 0)
            {
                bytes = new byte[4];
            }
            else
            {
                bytes = new byte[16];
            }
            Random.NextBytes(bytes);
            return new IPAddress(bytes);
        }

        protected override bool IsImmutable => true;
    }

    public class HashSetTests : FieldCodecTester<HashSet<string>, HashSetCodec<string>>
    {
        public HashSetTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override HashSet<string> CreateValue()
        {
            var result = new HashSet<string>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                _ = result.Add(Random.Next().ToString());
            }

            return result;
        }

        protected override HashSet<string>[] TestValues => new[] { null, new HashSet<string>(), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(HashSet<string> left, HashSet<string> right) => object.ReferenceEquals(left, right) || left.SetEquals(right);
    }

    public class HashSetCopierTests : CopierTester<HashSet<string>, HashSetCopier<string>>
    {
        public HashSetCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override HashSet<string> CreateValue()
        {
            var result = new HashSet<string>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                _ = result.Add(Random.Next().ToString());
            }

            return result;
        }

        protected override HashSet<string>[] TestValues => new[] { null, new HashSet<string>(), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(HashSet<string> left, HashSet<string> right) => object.ReferenceEquals(left, right) || left.SetEquals(right);
    }

    public class ImmutableHashSetTests : FieldCodecTester<ImmutableHashSet<string>, ImmutableHashSetCodec<string>>
    {
        public ImmutableHashSetTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ImmutableHashSet<string> CreateValue()
        {
            var hashSet = new HashSet<string>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                _ = hashSet.Add(Random.Next().ToString());
            }

            return hashSet.ToImmutableHashSet();
        }

        protected override ImmutableHashSet<string>[] TestValues => new[] { null, ImmutableHashSet.Create<string>(), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(ImmutableHashSet<string> left, ImmutableHashSet<string> right) => object.ReferenceEquals(left, right) || left.SetEquals(right);
    }

    public class ImmutableHashSetCopierTests : CopierTester<ImmutableHashSet<string>, ImmutableHashSetCopier<string>>
    {
        public ImmutableHashSetCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ImmutableHashSet<string> CreateValue()
        {
            var hashSet = new HashSet<string>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                _ = hashSet.Add(Random.Next().ToString());
            }

            return hashSet.ToImmutableHashSet();
        }

        protected override ImmutableHashSet<string>[] TestValues => new[] { null, ImmutableHashSet.Create<string>(), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(ImmutableHashSet<string> left, ImmutableHashSet<string> right) => object.ReferenceEquals(left, right) || left.SetEquals(right);

        protected override bool IsImmutable => true;
    }

    public sealed class ImmutableHashSetMutableCopierTests : CopierTester<ImmutableHashSet<object>, ImmutableHashSetCopier<object>>
    {
        public ImmutableHashSetMutableCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ImmutableHashSet<object> CreateValue()
        {
            var hashSet = new HashSet<object>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                _ = hashSet.Add(Random.Next());
            }

            return hashSet.ToImmutableHashSet();
        }

        protected override ImmutableHashSet<object>[] TestValues => new[] { null, ImmutableHashSet.Create<object>(), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(ImmutableHashSet<object> left, ImmutableHashSet<object> right) => ReferenceEquals(left, right) || left.SetEquals(right);

        protected override bool IsImmutable => false;
    }

    public class UriTests : FieldCodecTester<Uri, UriCodec>
    {
        public UriTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override int[] MaxSegmentSizes => new[] { 128 };
        protected override Uri CreateValue() => new Uri($"http://www.{Guid.NewGuid()}.com/");

        protected override Uri[] TestValues => new[] { null, CreateValue(), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(Uri left, Uri right) => object.ReferenceEquals(left, right) || left == right;
    }

    public class UriCopierTests : CopierTester<Uri, IDeepCopier<Uri>>
    {
        public UriCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Uri CreateValue() => new Uri($"http://www.{Guid.NewGuid()}.com/");

        protected override Uri[] TestValues => new[] { null, CreateValue(), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(Uri left, Uri right) => object.ReferenceEquals(left, right) || left == right;

        protected override bool IsImmutable => true;
    }

    public class FSharpOptionTests : FieldCodecTester<FSharpOption<Guid>, FSharpOptionCodec<Guid>>
    {
        public FSharpOptionTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpOption<Guid>[] TestValues => new[] { null, FSharpOption<Guid>.None, FSharpOption<Guid>.Some(Guid.Empty), FSharpOption<Guid>.Some(Guid.NewGuid()) };

        protected override FSharpOption<Guid> CreateValue() => FSharpOption<Guid>.Some(Guid.NewGuid());
    }

    public class FSharpOptionCopierTests : CopierTester<FSharpOption<Guid>, FSharpOptionCopier<Guid>>
    {
        public FSharpOptionCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpOption<Guid>[] TestValues => new[] { null, FSharpOption<Guid>.None, FSharpOption<Guid>.Some(Guid.Empty), FSharpOption<Guid>.Some(Guid.NewGuid()) };

        protected override FSharpOption<Guid> CreateValue() => FSharpOption<Guid>.Some(Guid.NewGuid());

        protected override bool Equals(FSharpOption<Guid> left, FSharpOption<Guid> right) => object.ReferenceEquals(left, right) || left.GetType() == right.GetType() && left.Value.Equals(right.Value);
    }
    
    public class FSharpOptionTests2 : FieldCodecTester<FSharpOption<object>, FSharpOptionCodec<object>>
    {
        public FSharpOptionTests2(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpOption<object>[] TestValues => new[] { null, FSharpOption<object>.Some(null), FSharpOption<object>.None, FSharpOption<object>.Some(Guid.Empty), FSharpOption<object>.Some(Guid.NewGuid()) };

        protected override FSharpOption<object> CreateValue() => FSharpOption<object>.Some(Guid.NewGuid());
    }

    public class FSharpRefTests : FieldCodecTester<FSharpRef<Guid>, FSharpRefCodec<Guid>>
    {
        public FSharpRefTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpRef<Guid>[] TestValues => new[] { null, new FSharpRef<Guid>(default), new FSharpRef<Guid>(Guid.Empty), new FSharpRef<Guid>(Guid.NewGuid()) };

        protected override FSharpRef<Guid> CreateValue() => new(Guid.NewGuid());

        protected override bool Equals(FSharpRef<Guid> left, FSharpRef<Guid> right) => object.ReferenceEquals(left, right) || EqualityComparer<Guid>.Default.Equals(left.Value, right.Value);
    }

    public class FSharpRefCopierTests : CopierTester<FSharpRef<Guid>, FSharpRefCopier<Guid>>
    {
        public FSharpRefCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpRef<Guid>[] TestValues => new[] { null, new FSharpRef<Guid>(default), new FSharpRef<Guid>(Guid.Empty), new FSharpRef<Guid>(Guid.NewGuid()) };

        protected override FSharpRef<Guid> CreateValue() => new(Guid.NewGuid());

        protected override bool Equals(FSharpRef<Guid> left, FSharpRef<Guid> right) => ReferenceEquals(left, right) || left.GetType() == right.GetType() && left.Value.Equals(right.Value);
    }

    public class FSharpValueOptionTests : FieldCodecTester<FSharpValueOption<Guid>, FSharpValueOptionCodec<Guid>>
    {
        public FSharpValueOptionTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpValueOption<Guid>[] TestValues => new[] { default, FSharpValueOption<Guid>.None, FSharpValueOption<Guid>.Some(Guid.Empty), FSharpValueOption<Guid>.Some(Guid.NewGuid()) };

        protected override FSharpValueOption<Guid> CreateValue() => FSharpValueOption<Guid>.Some(Guid.NewGuid());
    }

    public class FSharpValueOptionCopierTests : CopierTester<FSharpValueOption<Guid>, FSharpValueOptionCopier<Guid>>
    {
        public FSharpValueOptionCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpValueOption<Guid>[] TestValues => new[] { default, FSharpValueOption<Guid>.None, FSharpValueOption<Guid>.Some(Guid.Empty), FSharpValueOption<Guid>.Some(Guid.NewGuid()) };

        protected override FSharpValueOption<Guid> CreateValue() => FSharpValueOption<Guid>.Some(Guid.NewGuid());

        protected override bool Equals(FSharpValueOption<Guid> left, FSharpValueOption<Guid> right) => left.IsNone && right.IsNone || left.IsSome && right.IsSome && left.Value.Equals(right.Value);
    }

    public class FSharpChoice2Tests : FieldCodecTester<FSharpChoice<int, Guid>, FSharpChoiceCodec<int, Guid>>
    {
        public FSharpChoice2Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpChoice<int, Guid>[] TestValues => new[] { FSharpChoice<int, Guid>.NewChoice1Of2(-1), FSharpChoice<int, Guid>.NewChoice2Of2(Guid.NewGuid()) };

        protected override FSharpChoice<int, Guid> CreateValue() => FSharpChoice<int, Guid>.NewChoice1Of2(0);
    }

    public class FSharpChoice2CopierTests : CopierTester<FSharpChoice<int, Guid>, FSharpChoiceCopier<int, Guid>>
    {
        public FSharpChoice2CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpChoice<int, Guid>[] TestValues => new[] { FSharpChoice<int, Guid>.NewChoice1Of2(-1), FSharpChoice<int, Guid>.NewChoice2Of2(Guid.NewGuid()) };

        protected override FSharpChoice<int, Guid> CreateValue() => FSharpChoice<int, Guid>.NewChoice1Of2(0);
    }

    public class FSharpChoice3Tests : FieldCodecTester<FSharpChoice<int, Guid, Guid>, FSharpChoiceCodec<int, Guid, Guid>>
    {
        public FSharpChoice3Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpChoice<int, Guid, Guid>[] TestValues => new[] { FSharpChoice<int, Guid, Guid>.NewChoice1Of3(-1), FSharpChoice<int, Guid, Guid>.NewChoice3Of3(Guid.NewGuid()) };

        protected override FSharpChoice<int, Guid, Guid> CreateValue() => FSharpChoice<int, Guid, Guid>.NewChoice1Of3(0);
    }

    public class FSharpChoice3CopierTests : CopierTester<FSharpChoice<int, Guid, Guid>, FSharpChoiceCopier<int, Guid, Guid>>
    {
        public FSharpChoice3CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpChoice<int, Guid, Guid>[] TestValues => new[] { FSharpChoice<int, Guid, Guid>.NewChoice1Of3(-1), FSharpChoice<int, Guid, Guid>.NewChoice3Of3(Guid.NewGuid()) };

        protected override FSharpChoice<int, Guid, Guid> CreateValue() => FSharpChoice<int, Guid, Guid>.NewChoice1Of3(0);
    }

    public class FSharpChoice4Tests : FieldCodecTester<FSharpChoice<int, Guid, Guid, Guid>, FSharpChoiceCodec<int, Guid, Guid, Guid>>
    {
        public FSharpChoice4Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpChoice<int, Guid, Guid, Guid>[] TestValues => new[] { FSharpChoice<int, Guid, Guid, Guid>.NewChoice1Of4(-1), FSharpChoice<int, Guid, Guid, Guid>.NewChoice4Of4(Guid.NewGuid()) };

        protected override FSharpChoice<int, Guid, Guid, Guid> CreateValue() => FSharpChoice<int, Guid, Guid, Guid>.NewChoice1Of4(0);
    }

    public class FSharpChoice4CopierTests : CopierTester<FSharpChoice<int, Guid, Guid, Guid>, FSharpChoiceCopier<int, Guid, Guid, Guid>>
    {
        public FSharpChoice4CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpChoice<int, Guid, Guid, Guid>[] TestValues => new[] { FSharpChoice<int, Guid, Guid, Guid>.NewChoice1Of4(-1), FSharpChoice<int, Guid, Guid, Guid>.NewChoice4Of4(Guid.NewGuid()) };

        protected override FSharpChoice<int, Guid, Guid, Guid> CreateValue() => FSharpChoice<int, Guid, Guid, Guid>.NewChoice1Of4(0);
    }

    public class FSharpChoice5Tests : FieldCodecTester<FSharpChoice<int, Guid, Guid, Guid, Guid>, FSharpChoiceCodec<int, Guid, Guid, Guid, Guid>>
    {
        public FSharpChoice5Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpChoice<int, Guid, Guid, Guid, Guid>[] TestValues => new[] { FSharpChoice<int, Guid, Guid, Guid, Guid>.NewChoice1Of5(-1), FSharpChoice<int, Guid, Guid, Guid, Guid>.NewChoice5Of5(Guid.NewGuid()) };

        protected override FSharpChoice<int, Guid, Guid, Guid, Guid> CreateValue() => FSharpChoice<int, Guid, Guid, Guid, Guid>.NewChoice1Of5(0);
    }

    public class FSharpChoice5CopierTests : CopierTester<FSharpChoice<int, Guid, Guid, Guid, Guid>, FSharpChoiceCopier<int, Guid, Guid, Guid, Guid>>
    {
        public FSharpChoice5CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpChoice<int, Guid, Guid, Guid, Guid>[] TestValues => new[] { FSharpChoice<int, Guid, Guid, Guid, Guid>.NewChoice1Of5(-1), FSharpChoice<int, Guid, Guid, Guid, Guid>.NewChoice5Of5(Guid.NewGuid()) };

        protected override FSharpChoice<int, Guid, Guid, Guid, Guid> CreateValue() => FSharpChoice<int, Guid, Guid, Guid, Guid>.NewChoice1Of5(0);
    }

    public class FSharpChoice6Tests : FieldCodecTester<FSharpChoice<int, Guid, Guid, Guid, Guid, Guid>, FSharpChoiceCodec<int, Guid, Guid, Guid, Guid, Guid>>
    {
        public FSharpChoice6Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpChoice<int, Guid, Guid, Guid, Guid, Guid>[] TestValues => new[] { FSharpChoice<int, Guid, Guid, Guid, Guid, Guid>.NewChoice1Of6(-1), FSharpChoice<int, Guid, Guid, Guid, Guid, Guid>.NewChoice6Of6(Guid.NewGuid()) };

        protected override FSharpChoice<int, Guid, Guid, Guid, Guid, Guid> CreateValue() => FSharpChoice<int, Guid, Guid, Guid, Guid, Guid>.NewChoice1Of6(0);
    }

    public class FSharpChoice6CopierTests : CopierTester<FSharpChoice<int, Guid, Guid, Guid, Guid, Guid>, FSharpChoiceCopier<int, Guid, Guid, Guid, Guid, Guid>>
    {
        public FSharpChoice6CopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpChoice<int, Guid, Guid, Guid, Guid, Guid>[] TestValues => new[] { FSharpChoice<int, Guid, Guid, Guid, Guid, Guid>.NewChoice1Of6(-1), FSharpChoice<int, Guid, Guid, Guid, Guid, Guid>.NewChoice6Of6(Guid.NewGuid()) };

        protected override FSharpChoice<int, Guid, Guid, Guid, Guid, Guid> CreateValue() => FSharpChoice<int, Guid, Guid, Guid, Guid, Guid>.NewChoice1Of6(0);
    }

    public class FSharpSetTests : FieldCodecTester<FSharpSet<string>, FSharpSetCodec<string>>
    {
        public FSharpSetTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpSet<string> CreateValue()
        {
            var hashSet = new HashSet<string>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                _ = hashSet.Add(Random.Next().ToString());
            }

            return SetModule.OfSeq(hashSet);
        }

        protected override FSharpSet<string>[] TestValues => new[] { null, SetModule.Empty<string>(), new FSharpSet<string>(Array.Empty<string>()), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(FSharpSet<string> left, FSharpSet<string> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }

    public class FSharpSetCopierTests : CopierTester<FSharpSet<string>, FSharpSetCopier<string>>
    {
        public FSharpSetCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpSet<string> CreateValue()
        {
            var hashSet = new HashSet<string>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                _ = hashSet.Add(Random.Next().ToString());
            }

            return SetModule.OfSeq(hashSet);
        }

        protected override FSharpSet<string>[] TestValues => new[] { null, SetModule.Empty<string>(), new FSharpSet<string>(Array.Empty<string>()), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(FSharpSet<string> left, FSharpSet<string> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }

    public class FSharpMapTests : FieldCodecTester<FSharpMap<string, string>, FSharpMapCodec<string, string>>
    {
        public FSharpMapTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override int[] MaxSegmentSizes => new[] { 128 };

        protected override FSharpMap<string, string> CreateValue()
        {
            var collection = new List<Tuple<string, string>>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                collection.Add(Tuple.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            }

            return MapModule.OfSeq(collection);
        }

        protected override FSharpMap<string, string>[] TestValues => new[] { null, MapModule.Empty<string, string>(), new FSharpMap<string, string>(Array.Empty<Tuple<string, string>>()), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(FSharpMap<string, string> left, FSharpMap<string, string> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }

    public class FSharpMapCopierTests : CopierTester<FSharpMap<string, string>, FSharpMapCopier<string, string>>
    {
        public FSharpMapCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpMap<string, string> CreateValue()
        {
            var collection = new List<Tuple<string, string>>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                collection.Add(Tuple.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            }

            return MapModule.OfSeq(collection);
        }

        protected override FSharpMap<string, string>[] TestValues => new[] { null, MapModule.Empty<string, string>(), new FSharpMap<string, string>(Array.Empty<Tuple<string, string>>()), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(FSharpMap<string, string> left, FSharpMap<string, string> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }

    public class FSharpListTests : FieldCodecTester<FSharpList<string>, FSharpListCodec<string>>
    {
        public FSharpListTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpList<string> CreateValue()
        {
            var list = new List<string>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                list.Add(Random.Next().ToString());
            }

            return ListModule.OfSeq(list);
        }

        protected override FSharpList<string>[] TestValues => new[] { null, ListModule.Empty<string>(), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(FSharpList<string> left, FSharpList<string> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }

    public class FSharpListCopierTests : CopierTester<FSharpList<string>, FSharpListCopier<string>>
    {
        public FSharpListCopierTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override FSharpList<string> CreateValue()
        {
            var list = new List<string>();
            for (var i = 0; i < Random.Next(17) + 5; i++)
            {
                list.Add(Random.Next().ToString());
            }

            return ListModule.OfSeq(list);
        }

        protected override FSharpList<string>[] TestValues => new[] { null, ListModule.Empty<string>(), CreateValue(), CreateValue(), CreateValue() };

        protected override bool Equals(FSharpList<string> left, FSharpList<string> right) => object.ReferenceEquals(left, right) || left.SequenceEqual(right);
    }
}