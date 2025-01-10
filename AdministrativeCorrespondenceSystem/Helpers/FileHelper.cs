namespace AdministrativeCorrespondenceSystem.Helpers
{
    public class FileHelper
    {
        public static string SaveFile(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0) return null;

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

    }
}
