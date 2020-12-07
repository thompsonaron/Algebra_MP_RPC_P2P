using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class NetData
{
	public NetType dataType;
	public byte[] data;
}

[Serializable]
public class NetVector3
{
	public float x;
	public float y;
	public float z;
}

public enum NetType
{
	StartTheGame,
	Vector3,
	RandomNumber,
	NumberGuess,
	CorrectGuess,
	GoBigger,
	GoLower,
	MoveComplete

		,ReadyToPlay,
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