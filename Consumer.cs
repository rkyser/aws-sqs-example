using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;

namespace aws_sqs
{
    public class Consumer
    {
        private readonly AmazonSQSClient sqsClient;

        public Consumer(AmazonSQSClient sqsClient)
        {
            this.sqsClient = sqsClient;
        }

        public async Task StartReceiving(string queueUrl, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var res = await sqsClient.ReceiveMessageAsync(queueUrl, token);
                
                if (res.HttpStatusCode == HttpStatusCode.OK)
                {
                    foreach (var message in res.Messages)
                    {
                        Console.WriteLine($"received: {message.Body}");

                        await sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle, token);
                    }
                }

                await Task.Delay(800, token);
            }
        }
    }
}
