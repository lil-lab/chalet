using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Cyan
{
    public class SocketManager : MonoBehaviour
    {
        #region vars
        // Part of code borrowed from MSDN C# docs: 
        // Client code: https://msdn.microsoft.com/en-us/library/kb5kfec7(v=vs.110).aspx
        // Server code: https://msdn.microsoft.com/en-us/library/6y0e13d3(v=vs.110).aspx

        // Incoming data from the client.
        public static string data = null;
        public Queue<string> instructionsQueue = new Queue<string>();


        private Socket listener;
        private BotController botController;

        // Contains data received
        private byte[] bytes;

        // Handler for listening
        private Socket handler = null;

        // Number of jobs sent
        private int jobsSent;

        //private byte[] image;
        private Texture2D tex;

        // Message id
        private int msgID;

        // use local host
        public static bool uselocalhost = true;
        public bool gotMessage = false;

        private bool establishedConnection = false;
        #endregion
        public void Start()
        {
            botController = GetComponent<BotController>();
            //EstablishServerSocket();
        }

        public void EstablishServerSocket()
        {
            msgID = 0;

            // Initialize size of the image and image related datastructures
            //int size = 300 * 300 * 3 * 4;
            //this.image = new byte[size];
            tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBAFloat, false);

            // Data buffer for incoming data.
            bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.

            IPAddress ipAddress = null;
            int port = 11000;
            if (uselocalhost)
            {
                ipAddress = IPAddress.Any;
                port = 11000;
            }
            else
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                ipAddress = ipHostInfo.AddressList[0];
                port = 0;
            }
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                int assignedPort = ((IPEndPoint)listener.LocalEndPoint).Port;
                Debug.Log("Use localhost " + uselocalhost + ". Running at " + ipAddress.ToString() + " at port " + assignedPort);
            }
            catch (Exception e)
            {
                Debug.LogError("Exception " + e.ToString());
            }
        }

        public void Listen()
        {
            try
            {
                if (handler == null)
                {
                    Debug.Log("Waiting for connection...");
                    handler = listener.Accept();
                    data = null;
                }

                else if (handler != null) // if there is a connection, be open to receiving instructions. Any instructions will be added to the instructions queue and handled in BotController
                {
                    Debug.Log("Connected");
                    bytes = new byte[1024];
                    int receivingbytes = handler.Receive(bytes);
                    data = null;
                    data = Encoding.ASCII.GetString(bytes, 0, receivingbytes);
                    if(data != "")
                    {
                        Debug.Log("Got message: " + data);
                        if (data.Contains("`"))
                        {
                            string[] instructions = data.Split('`');
                            foreach(string instruction in instructions)
                            {
                                instructionsQueue.Enqueue(instruction.ToLower());
                            }
                        }
                        else
                            instructionsQueue.Enqueue(data.ToLower());

                    }
                }
            }
            catch (Exception e)
            {
                //throw new ApplicationException("Could not listen to message from agent. Error " + e);
            }
        }

        #region SendMessage Methods 
        public void sendMessage(object message)
        {
            sendMessage((string)message);
        }

        public void sendMessage(string message)
        {

            try
            {
                if (handler == null)
                {
                    Debug.Log("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.
                    handler = listener.Accept();
                }

                byte[] msg = Encoding.ASCII.GetBytes("Unity Manager: " + message);
                handler.Send(msg);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Could not send message to agent. Error " + e);
            }
        }

        public void sendMessage(byte[] message)
        {
            try
            {
                if (handler == null)
                {
                    Debug.Log("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.
                    handler = listener.Accept();
                }

                msgID++;
                Debug.Log("Sending Image Message ID " + msgID);
                handler.Send(message);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Could not send message to agent. Error " + e);
            }
        }
        #endregion

        void Update()
        {
            if (establishedConnection == true)
                Listen();

            var currentScene = SceneManager.GetActiveScene();
            if (establishedConnection == false && currentScene.name != "Base" )
            {
                EstablishServerSocket();
                establishedConnection = true;
            }


        }
    }
}

