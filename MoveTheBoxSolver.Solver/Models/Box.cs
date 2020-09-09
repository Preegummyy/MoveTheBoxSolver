using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace MoveTheBoxSolver.Solver.Models
{
    public class Box
    {
        public BoxType Type { get; set; }

        public Box Clone()
        {
            ////Serialize////
            byte[] arrByte = SerializeArrayByte(this);
            ///Deserialize///
            return (Box)DeserializeArrayByte(arrByte);
        }

        public static byte[] SerializeArrayByte(object obj)
        {
            MemoryStream memoryStream = (MemoryStream)null;
            try
            {
                memoryStream = new MemoryStream();
                new BinaryFormatter().Serialize((Stream)memoryStream, obj);
                return memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Close();
            }
        }

        public static object DeserializeArrayByte(byte[] serializedObject)
        {
            MemoryStream memoryStream = (MemoryStream)null;
            try
            {
                memoryStream = new MemoryStream(serializedObject);
                memoryStream.Position = 0L;
                return new BinaryFormatter().Deserialize((Stream)memoryStream);
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Close();
            }
        }        
    }

    public enum BoxType
    {
        Empty = 0,
        BrownWood = 1,
        RedWood = 2,
        GreenWood = 3,
        BrownLeather = 4,
        BrownPaper = 5,
        BlueSteel = 6,
    }
}
