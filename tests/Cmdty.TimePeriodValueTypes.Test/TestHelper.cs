#region License
// Copyright (c) 2019 Jake Fowler
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without 
// restriction, including without limitation the rights to use, 
// copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following 
// conditions:
//
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using NUnit.Framework;

namespace Cmdty.TimePeriodValueTypes.Test
{
    internal static class TestHelper
    {

        internal static void AssertTimePeriodXmlSerializationRoundTripSuccess<T>(T timePeriod)
            where T : ITimePeriod<T>
        {
            XmlSerializer xmlSerializer = new XmlSerializer(timePeriod.GetType());

            using (Stream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, timePeriod);
                memoryStream.Position = 0;
                var timePeriodRecreated = (T)xmlSerializer.Deserialize(memoryStream);
                Assert.AreEqual(timePeriod, timePeriodRecreated);
            }
        }

        internal static void AssertTimePeriodBinarySerializationRoundTripSuccess<T>(T timePeriod)
            where T : ITimePeriod<T>
        {
            var binaryFormatter = new BinaryFormatter();
            using (Stream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, timePeriod);
                memoryStream.Position = 0;
                T timePeriodDeserialized = (T) binaryFormatter.Deserialize(memoryStream);
                Assert.AreEqual(timePeriod, timePeriodDeserialized);
            }
        }

    }
}
