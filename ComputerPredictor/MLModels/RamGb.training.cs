﻿// This file was auto-generated by ML.NET Model Builder.
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.LightGbm;
using Microsoft.ML.Transforms;

namespace ComputerPredictor
{
    public partial class RamGb
    {
        public const string RetrainFilePath =  @"C:\Users\simon\source\repos\DotNetComputerPredictor\ComputerPredictor\Files\computers.csv";
        public const char RetrainSeparatorChar = ',';
        public const bool RetrainHasHeader =  true;
        public const bool RetrainAllowQuoting =  false;

         /// <summary>
        /// Train a new model with the provided dataset.
        /// </summary>
        /// <param name="outputModelPath">File path for saving the model. Should be similar to "C:\YourPath\ModelName.mlnet"</param>
        /// <param name="inputDataFilePath">Path to the data file for training.</param>
        /// <param name="separatorChar">Separator character for delimited training file.</param>
        /// <param name="hasHeader">Boolean if training file has a header.</param>
        public static void Train(string outputModelPath, string inputDataFilePath = RetrainFilePath, char separatorChar = RetrainSeparatorChar, bool hasHeader = RetrainHasHeader, bool allowQuoting = RetrainAllowQuoting)
        {
            var mlContext = new MLContext();

            var data = LoadIDataViewFromFile(mlContext, inputDataFilePath, separatorChar, hasHeader, allowQuoting);
            var model = RetrainModel(mlContext, data);
            SaveModel(mlContext, model, data, outputModelPath);
        }

        /// <summary>
        /// Load an IDataView from a file path.
        /// </summary>
        /// <param name="mlContext">The common context for all ML.NET operations.</param>
        /// <param name="inputDataFilePath">Path to the data file for training.</param>
        /// <param name="separatorChar">Separator character for delimited training file.</param>
        /// <param name="hasHeader">Boolean if training file has a header.</param>
        /// <returns>IDataView with loaded training data.</returns>
        public static IDataView LoadIDataViewFromFile(MLContext mlContext, string inputDataFilePath, char separatorChar, bool hasHeader, bool allowQuoting)
        {
            return mlContext.Data.LoadFromTextFile<ModelInput>(inputDataFilePath, separatorChar, hasHeader, allowQuoting: allowQuoting);
        }


        /// <summary>
        /// Save a model at the specified path.
        /// </summary>
        /// <param name="mlContext">The common context for all ML.NET operations.</param>
        /// <param name="model">Model to save.</param>
        /// <param name="data">IDataView used to train the model.</param>
        /// <param name="modelSavePath">File path for saving the model. Should be similar to "C:\YourPath\ModelName.mlnet.</param>
        public static void SaveModel(MLContext mlContext, ITransformer model, IDataView data, string modelSavePath)
        {
            // Pull the data schema from the IDataView used for training the model
            DataViewSchema dataViewSchema = data.Schema;

            using (var fs = File.Create(modelSavePath))
            {
                mlContext.Model.Save(model, dataViewSchema, fs);
            }
        }


        /// <summary>
        /// Retrain model using the pipeline generated as part of the training process.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="trainData"></param>
        /// <returns></returns>
        public static ITransformer RetrainModel(MLContext mlContext, IDataView trainData)
        {
            var pipeline = BuildPipeline(mlContext);
            var model = pipeline.Fit(trainData);

            return model;
        }

        /// <summary>
        /// build the pipeline that is used from model builder. Use this function to retrain model.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations
            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding(new []{new InputOutputColumnPair(@"storage_type", @"storage_type"),new InputOutputColumnPair(@"operating_system", @"operating_system"),new InputOutputColumnPair(@"target_use_case", @"target_use_case")}, outputKind: OneHotEncodingEstimator.OutputKind.Indicator)      
                                    .Append(mlContext.Transforms.ReplaceMissingValues(new []{new InputOutputColumnPair(@"cpu_speed_ghz", @"cpu_speed_ghz"),new InputOutputColumnPair(@"cpu_cores", @"cpu_cores"),new InputOutputColumnPair(@"storage_gb", @"storage_gb"),new InputOutputColumnPair(@"price_usd", @"price_usd")}))      
                                    .Append(mlContext.Transforms.Concatenate(@"Features", new []{@"storage_type",@"operating_system",@"target_use_case",@"cpu_speed_ghz",@"cpu_cores",@"storage_gb",@"price_usd"}))      
                                    .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:@"ram_gb",inputColumnName:@"ram_gb",addKeyValueAnnotationsAsText:false))      
                                    .Append(mlContext.MulticlassClassification.Trainers.LightGbm(new LightGbmMulticlassTrainer.Options(){NumberOfLeaves=4,NumberOfIterations=649,MinimumExampleCountPerLeaf=20,LearningRate=0.6405011586356651,LabelColumnName=@"ram_gb",FeatureColumnName=@"Features",Booster=new GradientBooster.Options(){SubsampleFraction=0.9999997766729865,FeatureFraction=0.9677018758765564,L1Regularization=3.104464036542983E-10,L2Regularization=0.9999997766729865},MaximumBinCountPerFeature=239}))      
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName:@"PredictedLabel",inputColumnName:@"PredictedLabel"));

            return pipeline;
        }
    }
 }
