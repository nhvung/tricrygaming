using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DataTransferAPILib.Models;
using Newtonsoft.Json;
using VSSystem;
using VSSystem.Extensions.Hosting.Controllers;
using VSSystem.Extensions.Hosting.Models;

namespace DataTransferAPILib.Controllers
{
    public class UploadController : AController
    {
        const long MaxInt32ContentLength = int.MaxValue - 56;
        public UploadController() : base("UploadController", VSHost.SERVICE_NAME, VSHost.StaticLogger, VSHost.PRIVATE_KEY)
        {
        }
        protected override Task _ProcessApiContext(string path, string queryString)
        {
            DateTime startTime = DateTime.UtcNow;
            if (path.Equals($"api/upload/", StringComparison.InvariantCultureIgnoreCase))
            {
                return Upload(startTime);
            }
            return base._ProcessApiContext(path, queryString);
        }
        async Task Upload(DateTime startTime)
        {
            try
            {
                if (Request.ContentLength > 0)
                {
                    string guid = IDGenerator.ConvertToGuid(startTime.Ticks, startTime.Ticks);
                    await _saveToFile(guid);
                    object responseObj = new
                    {
                        downloadUrl = $"/api/download?guid={guid}"
                    };
                    await this.ResponseJsonAsync(responseObj, System.Net.HttpStatusCode.OK);
                    return;
                }
                else
                {
                    await this.ResponseJsonAsync(DefaultResponse.InvalidParameters, System.Net.HttpStatusCode.BadRequest);
                    return;
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
            await this.ResponseEmptyAsync(VSSystem.Net.ContentType.Html, System.Net.HttpStatusCode.OK);
        }
        async Task _saveToFile(string guid)
        {
            try
            {
                FileInfo dataFile = new FileInfo($"{WorkingFolder.FullName}/data/{guid}.bin");
                FileInfo infoFile = new FileInfo($"{WorkingFolder.FullName}/data/{guid}.json");
                if (!infoFile.Directory.Exists)
                {
                    infoFile.Directory.Create();
                }
                using (var stream = dataFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    await Request.Body.CopyToAsync(stream, HttpContext.RequestAborted);
                    stream.Close();
                }

                DataFileInfo infoObj = new DataFileInfo
                {
                    FileName = Request.Headers["fileName"],
                    ContentType = Request.Headers["fileType"],
                    Length = Request.Headers["fileLength"],

                };

                await System.IO.File.WriteAllTextAsync(infoFile.FullName, JsonConvert.SerializeObject(infoObj), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }
    }
}