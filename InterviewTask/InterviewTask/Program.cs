using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace InterviewTask
{
    class Program
    {
        //Defination of global variables.
        const string fileName = "InputText2";
        const string filePath = "..\\..\\..\\InputText2.txt";
        const string awsAccessKeyId = "AKIA5Y3Y6ZSDMVXKBYGD";
        const string awsSecretAccessKey = "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";
        const string bucketName = "tamaraharoni6bucket";

        /// <summary>
        /// Function to count words in the file text.
        /// </summary>
        public static void countWords()
        {
            //Where to save the information before uploading to my S3 bucket.
            string resultFile = $"{fileName}_result.json";
            string text;
            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            try
            {
                // Open the file for reading
                using (StreamReader sr = new StreamReader(filePath))
                {
                    //Content of the file.
                    text = File.ReadAllText(filePath);

                    //Read the content of the file and count word occurrences
                    string[] words = text.Split(new[] { ' ', '\r', '\n', '.', ',', '?' }, StringSplitOptions.RemoveEmptyEntries);

                    //Count words.
                    int count = words.Length;

                    //Count word occurrences
                    wordCount = words.GroupBy(x => x)
                        .ToDictionary(g => g.Key, g => g.Count());

                    //Write to local file.
                    using (StreamWriter rf = new StreamWriter(resultFile, true))
                    {
                        rf.WriteLine(JsonConvert.SerializeObject(wordCount));
                        rf.WriteLine(JsonConvert.SerializeObject(count));
                        rf.Close();//Closed file.
                        rf.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                LogUtility.AddToLog("exception: " + e.Message);
            }

            UploadFileToS3(resultFile);
        }

        /// <summary>
        /// Function to save result in my S3 bucket.
        /// </summary>
        public static void UploadFileToS3(string resultFile)
        {
            try
            {
                //Defination of my acount in aws and connecting to him.
                using (var client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.USEast1))
                {
                    var fileTransferUtility = new TransferUtility(client);

                    //Upload file to my S3 bucket.
                    fileTransferUtility.Upload(resultFile, bucketName);

                    LogUtility.AddToLog("File uploaded successfully to S3 bucket: " + bucketName);
                }
            }
            catch (AmazonS3Exception ex)
            {
                LogUtility.AddToLog("Error uploading file: " + ex.Message);
            }
        }

        /// <summary>
        /// The function display the result for a specific file, byread it from my S3 bucket.
        /// </summary>
        /// <returns>status</returns>
        public static async Task<JsonDocument> GetResultFromS3(string fileName)
        {
            Console.WriteLine($"Retrieving word count results for {fileName} from S3...");

            //Defination of my acount in aws and connecting to him.
            using (var client = new AmazonS3Client(awsAccessKeyId,awsSecretAccessKey, RegionEndpoint.USEast1))
            {
                //Defination of the specific file that I want to show.
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = $"{fileName}_result.json"
                };

                //Retrieving the data from S3 according to the settings of the specific file to response.
                using (GetObjectResponse response = await client.GetObjectAsync(request))
                {
                    using (Stream responseStream = response.ResponseStream)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            string jsonContent = await reader.ReadToEndAsync();
                            return JsonDocument.Parse(jsonContent);
                        }
                    }
                }
            }
        }


        static async Task Main(string[] args)
        {
            //An example of using the various functions 
            countWords();
            try
            {
                JsonDocument jsonDocument = await GetResultFromS3(fileName);

                // The JSON data.
                // For example, the print on console.
                foreach (var property in jsonDocument.RootElement.EnumerateObject())
                {
                    Console.WriteLine($"{property.Name}: {property.Value}");
                }
            }
            catch (Exception ex)
            {
                LogUtility.AddToLog($"An error occurred: {ex.Message}");
            }
        }
    }
}

