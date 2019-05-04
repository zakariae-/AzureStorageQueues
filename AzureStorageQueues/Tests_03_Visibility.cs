using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Xunit;

namespace AzureStorageQueues
{
    public class Tests_03_Visibility
    {
        private readonly CloudQueueClient _cloudClient;

        public Tests_03_Visibility()
        {
            var StorageAccountName = "Your_Storage_Account_Name";
            var StorageAccountKey = "Your_Storage_Account_Key";

            var storageCredentials = new StorageCredentials(StorageAccountName, StorageAccountKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
            _cloudClient = cloudStorageAccount.CreateCloudQueueClient();
        }

        [Fact(DisplayName = "Should Change Visibility Message")]
        public async Task ShouldChangeVisibilityMessage()
        {
            var queueName = "test-queue-messages";
            var queue = _cloudClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            await queue.ClearAsync();

            var msg = "quos imitatae matronae complures opertis capitibus et basternis per latera civitatis cuncta discurrunt";
            var cloudQueueMessage = new CloudQueueMessage(msg);
            await queue.AddMessageAsync(cloudQueueMessage);

            var messageReceived = await queue.GetMessageAsync();
            messageReceived.NextVisibleTime.Should().BeAfter(DateTimeOffset.UtcNow);
            messageReceived.NextVisibleTime.Should().BeBefore(DateTimeOffset.UtcNow.AddSeconds(30));

            await queue.UpdateMessageAsync(messageReceived, TimeSpan.FromSeconds(50), MessageUpdateFields.Visibility);
            messageReceived.NextVisibleTime.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(30));

            await queue.DeleteMessageAsync(messageReceived);
        }
    }
}
