using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;

namespace aws_sqs
{
    public class Producer
    {
        private readonly AmazonSQSClient sqsClient;

        public Producer(AmazonSQSClient sqsClient)
        {
            this.sqsClient = sqsClient;
        }

        public async Task StartSending(string queueUrl, CancellationToken token)
        {
            var counter = 0;
            while (!token.IsCancellationRequested)
            {
                await sqsClient.SendMessageAsync(queueUrl, $"{{count: {counter++}}}", token);
                await Task.Delay(1000, token);
            }
        }
    }
}
