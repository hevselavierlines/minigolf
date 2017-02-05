using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using UnityEngine.UI;
//using System.IO.Ports;


public class GolfClubControl : MonoBehaviour {
	private UdpClient receivingUdpClient;
	private TcpListener tcpController;
	private IPEndPoint remoteIpEndPoint;
	private Thread threadUdp, threadConnector;
	private float[] receivedTransform;
	private float[] rotation;
	public Text textinfo;
	public Ball ball;
	public float power;
	private float[] resetPoint = new float[]{10.0f, 180.0f, 0.0f };
	private ArrayList clients;

	// port number
	private int receivePortUdp = 4545;
	private int receivePortTcp = 4540;
	private bool activeClub = false;
	public bool DEBUG;

	// Use this for initialization
	void Start () {
		receivedTransform = new float[4];
		try {
			receivingUdpClient = new UdpClient(receivePortUdp);
		} catch (Exception e) {
			Debug.Log (e.ToString());
		}

		remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

		// start the thread for receiving signals
		threadUdp = new Thread(new ThreadStart(ReceiveDataBytes));
		threadUdp.Start();

		rotation = new float[] { 0,0,0 };

		tcpController = new TcpListener (new IPEndPoint (IPAddress.Any, receivePortTcp));
		tcpController.Start ();

		threadConnector = new Thread (new ThreadStart (tcpConnector));
		threadConnector.Start ();

		clients = new ArrayList ();
		resetClub ();
	}

	public void setBall(Ball _ball) {
		ball = _ball;
	}
	
	// Update is called once per frame
	void Update () {
		//transform.rotation.SetEulerAngles (new Vector3 (45, -receivedTransform [2], receivedTransform [1]));
		//transform.Rotate (0, 0, 0);
		//transform.Rotate (-receivedTransform [0], -receivedTransform [2], receivedTransform [1]);
		//transform.Translate(new Vector3(0, 0, 1));
		//transform.eulerAngles = new Vector3(resetPoint[0] - receivedTransform[2], resetPoint[1] - receivedTransform[1], 0.0f);
		//transform.Translate (new Vector3 (0, 0, -1));
		if (textinfo != null) {
			StringBuilder sb = new StringBuilder ();
			sb.Append ("(");
			sb.Append (receivedTransform [2]);
			sb.Append (", ");
			sb.Append (-receivedTransform [1]);
			sb.Append (", ");
			sb.Append (receivedTransform [0]);
			sb.Append (")");
			textinfo.text = sb.ToString ();
		}

		transform.eulerAngles = new Vector3 (0,0,0);

		if (Input.GetKey (KeyCode.UpArrow)) {
			rotation [0] += 1;	
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			rotation [0] -= 1;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			rotation [1] += 1;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			rotation [1] -= 1;
		}
		if (!DEBUG) {
			rotation [0] = resetPoint [0] - receivedTransform [2];
			rotation [1] = resetPoint [1] + receivedTransform [0];
			rotation [2] = resetPoint [2] + receivedTransform [1];
		}
		float diffX = 0.0f;
		float diffZ = 1.0f;
		Vector3 rotatePoint = new Vector3 (transform.position.x + diffX, transform.position.y, transform.position.z + diffZ);
		transform.RotateAround (rotatePoint, new Vector3 (1, 0, 0), rotation [0]);
		transform.RotateAround (rotatePoint, new Vector3 (0, 1, 0), rotation [1]);
		power = receivedTransform [3];
		//transform.RotateAround (rotatePoint, new Vector3 (0, 0, 1), rotation [1]);
	}

	void vibrate() {
		sendCharacter ('h');
		activeClub = true;
	}

	private void sendCharacter(char sendingChar) {
		for(int i = clients.Count - 1; i >= 0; i--) {
			TcpClient client = (TcpClient)clients [i];
			try {
				if (client.Connected) {
					client.GetStream ().WriteByte ((byte)sendingChar);
					client.GetStream ().Flush ();
				} else {
					clients.RemoveAt (i);
				}
			} catch (Exception) {
				clients.RemoveAt (i);
			}
		}
	}

	void hit() {
		sendCharacter ('s');
		activeClub = false;
	}

	public void finishVibrate() {
		sendCharacter ('f');
	}

	void tcpConnector() {
		while(true) {
			TcpClient connectedClient = tcpController.AcceptTcpClient ();

			clients.Add (connectedClient);
			vibrate ();
		}
	}

	void ReceiveDataBytes() {
		while (true) {
			//Debug.Log ("Threading inside while");
			// NOTE!: This blocks execution until a new message is received
			byte[] buffer = receivingUdpClient.Receive(ref remoteIpEndPoint);
			int currPos = 0;
			int size = 0;
			size = (int)buffer[currPos++] << 8;
			size = (int)buffer[currPos++] | size;
			if (size > 0) {
				float[] parameters = new float[(size - 2) / 4];


				for (int i = 0; i < size / 4; i++) {
					int currElem = 0;
					currElem = (int)buffer [currPos++] << 24;
					currElem = (int)buffer [currPos++] << 16 | currElem;
					currElem = (int)buffer [currPos++] << 8 | currElem;
					currElem = (int)buffer [currPos++] | currElem;

					float floatElem = (float)currElem / 1000000;

					parameters [i] = floatElem;
				}
			
				if (activeClub == true) {
					for (int i = 0; i < 4; i++) {
						receivedTransform [i] = parameters [i];
					}
				} else {
					receivedTransform [0] = 0;
					receivedTransform [1] = 0;
					receivedTransform [2] = 0;
				}
			} else {
				byte[] infos = new byte[] { (byte)'O', (byte)'K' };
				receivingUdpClient.Send (infos, 2);
			}
			Thread.Sleep (1);
		}
	}

	Matrix4x4 getRotation(float[] rotationVector) {
		Matrix4x4 ret = new Matrix4x4 ();
		float q0;
		float q1 = -rotationVector[0];
		float q2 = -rotationVector[2];
		float q3 = rotationVector[1];

		if (rotationVector.Length >= 4) {
			q0 = rotationVector[3];
		} else {
			q0 = 1 - q1*q1 - q2*q2 - q3*q3;
			q0 = (q0 > 0) ? (float)Mathf.Sqrt(q0) : 0;
		}

		float sq_q1 = 2 * q1 * q1;
		float sq_q2 = 2 * q2 * q2;
		float sq_q3 = 2 * q3 * q3;
		float q1_q2 = 2 * q1 * q2;
		float q3_q0 = 2 * q3 * q0;
		float q1_q3 = 2 * q1 * q3;
		float q2_q0 = 2 * q2 * q0;
		float q2_q3 = 2 * q2 * q3;
		float q1_q0 = 2 * q1 * q0;

		ret.m00 = 1 - sq_q2 - sq_q3;
		ret.m01 = q1_q2 - q3_q0;
		ret.m02 = q1_q3 + q2_q0;
		ret.m03 = 0.0f;

		ret.m10 = q1_q2 + q3_q0;
		ret.m11 = 1 - sq_q1 - sq_q3;
		ret.m12 = q2_q3 - q1_q0;
		ret.m13 = 0.0f;

		ret.m20 = q1_q3 - q2_q0;
		ret.m21 = q2_q3 + q1_q0;
		ret.m22 = 1 - sq_q1 - sq_q2;
		ret.m23 = 0.0f;

		ret.m30 = 0.0f;
		ret.m31 = 0.0f;
		ret.m32 = 0.0f;
		ret.m33 = 1.0f;

		return ret;
	}

	void CloseClient() {
		threadUdp.Abort();
		threadConnector.Abort ();
		receivingUdpClient.Close();
	}

	public void clubDisappear() {
		hit ();
		resetClub ();
		gameObject.SetActive (false);
	}

	public void clubAppear() {
		vibrate ();
		resetClub ();
		gameObject.SetActive (true);
	}

	public void resetClub() {
		transform.eulerAngles = new Vector3 (0,0,0);
		rotation [0] = resetPoint [0];
		rotation [1] = resetPoint [1];
		rotation [2] = resetPoint [2];

		receivedTransform [0] = 0;
		receivedTransform [1] = 0;
		receivedTransform [2] = 0;
	}

	void OnApplicationQuit() {
		CloseClient();
	}
}
