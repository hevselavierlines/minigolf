package im.fhooe.at.golfclubsimulator;

import android.app.Activity;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.AsyncTask;
import android.os.Bundle;
import android.provider.ContactsContract;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.UnknownHostException;

public class IPInputActivity extends AppCompatActivity {
    private EditText etIpAddress;
    private Button btConnect;
    private SharedPreferences save;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_ipinput);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        btConnect = (Button) findViewById(R.id.btConnect);
        etIpAddress = (EditText) findViewById(R.id.etIP);

        save = getSharedPreferences(getApplication().getPackageName(), Activity.MODE_PRIVATE);
        etIpAddress.setText(save.getString("ip", ""));


        btConnect.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                String ip = etIpAddress.getText().toString();
                Intent intent = new Intent(IPInputActivity.this, GolfClub.class);
                intent.putExtra("IPADDRESS", ip);
                startActivity(intent);

                SharedPreferences.Editor editor = save.edit();
                editor.putString("ip", ip);
                editor.apply();
            }
        });
        class BroadcastSearcher extends AsyncTask<Void, Void, String> {
            private DatagramSocket udpSocket;
            public BroadcastSearcher() {
                try {
                    udpSocket = new DatagramSocket();
                    udpSocket.setBroadcast(true);
                } catch (SocketException e) {
                    e.printStackTrace();
                }
            }
            @Override
            protected String doInBackground(Void... params) {
                try {
                    InetAddress address = InetAddress.getByName("255.255.255.255");
                    byte[] buffer = new byte[2];
                    buffer[0] = 0;
                    buffer[1] = 1;
                    DatagramPacket packet = new DatagramPacket(
                            buffer, buffer.length, address, 4545
                    );
                    udpSocket.send(packet);


                    // Create a packet to receive data into the buffer
                    DatagramPacket packet2 = new DatagramPacket(buffer, buffer.length);
                    udpSocket.receive(packet2);
                    return packet2.getAddress().getHostAddress();
                } catch (UnknownHostException e) {
                    e.printStackTrace();
                } catch (IOException e) {
                    e.printStackTrace();
                }
                return null;
            }

            @Override
            protected void onPostExecute(String s) {
                super.onPostExecute(s);
                etIpAddress.setText(s);
            }
        }

        //BroadcastSearcher bc = new BroadcastSearcher();
        //bc.execute();
    }



}
