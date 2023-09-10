////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////


using System.Text.Json;
using System.Text.Json.Serialization;
using Anndotnet.Core.Entities;
using Anndotnet.Core.Interfaces;
using Anndotnet.Core.Layers;


namespace Anndotnet.Core.Util;

public class LayerConverter : JsonConverter<ILayer>
{

    public override bool CanConvert(Type typeToConvert) =>
        typeof(ILayer).IsAssignableFrom(typeToConvert);

    public override ILayer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        string propertyName = reader.GetString();
        if (propertyName != "Type")
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        if (!Enum.TryParse(reader.GetString(), out LayerType typeDiscriminator))
        {
            throw new JsonException();
        }

        ILayer layer = typeDiscriminator switch
        {
            LayerType.Dense => new Dense(),
            LayerType.Dropout => new Dropout(),
            _ => throw new JsonException()
        };

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return layer;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "Activation":
                        var act = reader.GetString();

                        if (Enum.TryParse(act, true, out Activation eAct))
                        {
                            ((Dense)layer).Activation = eAct;
                        }
                        else
                        {
                            throw new NotSupportedException(nameof(act));
                        }
                        break;
                    case "OutputDim":
                        int outDim = reader.GetInt32();
                        ((Dense)layer).OutputDim = outDim;
                        break;

                    case "Rate":
                        decimal drop = reader.GetDecimal();
                        ((Dropout)layer).Rate = Convert.ToSingle(drop);
                        break;
                    
                    case "HasBias":
                        bool hasBias = reader.GetBoolean();
                        ((Dense)layer).HasBias = hasBias;
                        break;
                    
                    case "Name":
                        var name = reader.GetString();
                        layer.Name = name;
                        break;
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, ILayer layerBase, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        var lType = Enum.GetName(typeof(LayerType), layerBase.Type);
        writer.WriteString("Type", lType);

        if (layerBase is Dense fcLayer)
        {
            writer.WriteBoolean("HasBias", (bool)fcLayer.HasBias);
            writer.WriteNumber("OutputDim", (int)fcLayer.OutputDim);
            writer.WriteString("Activation", fcLayer.Activation.ToString());
        }
        else if (layerBase is Dropout dropLayer)
        {
            writer.WriteNumber("Rate", (decimal)dropLayer.Rate);
        }

        writer.WriteString("Name", layerBase.Name);
           

        writer.WriteEndObject();
    }


}