using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using NUnit.Framework;

namespace D2L.DynamoDBv2.NUnitHelpers.Tests {

	[TestFixture]
	internal sealed class AmazonDynamoDBAssertTests {

		private IAmazonDynamoDB m_db;
		private string m_tableName;

		[OneTimeSetUp]
		public async Task OneTimeSetUp() {

			m_db = AmazonDynamoDBLocal.CreateClient();
			m_tableName = Guid.NewGuid().ToString();

			await m_db
				.CreateTableAsync(
					tableName: m_tableName,
					keySchema: new List<KeySchemaElement> {
						new KeySchemaElement( "key", KeyType.HASH )
					},
					attributeDefinitions: new List<AttributeDefinition> {
						new AttributeDefinition( "key", ScalarAttributeType.S )
					},
					provisionedThroughput: new ProvisionedThroughput( 1, 1 )
				)
				.ConfigureAwait( continueOnCapturedContext: false );
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDown() {

			await m_db
				.DeleteTableAsync( m_tableName )
				.ConfigureAwait( continueOnCapturedContext: false );

			m_db.Dispose();
		}

		private AttributeValue GenerateKey() {
			return new AttributeValue( Guid.NewGuid().ToString() );
		}

		#region AssertItemAsync Tests

		[Test]
		public async Task AssertItemAsync_WhenExistsAndEqual() {

			AttributeValue key = GenerateKey();

			Dictionary<string, AttributeValue> item = new Dictionary<string, AttributeValue> {
				{ "key", key },
				{ "age", new AttributeValue { N = "111" } }
			};

			await m_db
				.PutItemAsync( m_tableName, item )
				.ConfigureAwait( continueOnCapturedContext: false );

			Assert.DoesNotThrowAsync( async () => {

				await AmazonDynamoDBAssert
					.AssertItemAsync(
						m_db,
						m_tableName,
						key: new Dictionary<string, AttributeValue> {
							{ "key", key }
						},
						expectedItem: item
					)
					.ConfigureAwait( continueOnCapturedContext: false );
			} );
		}

		[Test]
		public async Task AssertItemAsync_WhenExistsAndNotEqual() {

			AttributeValue key = GenerateKey();

			Dictionary<string, AttributeValue> item = new Dictionary<string, AttributeValue> {
				{ "key", key },
				{ "age", new AttributeValue { N = "222" } }
			};

			await m_db
				.PutItemAsync( m_tableName, item )
				.ConfigureAwait( continueOnCapturedContext: false );

			Dictionary<string, AttributeValue> itemKey = new Dictionary<string, AttributeValue> {
				{ "key", key }
			};

			Dictionary<string, AttributeValue> expectedItem = new Dictionary<string, AttributeValue> {
				{ "key", key },
				{ "age", new AttributeValue { N = "333" } }
			};

			var err = Assert.Throws<AssertionException>( () => {

				AmazonDynamoDBAssert
					.AssertItemAsync(
						m_db,
						m_tableName,
						key: itemKey,
						expectedItem: expectedItem
					)
					.ConfigureAwait( continueOnCapturedContext: false )
					.GetAwaiter()
					.GetResult();
			} );

			Assert.That(
					err.Message,
					Does.StartWith( "  M[age].N must be equal" )
				);
		}

		[Test]
		public void AssertItemAsync_WhenDoesNotExist() {

			AttributeValue key = GenerateKey();

			Dictionary<string, AttributeValue> itemKey = new Dictionary<string, AttributeValue> {
				{ "key", key }
			};

			Dictionary<string, AttributeValue> expectedItem = new Dictionary<string, AttributeValue> {
				{ "key", key },
				{ "age", new AttributeValue { N = "555" } }
			};

			var err = Assert.Throws<AssertionException>( () => {

				AmazonDynamoDBAssert
					.AssertItemAsync(
						m_db,
						m_tableName,
						key: itemKey,
						expectedItem: expectedItem
					)
					.ConfigureAwait( continueOnCapturedContext: false )
					.GetAwaiter()
					.GetResult();
			} );

			Assert.That(
					err.Message,
					Is.EqualTo( "Item should exist." )
				);
		}

		#endregion

		#region AssertItemDoesNotExistAsync Tests

		[Test]
		public void AssertItemDoesNotExistAsync_WhenDoesNotExist() {

			AttributeValue key = GenerateKey();

			Dictionary<string, AttributeValue> itemKey = new Dictionary<string, AttributeValue> {
				{ "key", key }
			};

			Assert.DoesNotThrowAsync( async () => {

				await AmazonDynamoDBAssert
					.AssertItemDoesNotExistAsync(
						m_db,
						m_tableName,
						key: itemKey
					)
					.ConfigureAwait( continueOnCapturedContext: false );
			} );
		}

		[Test]
		public async Task AssertItemDoesNotExistAsync_WhenExists() {

			AttributeValue key = GenerateKey();

			await m_db
				.PutItemAsync(
					m_tableName,
					item: new Dictionary<string, AttributeValue> {
						{ "key", key },
						{ "age", new AttributeValue { N = "111" } }
					}
				)
				.ConfigureAwait( continueOnCapturedContext: false );

			Dictionary<string, AttributeValue> itemKey = new Dictionary<string, AttributeValue> {
				{ "key", key }
			};

			var err = Assert.Throws<AssertionException>( () => {

				AmazonDynamoDBAssert
					.AssertItemDoesNotExistAsync(
						m_db,
						m_tableName,
						key: itemKey
					)
					.ConfigureAwait( continueOnCapturedContext: false )
					.GetAwaiter()
					.GetResult();
			} );

			Assert.That(
					err.Message,
					Is.EqualTo( "Item should not exist." )
				);
		}

		#endregion

	}
}
