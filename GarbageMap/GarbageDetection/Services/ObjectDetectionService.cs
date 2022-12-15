﻿using Microsoft.Extensions.ML;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using GarbageMap.GarbageDetection.Helpers;
using GarbageMap.GarbageDetection.ML.DataModels;

namespace GarbageMap.GarbageDetection.Services
{
    public interface IObjectDetectionService
    {
        void DetectObjectsUsingModel(ImageInputData imageInputData, float minProbabilityToShow, float minThresholdToShow);
        Image DrawBoundingBox(Image image);
    }

    public class ObjectDetectionService : IObjectDetectionService
    {
        List<BoundingBox> _filteredBoxes;
        private readonly OnnxOutputParser _outputParser = new OnnxOutputParser(new TinyYoloModel(null));
        private readonly PredictionEnginePool<ImageInputData, TinyYoloPrediction> _predictionEngine;

        public ObjectDetectionService(PredictionEnginePool<ImageInputData, TinyYoloPrediction> predictionEngine)
        {
            this._predictionEngine = predictionEngine;
        }

        public void DetectObjectsUsingModel(ImageInputData imageInputData, float minProbabilityToShow, float minThresholdToShow)
        {
            var probs = _predictionEngine.Predict(imageInputData).PredictedLabels;
            var boundingBoxes = _outputParser.ParseOutputs(probs, (minProbabilityToShow / 100));
            _filteredBoxes = _outputParser.FilterBoundingBoxes(boundingBoxes, 50, (minThresholdToShow / 100));
        }

        public Image DrawBoundingBox(Image image)
        {
            //Image image = Image.FromFile(imageFilePath);
            var originalHeight = image.Height;
            var originalWidth = image.Width;
            if (_filteredBoxes != null)
            {
                foreach (var box in _filteredBoxes)
                {
                    // Process output boxes
                    var x = (uint) Math.Max(box.Dimensions.X, 0);
                    var y = (uint) Math.Max(box.Dimensions.Y, 0);
                    var width = (uint) Math.Min(originalWidth - x, box.Dimensions.Width);
                    var height = (uint) Math.Min(originalHeight - y, box.Dimensions.Height);

                    // Fit to current image size
                    x = (uint) originalWidth * x / ImageSettings.ImageWidth;
                    y = (uint) originalHeight * y / ImageSettings.ImageHeight;
                    width = (uint) originalWidth * width / ImageSettings.ImageWidth;
                    height = (uint) originalHeight * height / ImageSettings.ImageHeight;

                    using (var thumbnailGraphic = Graphics.FromImage(image))
                    {
                        thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
                        thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
                        thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        // Define Text Options
                        var drawFont = new Font("Arial", 10, FontStyle.Bold);
                        var size = thumbnailGraphic.MeasureString(box.Description, drawFont);
                        var fontBrush = new SolidBrush(Color.Black);
                        var atPoint = new Point((int) x, (int) y - (int) size.Height - 1);

                        // Define BoundingBox options
                        var pen = new Pen(box.BoxColor, 2.2f);
                        var colorBrush = new SolidBrush(box.BoxColor);

                        // Draw text on image 
                        thumbnailGraphic.FillRectangle(colorBrush, (int) x, (int) (y - size.Height - 1),
                            (int) size.Width, (int) size.Height);
                        thumbnailGraphic.DrawString(box.Description, drawFont, fontBrush, atPoint);

                        // Draw bounding box on image
                        thumbnailGraphic.DrawRectangle(pen, x, y, width, height);
                    }
                }
            }

            return image;
        }
    }
}
