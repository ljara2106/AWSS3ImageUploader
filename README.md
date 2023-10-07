# AWS S3 Bucket Image Uploader Tool

![GitHub last commit](https://img.shields.io/github/last-commit/ljara2106/AWSS3ImageUploader)

## Overview

The AWS S3 Bucket Image Uploader Tool is a web-based application that allows end-users to easily upload images to an Amazon S3 bucket and specify a specific path within the bucket. It is designed to simplify the process of uploading images to your AWS S3 storage while providing essential features and restrictions.

## Features

- Securely upload images to an AWS S3 bucket.
- Specify a custom path within the bucket for organized storage.
- Supports image file types: JPG, PNG, and JPEG.
- Enforces a size limit of 5MB per image upload.
- Provides error feedback for incorrect file size or file type.

## Configuration

To use this tool, you'll need to configure the following settings in the `web.config` file:

- AWS Access Key ID
- AWS Secret Access Key
- AWS Region
- Folder path within the S3 bucket

Make sure to keep your configuration secure and never share your AWS credentials publicly.

## Installation

1. Clone this repository to your local machine:

   ```bash
   git clone https://github.com/ljara2106/AWSS3ImageUploader.git
   ```

2. Open the project in your preferred development environment.

3. Configure the `web.config` file with your AWS credentials and desired settings.

4. Deploy the application to your web server or hosting platform of choice.

## Usage

1. Access the application in your web browser.

2. Select the image file you want to upload.

3. Click the "Upload" button.

4. If the image meets the specified criteria (file type and size), it will be uploaded to the specified S3 bucket path. Otherwise, you will receive an error message indicating the issue.

