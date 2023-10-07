using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
                // Check file size (2MB limit)
                int maxFileSize = 2 * 1024 * 1024; // 2MB
                if (ImageFileUpload.PostedFile.ContentLength > maxFileSize)
                {
                    // Resize the image
                    using (Stream originalStream = ImageFileUpload.PostedFile.InputStream)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            double scaleFactor = Math.Sqrt((double)maxFileSize / ImageFileUpload.PostedFile.ContentLength);
                            int newWidth = (int)(new Bitmap(originalStream).Width * scaleFactor);
                            int newHeight = (int)(new Bitmap(originalStream).Height * scaleFactor);

                            using (Image originalImage = Image.FromStream(originalStream))
                            using (Bitmap resizedImage = new Bitmap(newWidth, newHeight))
                            using (Graphics graphics = Graphics.FromImage(resizedImage))
                            {
                                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);

                                // Save the resized image to the memory stream
                                resizedImage.Save(memoryStream, ImageFormat.Jpeg);
                                memoryStream.Position = 0;

                                // Upload the resized image
                                string accessKey = WebConfigurationManager.AppSettings["AWS.AccessKey"];
                                string secretKey = WebConfigurationManager.AppSettings["AWS.SecretKey"];
                                string region = WebConfigurationManager.AppSettings["AWS.Region"];
                                string bucketName = WebConfigurationManager.AppSettings["AWS.BucketName"];
                                string folderPath = WebConfigurationManager.AppSettings["AWS.FolderPath"];

                                try
                                {
                                    using (IAmazonS3 client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region)))
                                    {
                                        string fileName = Path.GetFileName(ImageFileUpload.PostedFile.FileName);
                                        string objectKey = folderPath + fileName;

                                        TransferUtility fileTransferUtility = new TransferUtility(client);

                                        // Upload the resized image
                                        fileTransferUtility.Upload(memoryStream, bucketName, objectKey);

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

                                        // clickable link to the file URL
                                        ResultLabel.Text = $"File uploaded successfully. <br> <br> URL: <a href=\"{fileUrl}\" target=\"_blank\">{fileUrl}</a>";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ResultLabel.Text = $"Error uploading file: {ex.Message}";
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Upload the original image without resizing
                    string accessKey = WebConfigurationManager.AppSettings["AWS.AccessKey"];
                    string secretKey = WebConfigurationManager.AppSettings["AWS.SecretKey"];
                    string region = WebConfigurationManager.AppSettings["AWS.Region"];
                    string bucketName = WebConfigurationManager.AppSettings["AWS.BucketName"];
                    string folderPath = WebConfigurationManager.AppSettings["AWS.FolderPath"];

                    try
                    {
                        using (IAmazonS3 client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region)))
                        {
                            string fileName = Path.GetFileName(ImageFileUpload.PostedFile.FileName);
                            string objectKey = folderPath + fileName;

                            TransferUtility fileTransferUtility = new TransferUtility(client);

                            // Upload the original image
                            using (Stream stream = ImageFileUpload.PostedFile.InputStream)
                            {
                                fileTransferUtility.Upload(stream, bucketName, objectKey);
                            }

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

                            // Clickable link to the file URL
                            ResultLabel.Text = $"File uploaded successfully. <br> <br> URL: <a href=\"{fileUrl}\" target=\"_blank\">{fileUrl}</a>";
                        }
                    }
                    catch (Exception ex)
                    {
                        ResultLabel.Text = $"Error uploading file: {ex.Message}";
                    }
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
