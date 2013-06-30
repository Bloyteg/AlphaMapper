// Copyright 2013 Joshua R. Rodgers
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AlphaMapper.Property.Model;

namespace AlphaMapper.PropertyLoader
{
    class PropertyLoader : IDisposable
    {
        private readonly StreamReader _inStream;

        public PropertyLoader(string fileName)
        {
            _inStream = new StreamReader(File.OpenRead(fileName));

            var version = _inStream.ReadLine();
        }

        private static int CellFromCm(int x)
        {
            if (x < 0)
            {
                x -= 999;
            }

            return x / 1000;
        }

        private static int OffsetFromCm(int x)
        {
            return (1000 + (x % 1000)) % 1000;
        }

        private static string ReadLine(StreamReader streamReader)
        {
            var stringBuilder = new StringBuilder();

            while (!streamReader.EndOfStream)
            {
                var nextChar = streamReader.Read();

                if (nextChar == '\n')
                {
                    return stringBuilder.ToString();
                }

                if (nextChar == '\r' && streamReader.Peek() == '\n')
                {
                    streamReader.Read();
                    return stringBuilder.ToString();
                }

                stringBuilder.Append((char) nextChar);
            }

            return stringBuilder.ToString();
        }

        public bool HasMore
        {
            get { return !_inStream.EndOfStream; }
        }

        public IEnumerable<V3Object> Property
        {
            get
            {
                while(!_inStream.EndOfStream)
                {
                    var input = ReadLine(_inStream);
                    if (input == null)
                    {
                        continue;
                    }

                    var components = input.Split(new[] { ' ' }, 14);

                    if(components.Length < 14 || int.Parse(components[8]) != 0)
                    {
                        continue;
                    }

                    var owner = int.Parse(components[0]);
                    var buildDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(int.Parse(components[1]));
                    var xOrigin = int.Parse(components[2]);
                    var yOrigin = int.Parse(components[3]);
                    var zOrigin = int.Parse(components[4]);
                    var yaw = int.Parse(components[5]);
                    var tilt = int.Parse(components[6]);
                    var roll = int.Parse(components[7]);
                    var modelLength = int.Parse(components[9]);
                    var descriptionLength = int.Parse(components[10]);
                    var actionLength = int.Parse(components[11]);
                    var payload = Encoding.UTF8.GetBytes(components[13].Replace((char)0xFFFD, '\r').Replace((char)0x7F, '\n'));

                    string model = string.Empty, description = string.Empty, action = string.Empty;

                    if(modelLength > 0)
                    {
                        model = Encoding.UTF8.GetString(payload, 0, modelLength);
                    }

                    if (descriptionLength > 0)
                    {
                        description = Encoding.UTF8.GetString(payload, modelLength, descriptionLength);
                    }

                    if (actionLength > 0)
                    {
                        action = Encoding.UTF8.GetString(payload, modelLength + descriptionLength, actionLength);
                    }

                    yield return new V3Object
                                     {
                                         Owner = owner,
                                         BuildDate = buildDate,
                                         CellX = CellFromCm(xOrigin),
                                         CellZ = CellFromCm(zOrigin),
                                         OffsetX = OffsetFromCm(xOrigin),
                                         OffsetZ = OffsetFromCm(zOrigin),
                                         PositionY = yOrigin,
                                         Yaw = yaw,
                                         Tilt = tilt,
                                         Roll = roll,
                                         ModelName = model,
                                         Description = description,
                                         Action = action
                                     };
                }
            }
        }

        public void Dispose()
        {
            if(_inStream != null)
            {
                _inStream.Dispose();
            }
        }
    }
}
