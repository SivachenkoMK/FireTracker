using Azure.Storage.Blobs;
using FireTracker.Analysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FireTracker.Analysis;

public class ImageProcessor
{
    private readonly InferenceSession _inferenceSession;

    public ImageProcessor(IOptions<MachineLearningModelConfiguration> mlConfiguration)
    {
        var onnxModelPath = mlConfiguration.Value.ModelName;
        
        _inferenceSession = new InferenceSession(onnxModelPath);
    }

    public async Task<float> ProcessImageAsync(SessionInformationModel sessionInfo)
    {
        var localImagePath = await DownloadImageAsync(sessionInfo.PhotoUrl);
        
        var inputTensor = LoadImageAsTensor(localImagePath, 224, 224);
        
        using var outputs = _inferenceSession.Run([
            NamedOnnxValue.CreateFromTensor("digit", inputTensor)
        ]);
        
        var sigmoidValue = outputs.First().AsEnumerable<float>().First();

        return sigmoidValue;
    }

    private async Task<Stream> DownloadImageAsync(string blobUri)
    {
        var blobClient = new BlobClient(new Uri(blobUri));
        
        var streamingResponse = await blobClient.DownloadStreamingAsync();
        return streamingResponse.HasValue ? streamingResponse.Value.Content : Stream.Null;
    }

    private DenseTensor<float> LoadImageAsTensor(Stream imageStream, int width, int height)
    {
        // Could think about consuming multiple images from MQ and increasing batch size
        var tensor = new DenseTensor<float>(new[] { 1, height, width, 3 });

        using var image = Image.Load<Rgb24>(imageStream);

        image.Mutate(x => x.Resize(width, height));

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pixel = image[x, y];
                tensor[0, y, x, 0] = pixel.R / 255f;
                tensor[0, y, x, 1] = pixel.G / 255f;
                tensor[0, y, x, 2] = pixel.B / 255f;
            }
        }

        return tensor;
    }
}