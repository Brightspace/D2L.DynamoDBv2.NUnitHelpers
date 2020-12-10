using System.Collections.Generic;
using System.IO;
using Amazon.DynamoDBv2.Model;
using NUnit.Framework;

namespace D2L.DynamoDBv2.NUnitHelpers {

	[TestFixture]
	internal sealed class AttributeValueAssertTests {

		#region Dictionary

		[Test]
		[TestCaseSource( nameof( EqualMapTestCases ) )]
		public void AreEqual_Dictionary_WhenEquivalent(
				Dictionary<string, AttributeValue> x,
				Dictionary<string, AttributeValue> y
			) {

			AttributeValueAssert.AreEqual( x, y );
		}

		[Test]
		[TestCaseSource( nameof( NotEqualMapTestCases ) )]
		public void AreEqual_Dictionary_WhenNotEqual(
				Dictionary<string, AttributeValue> x,
				Dictionary<string, AttributeValue> y,
				string expectedMessage
			) {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual( x, y );
			} );

			Assert.That(
					err.Message,
					Does.StartWith( expectedMessage )
				);
		}

		#endregion

		#region B

		[Test]
		[TestCase( new byte[ 0 ] )]
		[TestCase( new byte[] { 0x1 } )]
		[TestCase( new byte[] { 0x1, 0x2, 0x3, 0x4 } )]
		public void AreEqual_Numeric_WhenEqual( byte[] bytes ) {

			AttributeValueAssert.AreEqual(
					new AttributeValue { B = new MemoryStream( bytes ) },
					new AttributeValue { B = new MemoryStream( bytes ) }
				);
		}

		[Test]
		[TestCase( new byte[] { 0x1 }, new byte[] { 0x2 } )]
		[TestCase( new byte[] { 0x1, 0x5 }, new byte[] { 0x2, 0x6 } )]
		public void AreEqual_Numeric_WhenNotEqual(
				byte[] x,
				byte[] y
			) {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue { B = new MemoryStream( x ) },
					new AttributeValue { B = new MemoryStream( y ) }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( "  B must be equal" )
				);
		}

		#endregion

		#region BS

		private static IEnumerable<TestCaseData> EqualBinarySetTestCases() {

			yield return new TestCaseData(
					new List<MemoryStream> {
						new MemoryStream()
					},
					new List<MemoryStream> {
						new MemoryStream()
					}
				)
				.SetName( "1 empty stream" );

			yield return new TestCaseData(
					new List<MemoryStream> {
						new MemoryStream( new byte[] { 0x1, 0x2 } )
					},
					new List<MemoryStream> {
						new MemoryStream( new byte[] { 0x1, 0x2 } )
					}
				)
				.SetName( "1 non-empty stream" );

			yield return new TestCaseData(
					new List<MemoryStream> {
						new MemoryStream( new byte[] { 0x1 } ),
						new MemoryStream( new byte[] { 0x2 } )
					},
					new List<MemoryStream> {
						new MemoryStream( new byte[] { 0x2 } ),
						new MemoryStream( new byte[] { 0x1 } )
					}
				)
				.SetName( "2 non-empty streams" );
		}

		[Test]
		[TestCaseSource( nameof( EqualBinarySetTestCases ) )]
		public void AreEqual_BinarySet_WhenEquivalent(
				List<MemoryStream> x,
				List<MemoryStream> y
			) {

			AttributeValueAssert.AreEqual(
					new AttributeValue { BS = x },
					new AttributeValue { BS = y }
				);
		}

		private static IEnumerable<TestCaseData> NotEqualBinarySetTestCases() {

			yield return new TestCaseData(
					new List<MemoryStream> {
						new MemoryStream( new byte[] { 0x1 } ),
						new MemoryStream( new byte[] { 0x2 } )
					},
					new List<MemoryStream> {
						new MemoryStream( new byte[] { 0x1 } )
					}
				);

			yield return new TestCaseData(
					new List<MemoryStream> {
						new MemoryStream( new byte[] { 0x1, 0x2 } )
					},
					new List<MemoryStream> {
						new MemoryStream( new byte[] { 0x2, 0x1 } )
					}
				);

			yield return new TestCaseData(
					new List<MemoryStream> {
						new MemoryStream( new byte[] { 0x1, 0x2 } ),
						new MemoryStream( new byte[] { 0x3, 0x4 } )
					},
					new List<MemoryStream> {
						new MemoryStream( new byte[] { 0x5, 0x6 } ),
						new MemoryStream( new byte[] { 0x7 } )
					}
				);

			yield return new TestCaseData(
					new List<MemoryStream>(),
					new List<MemoryStream> {
						new MemoryStream( new byte[] { 0x1 } )
					}
				);

			yield return new TestCaseData(
					new List<MemoryStream> {
						new MemoryStream( new byte[] { 0x1 } )
					},
					new List<MemoryStream>()
				);
		}

		[Test]
		[TestCaseSource( nameof( NotEqualBinarySetTestCases ) )]
		public void AreEqual_BinarySet_WhenNotEqual(
				List<MemoryStream> x,
				List<MemoryStream> y
			) {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue { BS = x },
					new AttributeValue { BS = y }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( "  BS must be equivalent" )
				);
		}

		#endregion

		#region BOOL

		[Test]
		public void AreEqual_Boolean_WhenEqual( [Values] bool value ) {

			AttributeValueAssert.AreEqual(
					new AttributeValue { BOOL = value },
					new AttributeValue { BOOL = value }
				);
		}

		[Test]
		public void AreEqual_Boolean_WhenNotEqual() {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue { BOOL = true },
					new AttributeValue { BOOL = false }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( "  BOOL must be equal" )
				);
		}

		[Test]
		public void AreEqual_Boolean_WhenBooleanNotSet() {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue { BOOL = true },
					new AttributeValue { S = "abc" }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( "  IsBOOLSet must be equal" )
				);
		}

		#endregion

		#region L

		private static IEnumerable<TestCaseData> EqualListTestCases() {

			yield return new TestCaseData(
					new List<AttributeValue> { },
					new List<AttributeValue> { }
				);

			yield return new TestCaseData(
					new List<AttributeValue> {
						new AttributeValue{ N = "34.8" }
					},
					new List<AttributeValue> {
						new AttributeValue{ N = "34.8" }
					}
				);

			yield return new TestCaseData(
					new List<AttributeValue> {
						new AttributeValue{ N = "34.8" },
						new AttributeValue{ BOOL = true }
					},
					new List<AttributeValue> {
						new AttributeValue{ N = "34.8" },
						new AttributeValue{ BOOL = true }
					}
				);
		}

		[Test]
		[TestCaseSource( nameof( EqualListTestCases ) )]
		public void AreEqual_List_WhenEquivalent(
				List<AttributeValue> x,
				List<AttributeValue> y
			) {

			AttributeValueAssert.AreEqual(
					new AttributeValue { L = x },
					new AttributeValue { L = y }
				);
		}

		private static IEnumerable<TestCaseData> NotEqualListTestCases() {

			yield return new TestCaseData(
					new List<AttributeValue> {
						new AttributeValue{ N = "33" }
					},
					new List<AttributeValue> {
						new AttributeValue{ N = "44" }
					},
					"  L[0].N must be equal"
				);

			yield return new TestCaseData(
					new List<AttributeValue> {
						new AttributeValue{ N = "33" },
						new AttributeValue{ S = "abc" }
					},
					new List<AttributeValue> {
						new AttributeValue{ N = "33" },
						new AttributeValue{ S = "def" }
					},
					"  L[1].S must be equal"
				);
		}

		[Test]
		[TestCaseSource( nameof( NotEqualListTestCases ) )]
		public void AreEqual_List_WhenNotEqual(
				List<AttributeValue> x,
				List<AttributeValue> y,
				string expectedMessage
			) {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue { L = x },
					new AttributeValue { L = y }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( expectedMessage )
				);
		}

		[Test]
		public void AreEqual_List_WhenListNotSet() {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue {
						L = new List<AttributeValue> {
							new AttributeValue { N = "23.88" }
						}
					},
					new AttributeValue { S = "abc" }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( "  IsLSet must be equal" )
				);
		}

		#endregion

		#region M

		private static IEnumerable<TestCaseData> EqualMapTestCases() {

			yield return new TestCaseData(
					new Dictionary<string, AttributeValue> { },
					new Dictionary<string, AttributeValue> { }
				);

			yield return new TestCaseData(
					new Dictionary<string, AttributeValue> {
						{ "x", new AttributeValue{ N = "34.8" } }
					},
					new Dictionary<string, AttributeValue> {
						{ "x", new AttributeValue{ N = "34.8" } }
					}
				);

			yield return new TestCaseData(
					new Dictionary<string, AttributeValue> {
						{ "x", new AttributeValue{ N = "34.8" } },
						{ "y", new AttributeValue{ BOOL = true } }
					},
					new Dictionary<string, AttributeValue> {
						{ "x", new AttributeValue{ N = "34.8" } },
						{ "y", new AttributeValue{ BOOL = true } }
					}
				);
		}

		[Test]
		[TestCaseSource( nameof( EqualMapTestCases ) )]
		public void AreEqual_Map_WhenEquivalent(
				Dictionary<string, AttributeValue> x,
				Dictionary<string, AttributeValue> y
			) {

			AttributeValueAssert.AreEqual(
					new AttributeValue { M = x },
					new AttributeValue { M = y }
				);
		}

		private static IEnumerable<TestCaseData> NotEqualMapTestCases() {

			yield return new TestCaseData(
					new Dictionary<string, AttributeValue> {
						{ "x", new AttributeValue{ N = "34.8" } }
					},
					new Dictionary<string, AttributeValue> {
						{ "x", new AttributeValue{ N = "88.3" } }
					},
					"  M[x].N must be equal"
				);

			yield return new TestCaseData(
					new Dictionary<string, AttributeValue> {
						{ "x", new AttributeValue{ N = "34.8" } }
					},
					new Dictionary<string, AttributeValue> {
						{ "y", new AttributeValue{ N = "34.8" } }
					},
					"  M.Keys must be equivalent"
				);

			yield return new TestCaseData(
					new Dictionary<string, AttributeValue> {
						{ "root", new AttributeValue{
							M = new Dictionary<string, AttributeValue> {
								{ "child", new AttributeValue { BOOL = true } }
							}
						} }
					},
					new Dictionary<string, AttributeValue> {
						{ "root", new AttributeValue{
							M = new Dictionary<string, AttributeValue> {
								{ "child", new AttributeValue { BOOL = false } }
							}
						} }
					},
					"  M[root].M[child].BOOL must be equal"
				);
		}

		[Test]
		[TestCaseSource( nameof( NotEqualMapTestCases ) )]
		public void AreEqual_Map_WhenNotEqual(
				Dictionary<string, AttributeValue> x,
				Dictionary<string, AttributeValue> y,
				string expectedMessage
			) {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue { M = x },
					new AttributeValue { M = y }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( expectedMessage )
				);
		}

		[Test]
		public void AreEqual_Map_WhenMapNotSet() {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue {
						M = new Dictionary<string, AttributeValue>{
							{ "x",  new AttributeValue { N = "23.88" } }
						}
					},
					new AttributeValue { S = "abc" }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( "  IsMSet must be equal" )
				);
		}

		#endregion

		#region N

		[Test]
		[TestCase( "-80" )]
		[TestCase( "0" )]
		[TestCase( "1" )]
		[TestCase( "2.3" )]
		public void AreEqual_Numeric_WhenEqual( string value ) {

			AttributeValueAssert.AreEqual(
					new AttributeValue { N = value },
					new AttributeValue { N = value }
				);
		}

		[Test]
		[TestCase( "33", "45" )]
		[TestCase( "33", null )]
		[TestCase( null, "44" )]
		public void AreEqual_Numeric_WhenNotEqual( string x, string y ) {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue { N = "33" },
					new AttributeValue { N = "45" }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( "  N must be equal" )
				);
		}

		#endregion

		#region NS

		private static IEnumerable<TestCaseData> EqualNumericSetTestCases() {

			yield return new TestCaseData(
					new List<string>(),
					new List<string>()
				);

			yield return new TestCaseData(
					new List<string> { "123.5" },
					new List<string> { "123.5" }
				);

			yield return new TestCaseData(
					new List<string> { "55", "44" },
					new List<string> { "44", "55" }
				);
		}

		[Test]
		[TestCaseSource( nameof( EqualNumericSetTestCases ) )]
		public void AreEqual_NumericSet_WhenEquivalent(
				List<string> x,
				List<string> y
			) {

			AttributeValueAssert.AreEqual(
					new AttributeValue { NS = x },
					new AttributeValue { NS = y }
				);
		}

		private static IEnumerable<TestCaseData> NotEqualNumericSetTestCases() {

			yield return new TestCaseData(
					new List<string> { "1" },
					new List<string> { "1", "2" }
				);

			yield return new TestCaseData(
					new List<string> { "66" },
					new List<string> { "77" }
				);

			yield return new TestCaseData(
					new List<string> { "9", "8" },
					new List<string> { "3", "2", "1" }
				);
		}

		[Test]
		[TestCaseSource( nameof( NotEqualNumericSetTestCases ) )]
		public void AreEqual_NumericSet_WhenNotEqual(
				List<string> x,
				List<string> y
			) {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue { NS = x },
					new AttributeValue { NS = y }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( "  NS must be equivalent" )
				);
		}

		#endregion

		#region NULL

		[Test]
		public void AreEqual_Null_WhenEqual( [Values] bool value ) {

			AttributeValueAssert.AreEqual(
					new AttributeValue { NULL = value },
					new AttributeValue { NULL = value }
				);
		}

		[Test]
		public void AreEqual_Null_WhenNotEqual() {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue { NULL = true },
					new AttributeValue { NULL = false }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( "  NULL must be equal" )
				);
		}

		#endregion

		#region S

		[Test]
		[TestCase( "" )]
		[TestCase( "abc" )]
		public void AreEqual_String_WhenEqual( string value ) {

			AttributeValueAssert.AreEqual(
					new AttributeValue { S = value },
					new AttributeValue { S = value }
				);
		}

		[Test]
		public void AreEqual_String_WhenNotEqual() {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue { S = "x" },
					new AttributeValue { S = "y" }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( "  S must be equal" )
				);
		}

		#endregion

		#region SS

		private static IEnumerable<TestCaseData> EqualStringSetTestCases() {

			yield return new TestCaseData(
					new List<string>(),
					new List<string>()
				);

			yield return new TestCaseData(
					new List<string> { "abc" },
					new List<string> { "abc" }
				);

			yield return new TestCaseData(
					new List<string> { "x", "y" },
					new List<string> { "y", "x" }
				);
		}

		[Test]
		[TestCaseSource( nameof( EqualStringSetTestCases ) )]
		public void AreEqual_StringSet_WhenEquivalent(
				List<string> x,
				List<string> y
			) {

			AttributeValueAssert.AreEqual(
					new AttributeValue { SS = x },
					new AttributeValue { SS = y }
				);
		}

		private static IEnumerable<TestCaseData> NotEqualStringSetTestCases() {

			yield return new TestCaseData(
					new List<string> { "x" },
					new List<string> { "x", "y" }
				);

			yield return new TestCaseData(
					new List<string> { "x" },
					new List<string> { "y" }
				);

			yield return new TestCaseData(
					new List<string> { "cats", "meow" },
					new List<string> { "dogs", "like", "bones" }
				);
		}

		[Test]
		[TestCaseSource( nameof( NotEqualStringSetTestCases ) )]
		public void AreEqual_StringSet_WhenNotEqual(
				List<string> x,
				List<string> y
			) {

			var err = Assert.Throws<AssertionException>( () => {
				AttributeValueAssert.AreEqual(
					new AttributeValue { SS = x },
					new AttributeValue { SS = y }
				);
			} );

			Assert.That(
					err.Message,
					Does.StartWith( "  SS must be equivalent" )
				);
		}

		#endregion

	}
}
