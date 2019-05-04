using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Xunit;

namespace AzureStorageQueues
{
    public class Tests_02_Messages
    {
        private readonly CloudQueueClient _cloudClient;

        public Tests_02_Messages()
        {
            var StorageAccountName = "Your_Storage_Account_Name";
            var StorageAccountKey = "Your_Storage_Account_Key";

            var storageCredentials = new StorageCredentials(StorageAccountName, StorageAccountKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
            _cloudClient = cloudStorageAccount.CreateCloudQueueClient();
        }

        [Fact(DisplayName = "Should Send Message")]
        public async Task ShouldSendMessage()
        {
            var queueName = "test-queue-messages";
            var queue = _cloudClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            await queue.ClearAsync();

            var msg = "quos imitatae matronae complures opertis capitibus et basternis per latera civitatis cuncta discurrunt";
            var cloudQueueMessage = new CloudQueueMessage(msg);
            await queue.AddMessageAsync(cloudQueueMessage);
        }

        [Fact(DisplayName = "Should Receive Message")]
        public async Task ShouldReceiveMessage()
        {
            var queueName = "test-queue-messages";
            var queue = _cloudClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            await queue.ClearAsync();

            var msg = "quos imitatae matronae complures opertis capitibus et basternis per latera civitatis cuncta discurrunt";
            var cloudQueueMessage = new CloudQueueMessage(msg);
            await queue.AddMessageAsync(cloudQueueMessage);

            var messageReceived = await queue.GetMessageAsync();
            messageReceived.AsString.Should().BeEquivalentTo(msg);
            messageReceived.DequeueCount.Should().Equals(1);

            await queue.DeleteMessageAsync(messageReceived);
        }
    }
}
