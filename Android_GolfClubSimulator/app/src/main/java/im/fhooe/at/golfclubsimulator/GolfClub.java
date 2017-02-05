package im.fhooe.at.golfclubsimulator;/*
 * Copyright (C) 2007 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.Socket;
import java.net.SocketException;
import java.net.UnknownHostException;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.FloatBuffer;
import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.opengles.GL10;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.opengl.GLSurfaceView;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Vibrator;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.SurfaceView;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

/**
 * Wrapper activity demonstrating the use of the new
 * {@link SensorEvent#values rotation vector sensor}
 * ({@link Sensor#TYPE_ROTATION_VECTOR TYPE_ROTATION_VECTOR}).
 *
 * @see Sensor
 * @see SensorEvent
 * @see SensorManager
 *
 */
public class GolfClub extends Activity implements SensorEventListener {
    private SensorManager mSensorManager;
    private TextView textView;
    private DatagramSocket datagramSocket;
    private int[] calibration;
    private int[] values = new int[3];
    private String IPADDRESS = "10.29.17.237";
    private int m_accelaration;
    private float acceleration;
    private Sensor mRotationVectorSensor;
    private Sensor mAccelerationSensor;
    private float[] mRotationMatrix = new float[16];
    private float[] oldValues;
    private float speed;
    private boolean sendSensorData = false;
    private Vibrator vibrator;
    private Thread controllerThread;
    private Socket socket;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_golf_club);

        if(getIntent().hasExtra("IPADDRESS")) {
            IPADDRESS = getIntent().getStringExtra("IPADDRESS");
        }
        vibrator = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
        mSensorManager = (SensorManager)getSystemService(SENSOR_SERVICE);
        mAccelerationSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
        mRotationVectorSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_ROTATION_VECTOR);

        textView = (TextView) findViewById(R.id.textView);
        Button calibrateButton = (Button) findViewById(R.id.btCalibrate);
        calibrateButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                calibrate();
            }
        });

        Button cancelButton = (Button) findViewById(R.id.btCancel);
        cancelButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                finish();
            }
        });

        try {
            datagramSocket = new DatagramSocket();
        } catch (SocketException e) {
            e.printStackTrace();
        }

        textView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                calibrate();
            }
        });

        mRotationMatrix[ 0] = 1;
        mRotationMatrix[ 4] = 1;
        mRotationMatrix[ 8] = 1;
        mRotationMatrix[12] = 1;

        controllerThread = new Thread(new Runnable() {
            @Override
            public void run() {
                controlListening();
            }
        });
        controllerThread.start();
    }

    private void controlListening() {
        InetAddress address;
        try {
            address = InetAddress.getByName(IPADDRESS);
            socket = new Socket(address, 4540);
            socket.setSoTimeout(60000);
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    textView.setText("connected");
                }
            });
            while(socket.isConnected()) {
                final int info = socket.getInputStream().read();
                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        if(info == 'h') {
                            vibrator.vibrate(500);
                            sendSensorData = true;
                            textView.setText("please hit the ball");
                        } else if(info == 's') {
                            vibrator.vibrate(150);
                            sendSensorData = false;
                            textView.setText("good shot! ball rolling");
                        } else if(info == 'f') {
                            textView.setText("You did it!");
                            try {
                                vibrator.vibrate(100);
                                Thread.sleep(50);
                                vibrator.vibrate(100);
                                Thread.sleep(50);
                                vibrator.vibrate(500);
                            } catch (InterruptedException e) {
                                e.printStackTrace();
                            }
                        }
                    }
                });
            }
        } catch (final Exception e) {
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    AlertDialog.Builder adb = new AlertDialog.Builder(GolfClub.this);
                    adb.setTitle("An error occurred");
                    adb.setMessage(e.getLocalizedMessage());
                    adb.setNegativeButton("close", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            finish();
                        }
                    });
                    try {
                        adb.show();
                    }catch (RuntimeException e) {
                        e.printStackTrace();
                    }
                }
            });
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        mSensorManager.registerListener(this, mRotationVectorSensor, SensorManager.SENSOR_DELAY_GAME);
        mSensorManager.registerListener(this, mAccelerationSensor, SensorManager.SENSOR_DELAY_GAME);
    }
    @Override
    protected void onPause() {
        super.onPause();
        mSensorManager.unregisterListener(this);
    }

    private void convertToDegrees(float[] vector){
        for (int i = 0; i < vector.length; i++){
            vector[i] = Math.round(Math.toDegrees(vector[i]));
        }
    }

    public void calibrate() {
        calibration = new int[values.length];
        for(int i = 0; i < values.length; i++) {
            calibration[i] = values[i];
        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if(controllerThread != null) {
            controllerThread.interrupt();
        }
        try {
            if(socket != null) {
                socket.close();
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void onSensorChanged(SensorEvent event) {
        // we received a sensor event. it is a good practice to check
        // that we received the proper event
        if(event.sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
            float values[] = event.values;
            if(oldValues == null) {
                oldValues = new float[] {values[0], values[1], values[2]};
            } else {
                float acc = (float)Math.sqrt(
                        Math.pow(values[0] - oldValues[0], 2)
                                + Math.pow(values[1] - oldValues[1], 2)
                                + Math.pow(values[2] - oldValues[2], 2));
                m_accelaration = (int)(acc * 1_000_000);

                oldValues[0] = values[0];
                oldValues[1] = values[1];
                oldValues[2] = values[2];
            }
        }
        if (event.sensor.getType() == Sensor.TYPE_ROTATION_VECTOR) {
            // convert the rotation-vector to a 4x4 matrix. the matrix
            // is interpreted by Open GL as the inverse of the
            // rotation-vector, which is what we want

            int[] calibratedValue = new int[3];

            //calculate Euler angles now
            float[] euler = new float[3];
            SensorManager.getRotationMatrixFromVector(mRotationMatrix, event.values);
            SensorManager.getOrientation(mRotationMatrix, euler);
            convertToDegrees(euler);

            for(int i = 0; i < 3; i++) {
                values[i] = (int) (euler[i] * 1_000_000);
                if(calibration != null) {
                    calibratedValue[i] = values[i] - calibration[i];
                } else {
                    calibratedValue = values;
                }
            }

            //textView.setText(calibratedValue[0] / 1_000_000 + "\n" + calibratedValue[1] / 1_000_000 + "\n" + calibratedValue[2] / 1_000_000);
            if(sendSensorData) {
                new SendData().execute(calibratedValue[0], calibratedValue[1], calibratedValue[2], m_accelaration);
            }
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }


    private class SendData extends AsyncTask<Integer, Object, Void> {
        protected Void doInBackground(Integer... params) {
            InetAddress address;
            try {
                address = InetAddress.getByName(IPADDRESS);
                int size = params.length * 4 + 2;
                byte[] buffer = new byte[size];
                int arVal = 0;
                buffer[arVal++] = (byte) (size >> 8);
                buffer[arVal++] = (byte) (size);
                for(int i = 0; i < params.length; i++) {
                    int param = params[i];
                    buffer[arVal++] = (byte) (param >> 24);
                    buffer[arVal++] = (byte) (param >> 16);
                    buffer[arVal++] = (byte) (param >> 8);
                    buffer[arVal++] = (byte) (param);
                }
                DatagramPacket packet = new DatagramPacket(
                        buffer, buffer.length, address, 4545
                );
                if(datagramSocket == null) {
                    datagramSocket = new DatagramSocket();
                }
                datagramSocket.send(packet);
            } catch (UnknownHostException e) {
                e.printStackTrace();
            } catch (IOException e) {
                e.printStackTrace();
            }
            return null;
        }
    }

    @Override
    public boolean onKeyUp(int keyCode, KeyEvent event) {
        if(keyCode == 23) {
            calibrate();
            return true;
        } else {
            return super.onKeyUp(keyCode, event);
        }
    }
}