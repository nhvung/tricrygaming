using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class DownloadController : AController
    {
        const long MaxInt32ContentLength = int.MaxValue - 56;
        public DownloadController() : base("DownloadController", VSHost.SERVICE_NAME, VSHost.StaticLogger, VSHost.PRIVATE_KEY)
        {
        }
        protected override Task _ProcessApiContext(string path, string queryString)
        {
            DateTime startTime = DateTime.UtcNow;
            if (path.Equals($"api/download/", StringComparison.InvariantCultureIgnoreCase))
            {
                return Download(startTime);
            }
            return base._ProcessApiContext(path, queryString);
        }
        async Task Download(DateTime startTime)
        {
            try
            {
                string guid = Request.Query["guid"];
                if (!string.IsNullOrWhiteSpace(guid))
                {
                    FileInfo dataFile = new FileInfo($"{WorkingFolder.FullName}/data/{guid}.bin");
                    FileInfo infoFile = new FileInfo($"{WorkingFolder.FullName}/data/{guid}.json");
                    if (dataFile.Exists)
                    {
                        string contentType = VSSystem.Net.ContentType.Stream;
                        List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
                        if (infoFile.Exists)
                        {
                            try
                            {
                                string jsonInfoObj = await System.IO.File.ReadAllTextAsync(infoFile.FullName, Encoding.UTF8, HttpContext.RequestAborted);
                                if (!string.IsNullOrWhiteSpace(jsonInfoObj))
                                {
                                    DataFileInfo infoObj = JsonConvert.DeserializeObject<DataFileInfo>(jsonInfoObj);
                                    if (infoObj != null)
                                    {
                                        if (!string.IsNullOrWhiteSpace(infoObj.ContentType))
                                        {
                                            contentType = infoObj.ContentType;
                                        }
                                        headers.Add(new KeyValuePair<string, string>("content-disposition", $"filename={infoObj.FileName}"));
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                this.LogError(ex);
                            }
                        }
                        using (var stream = dataFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await this.ResponseStreamAsync(stream, contentType, System.Net.HttpStatusCode.OK, headers);
                            stream.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }

            await this.ResponseEmptyAsync(VSSystem.Net.ContentType.Html, System.Net.HttpStatusCode.NotFound);
        }

    }
}