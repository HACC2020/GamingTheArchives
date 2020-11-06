using ArchiveSiteBackend.Api.Configuration;
using ArchiveSiteBackend.Api.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ArchiveSiteBackend.Api.Services
{
    /**
     * This class implements the Azure Cognitive Vision services that
     * we will use to read archive images.
     **/
    public class CognitiveService
    {
        private ComputerVisionClient ComputerVisionClient;
        private ILogger<CognitiveService> Logger;

        public CognitiveService(ILogger<CognitiveService> logger, AzureCognitiveConfiguration azureCognitiveConfiguration)
        {
            Logger = logger;

            ComputerVisionClient = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(azureCognitiveConfiguration.ApiKey))
            {
                Endpoint = azureCognitiveConfiguration.ApiUrl
            };
        }

        /// <summary>
        /// Sends an image to the Azure Cognitive Vision and returns the rendered text.
        /// More information available at https://westcentralus.dev.cognitive.microsoft.com/docs/services/computer-vision-v3-1-ga/operations/5d986960601faab4bf452005
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [Obsolete("It is highly likely that this API will change!")]
        public async Task<List<DocumentText>> ReadImage(string filename)
        {
            try
            {
                using (var fileStream = File.OpenRead(filename))
                {
                    var streamHeaders = await ComputerVisionClient.ReadInStreamAsync(fileStream, "en");
                    var operationLocation = streamHeaders.OperationLocation;

                    const int numberOfCharsInOperationId = 36;
                    string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

                    await Task.Delay(1500);

                    ReadOperationResult readOperationResult;
                    do
                    {
                        readOperationResult = await ComputerVisionClient.GetReadResultAsync(Guid.Parse(operationId));
                    } while (readOperationResult.Status == OperationStatusCodes.Running ||
                        readOperationResult.Status == OperationStatusCodes.NotStarted);

                    var listOfDocumentText = new List<DocumentText>();

                    var arrayOfReadResults = readOperationResult.AnalyzeResult.ReadResults;
                    foreach(var page in arrayOfReadResults)
                    {
                        foreach(var line in page.Lines)
                        {
                            var boundBox = new BoundingBox() {
                                x1 = line.BoundingBox[0],
                                y1 = line.BoundingBox[1],
                                x2 = line.BoundingBox[2],
                                y2 = line.BoundingBox[3]
                            };

                            var documentText = new DocumentText()
                            {
                                BoundingBox = boundBox,
                                Text = line.Text
                            };

                            listOfDocumentText.Add(documentText);
                        }
                    }

                    return listOfDocumentText;
                }
            }
            catch(FileNotFoundException e)
            {
                Logger.LogInformation($"file not found: {filename}");
                throw e;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"failed to analyze file: {filename}");
                return null;
            }
        }
    }
}
