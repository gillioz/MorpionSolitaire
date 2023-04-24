using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using MorpionSolitaire;

namespace MorpionSolitaireWeb;

public class InferenceModel
{
    private readonly InferenceSession _model;

    public InferenceModel()
    {
        _model = new InferenceSession("../models/Descartes.onnx");
    }

    public float Infer(Image image)
    {
        if (_model is null) throw new Exception("InferenceModel has not been initialized");

        const int size = 94;
        var imageTensor = image.ToTensor(size);
        var inputTensor = new DenseTensor<float>(new [] { 1, 1, size, size });
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                inputTensor[0, 0, x, y] = imageTensor[x, y];
            }
        }

        var input = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("images", inputTensor) };

        var result = _model.Run(input).ToList().First().AsTensor<float>();

        return result[0, 0];
    }
}