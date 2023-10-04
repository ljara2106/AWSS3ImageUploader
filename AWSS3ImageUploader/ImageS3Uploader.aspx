<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImageS3Uploader.aspx.cs" Inherits="s3ImageUploader.ImageS3Uploader" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>S3 - Image Uploader</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            text-align: center;
            background-color: #f4f4f4;
        }
        .container {
            margin: 50px auto;
            max-width: 600px;
            background-color: #fff;
            border: 1px solid #ccc;
            border-radius: 5px;
            padding: 20px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        .logo {
            width: 200px;
            height: 50px;
            margin: 0 auto 20px;
        }
        .title {
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 20px;
        }
        .upload-button {
            margin-top: 20px;
        }
        .result-box {
            margin-top: 20px;
            padding: 10px;
            background-color: #f9f9f9;
            border: 1px solid #ddd;
            border-radius: 5px;
            text-align: left;
        }
        .file-restrictions {
            font-size: 16px;
            color: #777;
            margin-bottom: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <img class="logo" src="amazon-s3-logo.png" alt="Logo" />
            <div class="title">S3 - Image Uploader</div>
            
            <!-- File Restrictions Message -->
            <div class="file-restrictions">
                Before uploading, please be aware of the following restrictions:
                <br />
                - Allowed file types: .jpg, .jpeg, .png
                <br />
                - Maximum file size: 5MB
            </div>
            
            <asp:FileUpload ID="ImageFileUpload" runat="server" accept=".jpg, .png, .jpeg" />
            <asp:Button ID="UploadButton" runat="server" Text="Upload" OnClick="UploadButton_Click" CssClass="upload-button" />
            <div class="result-box">
                <asp:Label ID="ResultLabel" runat="server" Text=""></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
