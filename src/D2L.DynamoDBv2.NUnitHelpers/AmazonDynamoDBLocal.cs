using System;
using Amazon.DynamoDBv2;
using Amazon.Runtime;

namespace D2L.DynamoDBv2.NUnitHelpers {

	public static class AmazonDynamoDBLocal {

		/// <summary>
		/// Creates an <see cref="IAmazonDynamoDB"/> client for DynamoDB local.
		/// 
		/// The service url is defined by the environment variable DYNAMODB_LOCAL_SERVICE_URL if set; otherwise <c>http://localhost:8000</c> will be used.
		/// </summary>
		/// <returns>Returns an Amazon DynamoDB client.</returns>
		public static IAmazonDynamoDB CreateClient() {

			string? serviceUrl = Environment.GetEnvironmentVariable( "DYNAMODB_LOCAL_SERVICE_URL" );
			if( serviceUrl == null ) {
				serviceUrl = "http://localhost:8000";
			}

			return new AmazonDynamoDBClient(
					credentials: new BasicAWSCredentials(
						accessKey: "accessKey",
						secretKey: "secretKey"
					),
					clientConfig: new AmazonDynamoDBConfig {
						ServiceURL = serviceUrl
					}
				);
		}
	}
}
