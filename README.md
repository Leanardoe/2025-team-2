# 🚀 ResumeSystem Azure Deployment Guide

This guide walks you through deploying the ResumeSystem ASP.NET Core MVC app to a new Azure account.

## ✅ Prerequisites

- An [Azure account](https://azure.microsoft.com)
- Visual Studio 2022+ or .NET SDK installed
- Azure CLI or Azure Portal access
- GitHub repository (for deployment)
- OpenAI API Key

---

## 🔧 1. Set Up Azure Blob Storage

1. Go to the [Azure Portal](https://portal.azure.com)
2. Create a **Storage Account** (e.g., `resumestorage123`)
3. Under the account, create a **Blob Container** named `resumes`
4. Set **Public Access Level** to **Private (no anonymous access)**
5. Under **Access Keys**, copy the **connection string**

---

## 🚀 2. Set Up Azure App Service

1. Create a new **App Service**
   - Use Windows OS and .NET 8 runtime stack
2. Deploy via:
   - GitHub Actions (recommended)
   - Zip deploy via Visual Studio or Azure CLI

---

## ⚙️ 3. Configure App Settings

In **Azure Portal > App Service > Configuration > Application settings**, add:

| Key                           | Value                                           |
|-------------------------------|-------------------------------------------------|
| `AzureStorage`                | *(blob connection string)*                     |
| `AzureStorage:ConnectionString` | *(same as above, for DI setup)*              |
| `OpenAI:ApiKey`               | *(your OpenAI API Key)*                        |
| `AI:Prompt`                   | `Extract name, phone, email, and skills.`      |

📝 **Remember to click 'Save' after adding settings.**

---

## 🔄 4. Enable Required Features

Ensure your App Service has:

- .NET 8+ Runtime
- Application Logging is enabled for diagnostics

---

# Secure Resume Filtering System - How To Use

### A secure, automated tool for recruiters to efficiently filter and rank internship candidates based on resume content matching predefined criteria.

---

## How to Use the App

### 1. Signing In

When first entering the application, you will be prompted with the following screen:

<img src="https://github.com/Leanardoe/2025-team-2/blob/main/ResumeSystem/Screens/SignIn.jpg" alt="Sign-In Screen" width="800px" />

After pressing the "Sign In" button, You will be presented with the following page:

<img src="https://github.com/Leanardoe/2025-team-2/blob/main/ResumeSystem/Screens/Signin-2.jpg" alt="Sign-In Screen" width="800px" />

To access the application, enter your credentials, and click the Sign In button to log in securely.

---

### 2. Uploading Resumes

#### Single Resume Upload

<img src="https://github.com/Leanardoe/2025-team-2/blob/main/ResumeSystem/Screens/Resume-Single.jpg" alt="" width="800px" />

-  Navigate to the **Resume Upload** section.
-  Fill in your name and email address.
-  Click **Choose File** to select a resume file (supported formats: PDF, DOCX, TXT).
-  Click **Upload Resume** to submit the file.

#### Mass Resume Upload

<img src="https://github.com/Leanardoe/2025-team-2/blob/main/ResumeSystem/Screens/Resume-Multiple.jpg" alt="" width="800px" />

1. Navigate to the **Mass Upload** section.
2. Fill in your name and email address.
3. Click **Choose Files** to select multiple resume files.
4. Click **Upload Resumes** to process all selected files.

---

### 3. Filtering Resumes

1. Navigate to the **Resume Filtering** section.
2. Enter keywords separated by commas in the **Search Keywords** field.
3. Use the **Filter By** dropdown to refine your search (e.g., most matches).
4. Click **Search** to view candidates that match your criteria.
