var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("upload", async (IFormFile file) =>
{
    IAmazonS3 client = new AmazonS3Client();
    var transferUtil = new TransferUtility(client);

    await using var memoryStream = new MemoryStream();
    await file.CopyToAsync(memoryStream);

    var bucketName = "s3-dotnet-eru-testing-testbucket";
    var key = file.FileName;

    try
    {
        await transferUtil.UploadAsync(new TransferUtilityUploadRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = memoryStream
        });

        return true;
    }
    catch (AmazonS3Exception s3Ex)
    {
        Console.WriteLine(s3Ex.Message);
        return false;
    }
});
app.MapGet("download", async (string fileName) =>
{
    IAmazonS3 client = new AmazonS3Client();
    var transferUtil = new TransferUtility(client);
    var bucketName = "s3-dotnet-eru-testing-testbucket";
    

    try
    {
        await transferUtil.DownloadAsync(new TransferUtilityDownloadRequest
        {
            BucketName = bucketName,
            Key = fileName,
            FilePath = $"test/{fileName}"
        });

        return true;
    }
    catch (AmazonS3Exception s3Ex)
    {
        Console.WriteLine(s3Ex.Message);
        return false;
    }

});

app.Run();
