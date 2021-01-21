using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using SysUtility.Config.Interfaces;
using SysUtility.Config.Models;

namespace WeighingSystemUPPCV3_5_Repository.Plugins
{
    public class GoogleDrive
    {
        protected Google.Apis.Auth.OAuth2.UserCredential userCredential;
        protected Google.Apis.Drive.v3.DriveService driveService;

        protected bool Validated;
        protected string ValidationMessage;

        private readonly GoogleDriveParam googleDriverParam;

        public GoogleDrive(GoogleDriveParam googleDriveParam)
        {
            this.googleDriverParam = googleDriveParam;
        }

        public bool ValidateCredential()
        {
            var res = true;
            try
            {

                Google.Apis.Auth.OAuth2.UserCredential credential =
                 Google.Apis.Auth.OAuth2.GoogleWebAuthorizationBroker.AuthorizeAsync(
                   new Google.Apis.Auth.OAuth2.ClientSecrets
                   {
                       ClientId = googleDriverParam.ClientId,
                       ClientSecret = googleDriverParam.ClientSecret,
                   },
                   new[] { Google.Apis.Drive.v3.DriveService.Scope.Drive },
                   "user",
                   System.Threading.CancellationToken.None).Result;

                userCredential = credential;

                var service = new Google.Apis.Drive.v3.DriveService(new Google.Apis.Services.BaseClientService.Initializer()
                {
                    HttpClientInitializer = userCredential,
                    ApplicationName = googleDriverParam.ApplicationName
                });

                driveService = service;
                Validated = true;
            }
            catch (Exception e)
            {
                Validated = false;
                ValidationMessage = e.Message;
                res = false;
            }
            finally
            {

            }

            return res;
        }

        public void UploadFile(GoogleDriveMetaDataModel dataFile)
        {
            Google.Apis.Drive.v3.Data.File file = new Google.Apis.Drive.v3.Data.File();
            file.Name = dataFile.Name;
            file.Description = dataFile.Description;
            file.MimeType = dataFile.MimeType;
            file.Parents = new List<string>();
            if (string.IsNullOrEmpty(dataFile.ParentFolderId) == false) file.Parents.Add(dataFile.ParentFolderId);

            byte[] byteArray = System.IO.File.ReadAllBytes(dataFile.FileName);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

            var uploadedFile = driveService.Files.Create(file, stream, dataFile.MimeType);

            Google.Apis.Drive.v3.FilesResource.CreateMediaUpload request = driveService.Files.Create(file, stream, dataFile.MimeType);
            request.ChunkSize = 1024 * 1024;
            request.Upload();
        }

        public string SearchFolder(string folderName)
        {
            try
            {
                string resultFolderId = null;
                string parentId = "root";
                string MMType = "application/vnd.google-apps.folder";
                var request = driveService.Files.List();
                    request.Q = "trashed=false"; // undeleted items
                request.Q += string.Format(" and '{0}' in parents", parentId); // https://developers.google.com/drive/search-parameters
                request.Q += !string.IsNullOrEmpty(folderName) ? string.Format(" and name = '{0}'", folderName) : string.Empty;
                request.Q += !string.IsNullOrEmpty(MMType) ? string.Format(" and mimeType = '{0}'", MMType) : string.Empty;

                var fileList = request.Execute();
                if (fileList.Files.Count != 0)
                {
                    foreach (var file in fileList.Files) resultFolderId = file.Id;
                }
                return resultFolderId;
            }
            catch (Exception e)
            { throw new Exception(e.Message); }
            finally
            { }
        }

        public string CreateFolder(string folderName)
        {
            try
            {
                Google.Apis.Drive.v3.Data.File fileMetaData = new Google.Apis.Drive.v3.Data.File();
                fileMetaData.Name = folderName;
                fileMetaData.MimeType = "application/vnd.google-apps.folder";
                Google.Apis.Drive.v3.Data.File file = driveService.Files.Create(fileMetaData).Execute();
                return file.Id;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {

            }
        }
    }

    public class GoogleDriveMetaDataModel
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public string MimeType { get; set; }

        public string FileName { get; set; }

        public string ParentFolderId { get; set; }

    }

    internal class FileMimeTypes
    {
        internal static string FolderMimeType = "application/vnd.google-apps.folder";

        IDictionary<string, string> dictionary = new Dictionary<string, string>()
        {
            {"pdf", "application/pdf"},
            {"txt", "text/plain"},
            {"doc", "application/doc"},
            {"docx", "application/docx"},
            {"xls", "application/xls"},
            {"xlsx", "application/xlsx"},
            {"ppt", "application/ppt"},
            {"pptx", "application/pptx"},
            {"folder","application/vnd.google-apps.folder"},
            {"default","application/octet-stream"},
            {"rar","application/rar"},
            {"zip","application/zip"},
            {"csv","text/plain"}
                //Tagged Image File Format (.TIFF)
                //Scalable Vector Graphics (.SVG)
                //PostScript (.EPS, .PS)
                //TrueType (.TTF)
                //XML Paper Specification (.XPS)
        };

        internal string this[string extension]
        {
            get
            {
                return dictionary.ContainsKey(extension) ? dictionary[extension] : null;
            }
        }

    }

}
