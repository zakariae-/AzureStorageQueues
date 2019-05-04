using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Xunit;

namespace AzureStorageQueues
{
    public class Test_01_ConnectTo
    {
        [Fact(DisplayName = "Should Connect To Localhost")]
        public async Task ShouldConnectToLocalhost()
        {
            var StorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var client = StorageAccount.CreateCloudQueueClient();

            var queueName = "test-queue";
            var queue = client.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            var isExist = await queue.ExistsAsync();

            isExist.Should().BeTrue();
            queue.Name.Should().BeEquivalentTo(queueName);
        }

        [Fact(DisplayName = "Should Connect To Cloud")]
        public async Task ShouldConnectToCloud()
        {
            var StorageAccountName = "Your_Storage_Account_Name";
            var StorageAccountKey = "Your_Storage_Account_Key";

            var storageCredentials = new StorageCredentials(StorageAccountName, StorageAccountKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
            var cloudClient = cloudStorageAccount.CreateCloudQueueClient();

            var queueName = "test-queue";
            var queue = cloudClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            var isExist = await queue.ExistsAsync();

            isExist.Should().BeTrue();
            queue.Name.Should().BeEquivalentTo(queueName);
        }
    }
}
