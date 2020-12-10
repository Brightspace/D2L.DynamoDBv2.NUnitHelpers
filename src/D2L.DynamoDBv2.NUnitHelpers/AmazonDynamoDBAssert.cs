using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using NUnit.Framework;

namespace D2L.DynamoDBv2.NUnitHelpers {

	public static class AmazonDynamoDBAssert {

		public static async Task AssertItemAsync(
				this IAmazonDynamoDB db,
				string tableName,
				Dictionary<string, AttributeValue> key,
				Dictionary<string, AttributeValue> expectedItem
			) {

			GetItemRequest request = new GetItemRequest {
				TableName = tableName,
				Key = key,
				ConsistentRead = true
			};

			GetItemResponse response = await db
				.GetItemAsync( request )
				.ConfigureAwait( false );

			Assert.That(
					response.IsItemSet,
					Is.True,
					"Item should exist"
				);

			AttributeValueAssert.AreEqual(
					actual: response.Item,
					expected: expectedItem
				);
		}

		public static async Task AssertItemDoesNotExistAsync(
				this IAmazonDynamoDB db,
				string tableName,
				Dictionary<string, AttributeValue> key
			) {

			GetItemRequest request = new GetItemRequest {
				TableName = tableName,
				Key = key,
				ConsistentRead = true
			};

			GetItemResponse response = await db
				.GetItemAsync( request )
				.ConfigureAwait( false );

			Assert.That(
					response.IsItemSet,
					Is.False,
					"Item should not exist"
				);
		}
	}
}
