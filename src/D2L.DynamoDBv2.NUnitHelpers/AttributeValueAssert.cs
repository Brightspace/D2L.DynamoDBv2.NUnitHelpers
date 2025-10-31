using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Amazon.DynamoDBv2.Model;
using NUnit.Framework;

namespace D2L.DynamoDBv2.NUnitHelpers {

	public static class AttributeValueAssert {

		public static void AreEqual(
				Dictionary<string, AttributeValue>? actual,
				Dictionary<string, AttributeValue>? expected
			) {

			AssertMapAttributes(
					path: "M",
					actual: actual,
					expected: expected
				);
		}

		public static void AreEqual(
				AttributeValue actual,
				AttributeValue expected
			) {

			AssertAttributeValue(
					path: string.Empty,
					actual: actual,
					expected: expected
				);
		}

		private static void AssertListAttributes(
				string path,
				List<AttributeValue>? actual,
				List<AttributeValue>? expected
			) {

			if( expected is null ) {
				Assert.That(
						actual,
						Is.Null,
						FormattableStringFactory.Create( "{0} is expected to be null", path )
					);
				return;
			}

			if( actual is null ) {
				Assert.Fail( $"{path} is expected to be not null" );
				return;
			}

			Assert.That(
					actual.Count,
					Is.EqualTo( expected.Count ),
					FormattableStringFactory.Create( "List length must be equal ({0})", path )
				);

			for( int i = 0; i < actual.Count; i++ ) {

				AssertAttributeValue(
						path: $"{ path }[{ i }].",
						actual: actual[ i ],
						expected: expected[ i ]
					);
			}
		}

		private static void AssertMapAttributes(
				string path,
				Dictionary<string, AttributeValue>? actual,
				Dictionary<string, AttributeValue>? expected
			) {

			if( expected is null ) {
				Assert.That(
					actual,
					Is.Null,
					FormattableStringFactory.Create( "{0} is expected to be null", path )
				);
				return;
			}

			if( actual is null ) {
				Assert.Fail( $"{path} is expected to be not null" );
				return;
			}

			Assert.That(
					actual.Keys,
					Is.EquivalentTo( expected.Keys ),
					FormattableStringFactory.Create( "{0}.Keys must be equivalent", path )
				);

			foreach( KeyValuePair<string, AttributeValue> pair in actual ) {

				AssertAttributeValue(
						path: $"{ path }[{ pair.Key }].",
						actual: pair.Value,
						expected: expected[ pair.Key ]
					);
			}
		}

		private static void AssertAttributeValue(
				string path,
				AttributeValue actual,
				AttributeValue expected
			) {

			if( actual.B != null && expected.B != null ) {

				Assert.That(
						actual.B.ToArray(),
						Is.EqualTo( expected.B.ToArray() ),
						FormattableStringFactory.Create( "{0}B must be equal", path )
					);

			} else {

				Assert.That(
						actual.B,
						Is.EqualTo( expected.B ),
						FormattableStringFactory.Create( "{0}B must be equal", path )
					);
			}

			Assert.That(
					actual.IsBOOLSet,
					Is.EqualTo( expected.IsBOOLSet ),
					FormattableStringFactory.Create( "{0}IsBOOLSet must be equal", path )
				);

			Assert.That(
					actual.BOOL,
					Is.EqualTo( expected.BOOL ),
					FormattableStringFactory.Create( "{0}BOOL must be equal", path )
				);

			Assert.That(
					actual.BS,
					Is.EqualTo( expected.BS ).Or.EquivalentTo( expected.BS ).Using( MemoryStreamEqualityComparer.Instance ),
					FormattableStringFactory.Create( "{0}BS must be equivalent", path )
				);

			Assert.That(
					actual.IsLSet,
					Is.EqualTo( expected.IsLSet ),
					FormattableStringFactory.Create( "{0}IsLSet must be equal", path )
				);

			AssertListAttributes(
					path: $"{ path }L",
					actual: actual.L,
					expected: expected.L
				);

			Assert.That(
					actual.IsMSet,
					Is.EqualTo( expected.IsMSet ),
					FormattableStringFactory.Create( "{0}IsMSet must be equal", path )
				);

			AssertMapAttributes(
					path: $"{ path }M",
					actual: actual.M,
					expected: expected.M
				);

			Assert.That(
					actual.N,
					Is.EqualTo( expected.N ),
					FormattableStringFactory.Create( "{0}N must be equal", path )
				);

			Assert.That(
					actual.NULL,
					Is.EqualTo( expected.NULL ),
					FormattableStringFactory.Create( "{0}NULL must be equal", path )
				);

			Assert.That(
					actual.NS,
					Is.EqualTo( expected.NS ).Or.EquivalentTo( expected.NS ),
					FormattableStringFactory.Create( "{0}NS must be equivalent", path )
				);

			Assert.That(
					actual.S,
					Is.EqualTo( expected.S ),
					FormattableStringFactory.Create( "{0}S must be equal", path )
				);

			Assert.That(
					actual.SS,
					Is.EqualTo( expected.SS ).Or.EquivalentTo( expected.SS ),
					FormattableStringFactory.Create( "{0}SS must be equivalent", path )
				);
		}

		private sealed class MemoryStreamEqualityComparer : IEqualityComparer<MemoryStream> {

			internal static readonly IEqualityComparer<MemoryStream> Instance = new MemoryStreamEqualityComparer();
			private MemoryStreamEqualityComparer() { }

			bool IEqualityComparer<MemoryStream>.Equals( MemoryStream? x, MemoryStream? y ) {

				if( ReferenceEquals( x, y ) ) {
					return true;
				}

				if( x != null && y != null ) {

					if( x.Length != y.Length ) {
						return false;
					}

					byte[] xBytes = x.ToArray();
					byte[] yBytes = y.ToArray();

					for( int i = 0; i < xBytes.Length; i++ ) {

						if( xBytes[ i ] != yBytes[ i ] ) {
							return false;
						}
					}

					return true;
				}

				return false;
			}

			int IEqualityComparer<MemoryStream>.GetHashCode( MemoryStream obj ) {
				throw new NotImplementedException();
			}
		}
	}
}
