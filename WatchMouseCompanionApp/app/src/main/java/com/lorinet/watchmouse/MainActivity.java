package com.lorinet.watchmouse;

import android.Manifest;
import android.annotation.TargetApi;
import android.app.Activity;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.hardware.usb.UsbAccessory;
import android.hardware.usb.UsbManager;
import android.net.wifi.WifiManager;
import android.os.AsyncTask;
import android.os.Environment;
import android.os.ParcelFileDescriptor;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.format.Formatter;
import android.util.Log;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.view.View;
import android.widget.Toast;

import com.garmin.android.connectiq.ConnectIQ;
import com.garmin.android.connectiq.ConnectIQ.ConnectIQListener;
import com.garmin.android.connectiq.ConnectIQ.IQConnectType;
import com.garmin.android.connectiq.ConnectIQ.IQDeviceEventListener;
import com.garmin.android.connectiq.ConnectIQ.IQSdkErrorStatus;
import com.garmin.android.connectiq.IQApp;
import com.garmin.android.connectiq.IQDevice;
import com.garmin.android.connectiq.IQDevice.IQDeviceStatus;
import com.garmin.android.connectiq.exception.InvalidStateException;
import com.garmin.android.connectiq.exception.ServiceUnavailableException;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.File;
import java.io.FileDescriptor;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.HttpURLConnection;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.URL;
import java.util.Collection;
import java.util.Iterator;
import java.util.List;
import java.util.concurrent.ArrayBlockingQueue;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.TimeUnit;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;

public class MainActivity extends AppCompatActivity {
    ConnectIQ iq;
    TextView stateLabel;
    TextView deviceLabel;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE)
                != PackageManager.PERMISSION_GRANTED) {
            requestPermissions(new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE}, 0);
        }
        stateLabel = findViewById(R.id.connectionStateLabel);
        deviceLabel = findViewById(R.id.deviceLabel);
        iq = ConnectIQ.getInstance(getApplicationContext(), IQConnectType.WIRELESS);
        iq.initialize(getApplicationContext(), true, new ConnectIQListener() {
            @Override
            public void onSdkReady() {

                // Do any post initialization setup.
            }

            @Override
            public void onInitializeError(IQSdkErrorStatus status) {

                // A failure has occurred during initialization.  Inspect
                // the IQSdkErrorStatus value for more information regarding
                // the failure.

            }

            @Override
            public void onSdkShutDown() {

            }
        });
        Button cb = findViewById(R.id.connectbtn);
        cb.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
                IQDevice dev = selectDevice();
                if (dev != null) {
                    deviceLabel.setText(dev.toString());
                    connect(dev);
                }
            }
        });
        Button wdlb = findViewById(R.id.wdlbtn);
        wdlb.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DownloadTask dt = new DownloadTask();
                Message("Downloading");
                dt.execute();
            }
        });
        Button pcb = findViewById(R.id.connectbtn2);
        pcb.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
                connectPC();
            }
        });
    }

    public void connectPC() {
        if(!connected)
        {
            EditText ipet = (EditText)findViewById(R.id.ipBox);
            ip = ipet.getText().toString();
            mTcpClient = new TcpClient();
            ct = new ConnectTask();
            ct.execute("");
            Button pcb = findViewById(R.id.connectbtn2);
            pcb.setText("Disconnect");
            connected = true;
        }
        else
        {
            mTcpClient.stopClient();
            connected = false;
            Button pcb = findViewById(R.id.connectbtn2);
            pcb.setText("Connect to PC");
        }

    }

    public IQDevice selectDevice() {
        try {
            List paired = iq.getKnownDevices();
            if (paired != null && paired.size() > 0) {
                for (Object device : paired) {
                    IQDeviceStatus status = iq.getDeviceStatus((IQDevice) device);
                    if (status == IQDeviceStatus.CONNECTED) {
                        return (IQDevice) device;
                    }
                }
            }
        } catch (Exception e) {
            Message("No connected Garmin devices found!");
            return null;
        }
        return null;
    }

    public void connect(IQDevice device) {
        try {
            iq.registerForAppEvents(device, new IQApp("6c8f72ab-c4b3-492d-8985-7592f33fd3d1", "WatchMouse", 1), new ConnectIQ.IQApplicationEventListener() {

                @Override
                public void onMessageReceived(IQDevice device, IQApp app, List<Object> messageData, ConnectIQ.IQMessageStatus status) {
                    if (status == ConnectIQ.IQMessageStatus.SUCCESS) {
                        for (Object o : messageData) {
                            if (o != null) {
                                String s = o.toString();
                                if (s.length() > 0) {
                                    tx(s);
                                }
                            }
                        }
                    }
                }
            });
            stateLabel.setText("Connected");
            Message("Connected");
        } catch (Exception ex) {

        }
    }
    private boolean unpackZip(String path, String zipname)
    {
        InputStream is;
        ZipInputStream zis;
        try
        {
            String filename;
            is = new FileInputStream(path + zipname);
            zis = new ZipInputStream(new BufferedInputStream(is));
            ZipEntry ze;
            byte[] buffer = new byte[1024];
            int count;

            while ((ze = zis.getNextEntry()) != null)
            {
                filename = ze.getName();

                // Need to create directories if not exists, or
                // it will generate an Exception...
                if (ze.isDirectory()) {
                    File fmd = new File(path + filename);
                    fmd.mkdirs();
                    continue;
                }

                FileOutputStream fout = new FileOutputStream(path + filename);

                while ((count = zis.read(buffer)) != -1)
                {
                    fout.write(buffer, 0, count);
                }

                fout.close();
                zis.closeEntry();
            }

            zis.close();
        }
        catch(IOException e)
        {
            e.printStackTrace();
            return false;
        }

        return true;
    }
    TcpClient mTcpClient;
    ConnectTask ct;
    String ip = "";

    boolean connected = false;

    private void tx(String b) {
        try {
            mTcpClient.sendMessage(b);
        } catch (Exception e) {
            Message("Fail");
        }
    }

    public void Message(String text) {
        Toast.makeText(getApplicationContext(), text, Toast.LENGTH_LONG).show();
    }

    public String getLocalIpAddress() {
        WifiManager wm = (WifiManager) getApplicationContext().getSystemService(WIFI_SERVICE);
        String ip = Formatter.formatIpAddress(wm.getConnectionInfo().getIpAddress());
        return ip;
    }
    class ConnectTask extends AsyncTask<String, Void, TcpClient> {

        @Override
        protected TcpClient doInBackground(String... message) {

            TcpClient.SERVER_IP = ip;
            mTcpClient = new TcpClient();
            mTcpClient.run();

            return null;
        }
    }
    class DownloadTask extends AsyncTask<Void, Void, Void>
    {

        @Override
        protected Void doInBackground(Void... voids) {
            downloadZipFile("http://lorinet.000webhostapp.com/watchmouse/watchmouseapp.zip", Environment.getExternalStorageDirectory() + "/" + Environment.DIRECTORY_DOWNLOADS + "/" + "watchmouseapp.zip");
            unpackZip(Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS).getPath() + "/" + "watchmouseapp.zip");
            return null;
        }
        public void downloadZipFile(String urlStr, String destinationFilePath) {
            InputStream input = null;
            OutputStream output = null;
            HttpURLConnection connection = null;
            try {
                URL url = new URL(urlStr);

                connection = (HttpURLConnection) url.openConnection();
                connection.connect();

                if (connection.getResponseCode() != HttpURLConnection.HTTP_OK) {
                    Log.d("downloadZipFile", "Server ResponseCode=" + connection.getResponseCode() + " ResponseMessage=" + connection.getResponseMessage());
                }

                // download the file
                input = connection.getInputStream();

                Log.d("downloadZipFile", "destinationFilePath=" + destinationFilePath);
                new File(destinationFilePath).createNewFile();
                output = new FileOutputStream(destinationFilePath);

                byte data[] = new byte[4096];
                int count;
                while ((count = input.read(data)) != -1) {
                    output.write(data, 0, count);
                }
            } catch (Exception e) {
                e.printStackTrace();
                return;
            } finally {
                try {
                    if (output != null) output.close();
                    if (input != null) input.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }

                if (connection != null) connection.disconnect();
            }

            File f = new File(destinationFilePath);

            Log.d("downloadZipFile", "f.getParentFile().getPath()=" + f.getParentFile().getPath());
            Log.d("downloadZipFile", "f.getName()=" + f.getName().replace(".zip", ""));
        }
        public boolean unpackZip(String filePath) {
            InputStream is;
            ZipInputStream zis;
            try {

                File zipfile = new File(filePath);
                String parentFolder = zipfile.getParentFile().getPath();
                String filename;

                is = new FileInputStream(filePath);
                zis = new ZipInputStream(new BufferedInputStream(is));
                ZipEntry ze;
                byte[] buffer = new byte[1024];
                int count;

                while ((ze = zis.getNextEntry()) != null) {
                    filename = ze.getName();

                    if (ze.isDirectory()) {
                        File fmd = new File(parentFolder + "/" + filename);
                        fmd.mkdirs();
                        continue;
                    }

                    FileOutputStream fout = new FileOutputStream(parentFolder + "/" + filename);

                    while ((count = zis.read(buffer)) != -1) {
                        fout.write(buffer, 0, count);
                    }

                    fout.close();
                    zis.closeEntry();
                }

                zis.close();
            } catch(IOException e) {
                e.printStackTrace();
                return false;
            }

            return true;
        }
    }
}

