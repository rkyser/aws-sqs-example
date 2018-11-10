using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;

namespace aws_sqs
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MainAsync(args).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        static async Task MainAsync(string[] args)
        {
            var sqsClient = CreateClient();
            var queueUrl = await FindQueueUrlAsync(sqsClient);

            Console.WriteLine($"using queue url: {queueUrl}");

            using (var cancelTokenSource = new CancellationTokenSource())
            {
                var producer = new Producer(sqsClient);
                var producerTask = producer.StartSending(queueUrl, cancelTokenSource.Token);
                
                var consumer = new Consumer(sqsClient);
                var consumerTask = consumer.StartReceiving(queueUrl, cancelTokenSource.Token);

                Console.ReadLine();
            }
        }

        static async Task<string> FindQueueUrlAsync(AmazonSQSClient client)
        {
            var queues = await client.ListQueuesAsync("test");
            return queues.QueueUrls.FirstOrDefault();
        }

        static AmazonSQSClient CreateClient()
        {
            var sqsConfig = new AmazonSQSConfig
            {
                ServiceURL = "http://sqs.us-east-1.amazonaws.com"
            };
            return new AmazonSQSClient(sqsConfig);
        }
    }
}
