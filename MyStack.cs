using Pulumi;
using Pulumi.Aws.DataSync;
using Pulumi.Aws.S3;
using Pulumi.Aws.S3.Inputs;

class MyStack : Stack
{
    public MyStack()
    {
        // Create an AWS resource (S3 Bucket)
        var bucket = new Bucket("s3.bucket.logs", new BucketArgs
        {
            
            Acl = "private",
            Tags =
            {
                {"Environment","Dev"},
                {"Name","Ornek Bucket"}
            },
            
            //TODO: Burası için konuşulması gerekiyor. 30 gün sonra bucket içerisindeki verilerin silinme ihtimali söz konusu.
            LifecycleRules = new BucketLifecycleRuleArgs()
            {
                Enabled = true,
                Expiration = new BucketLifecycleRuleExpirationArgs()
                {
                    Days = 60,
                },
                Id = "log",
                Prefix = "log/",
                Tags = {{"UseCase","log"}},
                Transitions =
                {
                    new BucketLifecycleRuleTransitionArgs()
                    {
                        Days = 30,
                        StorageClass = "INTELLIGENT_TIERING",
                    },
                }
            },
        });
        //Intelligent Tiering Config ayarını yapıyoruz.
        var intelligentTieringConfiguration = new BucketIntelligentTieringConfiguration("s3.bucket.logs-intelligent-tiering",
            new BucketIntelligentTieringConfigurationArgs()
            {
                Bucket = bucket.BucketName,
                Tierings =
                {
                    new BucketIntelligentTieringConfigurationTieringArgs()
                    {
                        AccessTier = "ARCHIVE_ACCESS",
                        Days = 90,
                    },
                }
            });
        //Public erişimi kısıtlıyoruz.
        var publicAccessBlock = new BucketPublicAccessBlock("ornek-bucketPublicAccessBlock",
            new BucketPublicAccessBlockArgs()
            {
                Bucket = bucket.BucketName,
                BlockPublicAcls = true,
                BlockPublicPolicy = true,
            });
        //Bucket objelerini private yapmak ??.
        var bucketObjects = new BucketObject("bucket-objem", new BucketObjectArgs()
        {
            Bucket = bucket.BucketName,
            Acl = "private",
        });
        // Export the name of the bucket
        this.BucketName = bucket.Id;
    }

    [Output]
    public Output<string> BucketName { get; set; }
}
