using StoreSystem.Database;

namespace StoreSystem.Services
{
    public class FileService
    {
        private readonly StoreSystemContext _db;

        public FileService(StoreSystemContext db) {
            _db = db;
        }

        public (byte[],string) GetImage(long id)
        {
            var file = _db.ProductFiles.FirstOrDefault(x => x.ProductId == id);
            if (file == null) throw new Exception("檔案不存在");
            var imageBytes = File.ReadAllBytes(file.Path);
            var contentType = GetContentType(file.Path);
            return (imageBytes,contentType);
        }

        /// <summary>
        /// 依副檔名判斷 Content-Type
        /// </summary>
        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream",
            };
        }
    }
}
