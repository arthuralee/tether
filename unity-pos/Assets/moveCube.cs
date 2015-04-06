using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System;
using System.Net;
using System.Net.Sockets;

public class moveCube : MonoBehaviour {
	Vector3 newPosition = new Vector3 (0, 0, 0);
	private Socket _clientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
	private byte[] _receiveBuffer = new byte[1024];
	
	// Use this for initialization
	private void Start () {
		try {
			_clientSocket.Connect (new IPEndPoint(IPAddress.Parse("127.0.0.1"),8080));
		} catch(SocketException ex) {
			Debug.Log (ex.Message);
		}

		_clientSocket.BeginReceive (_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, new AsyncCallback (ReceiveCallback), null);
	}

	private void ReceiveCallback(IAsyncResult AR)
	{
		//Check how much bytes are recieved and call EndRecieve to finalize handshake
		int recieved = _clientSocket.EndReceive (AR);
		
		if (recieved <= 0)
			return;
		
		//Copy the recieved data into new buffer , to avoid null bytes
		byte[] recData = new byte[recieved];
		Buffer.BlockCopy (_receiveBuffer, 0, recData, 0, recieved);
		
		// Update our newPosition
		// id,dist,x,y,z,yaw,pitch,roll
		string[] values = System.Text.Encoding.UTF8.GetString (recData).Split (',');

		// temporary swapping to make demo work
		newPosition.x = -5*float.Parse (values [3]); // mirror image for now
		newPosition.y = 5*float.Parse (values [4]);
		newPosition.z = 5*float.Parse (values [2]);

		//Start receiving again
		_clientSocket.BeginReceive (_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, new AsyncCallback (ReceiveCallback), null);
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = newPosition;
	}
}