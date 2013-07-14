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
using System.IO;
using System.Text;

namespace AlphaMapper.PropertyLoader
{
//    static class ModelExtension
//    {
//        public static string ToBulkInsertString(this V3Object v3Object)
//        {
//            return string.Format("\"{0}\",\"{1}\",{2},{3},{4},{5},{6},{7},{8},{9},{10},\"{11}\",\"{12}\"",
//                MySqlHelper.EscapeString(v3Object.BuildDate.Value.ToString("yyyy-MM-dd hh:mm")),
//                MySqlHelper.EscapeString(v3Object.ModelName),
//                v3Object.Owner,
//                v3Object.PositionY,
//                v3Object.CellX,
//                v3Object.OffsetX,
//                v3Object.CellZ,
//                v3Object.OffsetZ,
//                v3Object.Yaw,
//                v3Object.Tilt,
//                v3Object.Roll,
//                MySqlHelper.EscapeString(v3Object.Description),
//                MySqlHelper.EscapeString(v3Object.Action));
//        }
//    }

//    class Program
//    {
//        static void Main()
//        {
//            Console.Write("Enter file to import: ");
//            var fileName = Console.ReadLine();
            
//            using(var propertyLoader = new PropertyLoader(fileName))
//            using (var connection = new MySqlConnection("Server=192.168.1.55;Database=alphamapper;Uid=byte;Pwd="))
//            {
//                connection.Open();
//                CreateTable(connection);
//                BulkLoad(connection, propertyLoader);
//                SetupIndexes(connection);
//            }
//        }

//        private static void SetupIndexes(MySqlConnection connection)
//        {
//            //TODO: Create indexes.
//        }

//        private static void BulkLoad(MySqlConnection connection, PropertyLoader propertyLoader)
//        {
//            var bulkLoader = new MySqlBulkLoader(connection)
//            {
//                TableName = "models",
//                FieldTerminator = ",",
//                LineTerminator = "\r\n",
//                EscapeCharacter = '\\',
//                FieldQuotationCharacter = '"',
//                FileName = "property.csv"
//            };

//            while(propertyLoader.HasMore)
//            {
//                Console.WriteLine("Loading batch.");
//                using(var temporaryFile = File.OpenWrite("property.csv"))
//                using (var streamWriter = new StreamWriter(temporaryFile))
//                {
//                    var input = propertyLoader.Property.Take(50000).Select(model => model.ToBulkInsertString());
//                    foreach (var line in input)
//                    {
//                        streamWriter.WriteLine(line);
//                    }
//                }

//                int count = bulkLoader.Load();
//                Console.WriteLine("Batch loaded {0} total entries", count);
//                File.Delete("property.csv");
//            }
//        }

//        private static void CreateTable(MySqlConnection connection)
//        {
//            const string dropText = "DROP TABLE IF EXISTS V3Objects";
//            var dropCommand = new MySqlCommand(dropText, connection);

//            const string createText = @"CREATE TABLE IF NOT EXISTS V3Objects (
//                                            BuildDate DATETIME,
//                                            ModelName varchar(64),
//                                            Owner int,
//                                            PositionY int,
//                                            CellX int,
//                                            OffsetX int,
//                                            CellZ int,
//                                            OffsetZ int,
//                                            Yaw int,
//                                            Tilt int,
//                                            Roll int,
//                                            Description varchar(256),
//                                            Action varchar(256)
//                                        )";

//            var createCommand = new MySqlCommand(createText, connection);

//            dropCommand.ExecuteNonQuery();
//            createCommand.ExecuteNonQuery();
//        }
//    }

    class Program
    {
        static void Main()
        {
            
        }
    }
}
