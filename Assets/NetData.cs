using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class NetData
{
	public NetType dataType;
	public byte[] data;
}

public enum NetType
{
	StartTheGame,
	MoveComplete,
	ReadyToPlay,
    ClientMove,
    HostMove
}
























public static class Serializator
{ 
	public static T deserialize<T>(byte[] bytes)
	{
		BinaryFormatter bf = new BinaryFormatter();
		MemoryStream ms = new MemoryStream(bytes);

		T output = (T)bf.Deserialize(ms);

		ms.Close();
		return output;
	}

	public static byte[] serialize(object data)
	{
		BinaryFormatter bf = new BinaryFormatter();
		MemoryStream ms = new MemoryStream();
		bf.Serialize(ms, data);

		ms.Close();
		return ms.ToArray();
	}
}