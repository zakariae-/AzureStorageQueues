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
    public class Tests_05_SharedAccessSignature
    {
        private readonly string _sas_read;
        private readonly string _sas_add;

        public Tests_05_SharedAccessSignature()
        {
            var StorageAccountName = "Your_Storage_Account_Name";
            var StorageAccountKey = "Your_Storage_Account_Key";

            var storageCredentials = new StorageCredentials(StorageAccountName, StorageAccountKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
            var cloudClient = cloudStorageAccount.CreateCloudQueueClient();

            var queueName = "test-queue-sharedaccesssignature";
            var queue = cloudClient.GetQueueReference(queueName);
            queue.CreateIfNotExistsAsync().Wait();
            queue.ClearAsync().Wait();

            _sas_read = queue.GetSharedAccessSignature(
                new SharedAccessQueuePolicy()
                {
                    Permissions = SharedAccessQueuePermissions.Read,
                    SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddSeconds(50)
                });

            _sas_add = queue.GetSharedAccessSignature(
                new SharedAccessQueuePolicy() 
                {
                    Permissions = SharedAccessQueuePermissions.Add,
                    SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddSeconds(50)
                });
        }

        [Fact(DisplayName = "Add Message")]
        public async Task ShouldAddMessage()
        {
            var cloudQueueClient = new CloudQueueClient(
                new Uri("Your_Queue_Uri"),
                new StorageCredentials(_sas_add));

            var queue = cloudQueueClient.GetQueueReference("test-queue-sharedaccesssignature");

            var msg = "quos imitatae matronae complures opertis capitibus et basternis per latera civitatis cuncta discurrunt";
            var cloudQueueMessage = new CloudQueueMessage(msg);
            await queue.AddMessageAsync(cloudQueueMessage);
        }

        [Fact(DisplayName = "Can't verify the Queue existence")]
        public async Task ShouldNotVerifyQueueExistence()
        {
            var cloudQueueClient = new CloudQueueClient(
                new Uri("Your_Queue_Uri"), 
                new StorageCredentials(_sas_add));

            var queue = cloudQueueClient.GetQueueReference("test-queue-sharedaccesssignature");

            await Assert.ThrowsAsync<StorageException>(() => queue.ExistsAsync());
        }

        [Fact(DisplayName = "Should Verify the Existence")]
        public async Task ShouldVerifyExistence()
        {
            var cloudQueueClient = new CloudQueueClient(
                new Uri("Your_Queue_Uri"),
                new StorageCredentials(_sas_read));

            var queue = cloudQueueClient.GetQueueReference("test-queue-sharedaccesssignature");

            var isExist = await queue.ExistsAsync();
            isExist.Should().BeTrue();
        }
    }
}
