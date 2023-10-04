using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace s3ImageUploader
{
    public partial class ImageS3Uploader : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (ImageFileUpload.HasFile)
            {
                // Check file size (5MB limit)
                int maxFileSize = 5 * 1024 * 1024; // 5MB
                if (ImageFileUpload.PostedFile.ContentLength > maxFileSize)
                {
                    ResultLabel.Text = "File size exceeds the 5MB limit.";
                    return;
                }

                // Check file extension
                string fileExtension = Path.GetExtension(ImageFileUpload.PostedFile.FileName).ToLower();
                if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                {
                    ResultLabel.Text = "Only .jpg, .jpeg, and .png files are allowed.";
                    return;
                }

                string accessKey = WebConfigurationManager.AppSettings["AWS.AccessKey"];
                string secretKey = WebConfigurationManager.AppSettings["AWS.SecretKey"];
                string region = WebConfigurationManager.AppSettings["AWS.Region"];
                string bucketName = WebConfigurationManager.AppSettings["AWS.BucketName"];
                string folderPath = WebConfigurationManager.AppSettings["AWS.FolderPath"];

                try
                {
                    using (IAmazonS3 client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region)))
                    {
                        using (Stream stream = ImageFileUpload.PostedFile.InputStream)
                        {
                            string fileName = Path.GetFileName(ImageFileUpload.PostedFile.FileName);

                            // Combine the folder path and file name to form the S3 object key
                            string objectKey = folderPath + fileName;

                            TransferUtility fileTransferUtility = new TransferUtility(client);

                            // Upload the file
                            fileTransferUtility.Upload(stream, bucketName, objectKey);

                            // Set public-read ACL for the uploaded object
                            client.PutACL(new PutACLRequest
                            {
                                BucketName = bucketName,
                                Key = objectKey,
                                CannedACL = S3CannedACL.PublicRead
                            });

                            string fileUrl = $"https://{bucketName}.s3.amazonaws.com/{objectKey}";

                            // Get the user's public IP address
                            string userIpAddress = GetUserIPAddress();

                            // Log the date, public IP address, file name, and extension to a log file
                            string logFolderPath = Server.MapPath("~/Logs/");
                            if (!Directory.Exists(logFolderPath))
                            {
                                Directory.CreateDirectory(logFolderPath);
                            }
                            string logFilePath = Path.Combine(logFolderPath, "upload_log.txt");

                            File.AppendAllText(logFilePath, $"{DateTime.Now}: Public IP Address - {userIpAddress}, Uploaded File - {fileName}{Environment.NewLine}");

                            // Create a clickable link to the file URL
                            ResultLabel.Text = $"File uploaded successfully. <br> <br> URL: <a href=\"{fileUrl}\" target=\"_blank\">{fileUrl}</a>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    ResultLabel.Text = $"Error uploading file: {ex.Message}";
                }
            }
            else
            {
                ResultLabel.Text = "Please select a file to upload.";
            }
        }
        private string GetUserIPAddress()
        {
            string userIPAddress = string.Empty;

            string forwardedFor = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                string[] ipArray = forwardedFor.Split(',');
                userIPAddress = ipArray[0].Trim();
            }
            else
            {
                userIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return userIPAddress;
        }
    }
}
