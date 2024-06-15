namespace DataTransferAPILib.Models
{
    [Newtonsoft.Json.JsonObject(ItemNullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public class DataFileInfo
    {
        string _FileName;
        public string FileName { get { return _FileName; } set { _FileName = value; } }
        string _Length;
        public string Length { get { return _Length; } set { _Length = value; } }
        string _ContentType;
        public string ContentType { get { return _ContentType; } set { _ContentType = value; } }
    }
}