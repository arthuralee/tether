using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Globalization;

public class Rotation : MonoBehaviour {
	public Vector3 axis = new Vector3(0, 0, 0);
	public int speed = 20;
	SerialPort stream = new SerialPort("/dev/tty.usbmodemfa131", 115200);
	float acc_roll;
	float acc_pitch;
	float gyr_x;
	float gyr_y;
	float gyr_z;
	float mag_heading;
	float kalAngleX;
	float kalAngleY;
	float compAngleX;
	float compAngleY;
	float timer;
	float angle =0;
	// Define an array of format providers.
	CultureInfo provider = new CultureInfo("en-US"); 
	// Define an array of styles.
	NumberStyles style = NumberStyles.Float | NumberStyles.Integer | NumberStyles.None | NumberStyles.Number | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
	
	class Kalman{
		public float angle;
		private float bias;
		private float[,] P = new float[2, 2]; // Error covariance matrix - This is a 2x2 matrix
		private float rate;
		/* We will set the variables like so, these can also be tuned by the user */
		private float Q_angle = 0.001f;
		private float Q_bias = 0.003f;
		private float R_measure = 0.03f;
		
		public float get_angle(float newAngle, float newRate, float dt) {
			// Discrete Kalman filter time update equations - Time Update ("Predict")
			// Update xhat - Project the state ahead
			/* Step 1 */
			rate = newRate - bias;
			angle += dt * rate;
			
			// Update estimation error covariance - Project the error covariance ahead
			/* Step 2 */
			P[0,0] += dt * (dt*P[1,1] - P[0,1] - P[1,0] + Q_angle);
			P[0,1] -= dt * P[1,1];
			P[1,0] -= dt * P[1,1];
			P[1,1] += Q_bias * dt;
			
			// Discrete Kalman filter measurement update equations - Measurement Update ("Correct")
			// Calculate Kalman gain - Compute the Kalman gain
			/* Step 4 */
			float S = P[0,0] + R_measure; // Estimate error
			/* Step 5 */
			float[] K = new float[2]; // Kalman gain - This is a 2x1 vector
			K[0] = P[0,0] / S;
			K[1] = P[1,0] / S;
			
			// Calculate angle and bias - Update estimate with measurement zk (newAngle)
			/* Step 3 */
			float y = newAngle - angle; // Angle difference
			/* Step 6 */
			angle += K[0] * y;
			bias += K[1] * y;
			
			// Calculate estimation error covariance - Update the error covariance
			/* Step 7 */
			float P00_temp = P[0,0];
			float P01_temp = P[0,1];
			
			P[0,0] -= K[0] * P00_temp;
			P[0,1] -= K[0] * P01_temp;
			P[1,0] -= K[1] * P00_temp;
			P[1,1] -= K[1] * P01_temp;
			
			return angle;
		}
	}

	Kalman kalmanX = new Kalman();
	Kalman kalmanY = new Kalman();

	// Use this for initialization
	void Start () {
		stream.Open();
		
		string value = stream.ReadLine();
		string[] angles = value.Split (',');
		try{
			acc_roll = float.Parse (angles [0], style, provider);
			acc_pitch = float.Parse (angles [1], style, provider);
			gyr_x = float.Parse (angles [2], style, provider);
			gyr_y = float.Parse (angles [3], style, provider);
			gyr_z = float.Parse (angles [4], style, provider);
			mag_heading = float.Parse (angles [5], style, provider);
		}
		catch(System.Exception e){
			return;
		}
		timer = Time.time;

		kalmanX.angle = acc_roll;
		kalmanY.angle = acc_pitch;
		kalAngleX = acc_roll;
		kalAngleY = acc_pitch;
		compAngleX = acc_roll;
		compAngleY = acc_pitch;

	}
	
	// Update is called once per frame
	void Update () {
		string value = stream.ReadLine();
		float dt = Time.time - timer;
		string[] angles = value.Split (',');
		try{
			acc_roll = float.Parse (angles [0], style, provider);
			acc_pitch = float.Parse (angles [1], style, provider);
			gyr_x = float.Parse (angles [2], style, provider);
			gyr_y = float.Parse (angles [3], style, provider);
			gyr_z = float.Parse (angles [4], style, provider);
			mag_heading = float.Parse (angles [5], style, provider);
		}
		catch(System.Exception e){
			return;
		}
	
		timer = Time.time;


		// This fixes the transition problem when the accelerometer angle jumps between -180 and 180 degrees
		if ((acc_pitch < -120 && kalAngleY > 120) || (acc_pitch > 120 && kalAngleY < -120)) {
			kalmanY.angle = acc_pitch;
			kalAngleY = acc_pitch;
			compAngleY = acc_pitch;
		} else
			kalAngleY = kalmanY.get_angle(acc_pitch, gyr_y, dt); // Calculate the angle using a Kalman filter

		if ((acc_roll < -60 && kalAngleX > 60) || (acc_roll > 60 && kalAngleX < -60)) {
			kalmanX.angle = acc_roll;
			kalAngleX = acc_roll;
		} else
			kalAngleX = kalmanX.get_angle(acc_roll, gyr_x, dt); // Calculate the angle using a Kalman filter


		Quaternion target = Quaternion.Euler(-kalAngleY, -mag_heading, kalAngleX);
		transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 2.0f);
	}
}