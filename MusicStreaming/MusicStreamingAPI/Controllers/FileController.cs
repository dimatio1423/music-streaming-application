using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IAmazonS3 _amazonS3;

        public FileController(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string? prefix)
        {
            //TimeSpan duration;

            TimeOnly finalDuration;

            using (var stream = file.OpenReadStream())
            {
                var tfile = TagLib.File.Create(new TagLib.StreamFileAbstraction(file.FileName, stream, stream));
                TimeSpan duration = tfile.Properties.Duration;

                var roundedDuration = TimeSpan.FromSeconds(Math.Ceiling(duration.TotalSeconds));

                // Format the duration as "hh:mm:ss"
                finalDuration = new TimeOnly(roundedDuration.Hours, roundedDuration.Minutes, roundedDuration.Seconds);
            }

            var bucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, "music-streaming-application");
            if (!bucketExist) return NotFound("Bucket does not exist");

            if (file.FileName.ToLowerInvariant().Contains("y2meta.com"))
            {
                return BadRequest("Invalid music file, file names containing 'y2meta.com' are not allowed.");
            }

            prefix = Guid.NewGuid().ToString();

            var request = new PutObjectRequest
            {
                BucketName = "music-streaming-application",
                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix}-{file.FileName}",
                InputStream = file.OpenReadStream()
            };

            request.Metadata.Add("Content-Type", file.ContentType);
            var uploadResult = await _amazonS3.PutObjectAsync(request);

            //var s3Oject = await _amazonS3.GetObjectAsync("music-streaming-application", request.Key);
            //return File(s3Oject.p, s3Oject.Headers.);

            if (uploadResult.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                // Generate the URL of the uploaded file
                var fileUrl = $"https://{"music-streaming-application"}.s3.ap-southeast-2.amazonaws.com/{request.Key}";

                // Return the file URL
                return Ok(new { Url = fileUrl, Duration = finalDuration });
            }

            return StatusCode((int)uploadResult.HttpStatusCode, "Failed to upload file");
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllFiles( string? prefix)
        //{
        //    var bucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, "music-streaming-application");
        //    if (!bucketExist) return NotFound("Bucket does not exist");

        //    var request = new ListObjectsV2Request
        //    {
        //        BucketName = "music-streaming-application",
        //        Prefix = prefix
        //    };


        //    var result = await _amazonS3.ListObjectsV2Async(request);

        //    var s3Objects = result.S3Objects.Select(x =>
        //    {
        //        var url = new GetPreSignedUrlRequest()
        //        {
        //            BucketName = "music-streaming-application",
        //            Key = x.Key,
        //            Expires = DateTime.Now.AddMinutes(1)
        //        };

        //        return new
        //        {
        //            Name = x.Key.ToString(),
        //            PresignURL = _amazonS3.GetPreSignedURL(url),
        //        };
        //    });

        //    return Ok(s3Objects);
        //}
    }
}
