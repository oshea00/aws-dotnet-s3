using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace S3CreateAndList
{
    class Program
    {
        static Logger log = new LoggerConfiguration()
                                .WriteTo.Console()
                                .CreateLogger();

        static IConfigurationRoot configuration =
            new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile($"appsettings.json")
                .Build();

        static void Main(string[] args)
        {
            try {

                var awsOptions = configuration.GetAWSOptions();

                using (var s3Client = awsOptions.CreateServiceClient<IAmazonS3>()) {

                    var bucketName = "s3create-program-example-73145";

                    var bucketResp = s3Client.PutBucketAsync(
                        new PutBucketRequest{
                            BucketName = bucketName,
                            BucketRegionName = "us-west-2"
                        } 
                    ).Result;

                    log.Information("{@BucketResponse}",bucketResp);

                    if (bucketResp.HttpStatusCode == HttpStatusCode.OK) {
                        log.Information("Bucket Created!");
                    }

                    foreach (var b in s3Client.ListBucketsAsync().Result.Buckets) {
                        System.Console.WriteLine(b.BucketName);
                    }

                    var delResp = s3Client.DeleteBucketAsync(bucketName).Result;

                    log.Information("Done!");
                }

            } catch (AmazonS3Exception ex) {
                log.Error($"{ex.ErrorCode}");
            }
       }

    }
}
