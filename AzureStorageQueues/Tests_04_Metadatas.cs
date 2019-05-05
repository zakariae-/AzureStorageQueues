using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Xunit;

namespace AzureStorageQueues
{
    public class Tests_04_Metadatas
    {
        private readonly CloudQueueClient _cloudClient;

        public Tests_04_Metadatas()
        {
            var StorageAccountName = "Your_Storage_Account_Name";
            var StorageAccountKey = "Your_Storage_Account_Key";

            var storageCredentials = new StorageCredentials(StorageAccountName, StorageAccountKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
            _cloudClient = cloudStorageAccount.CreateCloudQueueClient();
        }

        [Fact(DisplayName = "Should Add Or Update Metadata")]
        public async Task ShouldAddOrUpdateMetadata()
        {
            var queueName = "test-queue-messages";
            var queue = _cloudClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            await queue.ClearAsync();

            var visibilityTimeout = TimeSpan.FromSeconds(50).ToString();
            queue.Metadata.Add(
                new KeyValuePair<string, string>("VisibilityTimeout", visibilityTimeout));

            await queue.SetMetadataAsync();
        }

        [Fact(DisplayName = "Should get Metadata")]
        public async Task ShouldGetMetadata()
        {
            var visibilityTimeout = TimeSpan.FromSeconds(50).ToString();
            var queueName = "test-queue-messages";
            var queue = _cloudClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            await queue.ClearAsync();

            await queue.FetchAttributesAsync();

            queue.Metadata["VisibilityTimeout"].Should().NotBeNull();
            queue.Metadata["VisibilityTimeout"].Should().Equals(visibilityTimeout);
        }

    }
}
