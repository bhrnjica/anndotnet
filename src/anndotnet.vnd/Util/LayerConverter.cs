///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//                Created by anndotnet community, anndotnet.com              //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/anndotnet/anndotnet //
//                                                                           //
//             For feedback:https://github.com/anndotnet/anndotnet/issues    //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using AnnDotNet.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;
using AnnDotNet.Vnd.Layers;

namespace AnnDotNet.Vnd.Util;

public class JsonLayerConverter : JsonConverter<ILayer>
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
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException();
        }

        LayerType typeDiscriminator = (LayerType)reader.GetInt32();
        ILayer layer = typeDiscriminator switch
        {
            LayerType.Activation => new ActLayer(),
            LayerType.Dense => new FCLayer(),
            LayerType.Drop => new DropLayer(),
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
                        int act = reader.GetInt32();
                        ((ActLayer)layer).Activation = (Activation)act;
                        break;
                    case "OutDim":
                        int outDim = reader.GetInt32();
                        ((FCLayer)layer).OutDim = outDim;
                        break;
                    case "DropPercentage":
                        int drop = reader.GetInt32();
                        ((DropLayer)layer).DropPercentage = drop;
                        break;
                    case "Name":
                        var name = reader.GetString();
                        layer.Name = name;
                        break;
                    case "Type":
                        int type = reader.GetInt32();
                        layer.Type = (LayerType)type;
                        break;
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, ILayer layerBase, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("Type", (int)layerBase.Type);

        if (layerBase is ActLayer actLayer)
        {
            writer.WriteNumber("Activation", (int)actLayer.Activation);
        }
        else if (layerBase is FCLayer fcLayer)
        {
            writer.WriteNumber("OutDim", (int)fcLayer.OutDim);
        }
        else if (layerBase is DropLayer dropLayer)
        {
            writer.WriteNumber("DropPercentage", (int)dropLayer.DropPercentage);
        }

        writer.WriteString("Name", layerBase.Name);
           

        writer.WriteEndObject();
    }


}