using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;
namespace xChatLib
{
    #region Asynchronus Server
    public class AsyncTcpServer : IDisposable
    {
        #region Objects
        private string _Key = string.Empty;
        private bool _Disposed = false;
        private ushort _Port = ushort.MinValue;
        private Socket _Socket = null;
        private List<AsyncClient> _Clients = null;
        #endregion
        #region Accessors
        public Socket GetSocket
        {
            get
            {
                return _Socket;
            }
        }
        public List<AsyncClient> Clients
        {
            get
            {
                return _Clients;
            }
        }
        #endregion
        #region Events / Delegates
        public delegate void ClientConnectedHandler(object sender, AsyncClient e);
        public event ClientConnectedHandler OnClientConnected = (o, e) => { };
        #endregion
        #region Constructors
        public AsyncTcpServer(ushort Port, string Key)
        {
            _Key = Key;
            _Port = Port;
            _Clients = new List<AsyncClient>();
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, false);
            _Socket.Bind(new IPEndPoint(IPAddress.Any, Port));
            _Socket.Listen(int.MaxValue);
            _Socket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        #endregion
        #region Methods
        private void AcceptCallback(IAsyncResult AR)
        {
            AsyncClient Accepted = new AsyncClient(_Socket.EndAccept(AR), _Key);
            _Clients.Add(Accepted);
            OnClientConnected(this, Accepted);
            _Socket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        public void Close()
        {
            _Socket.Shutdown(SocketShutdown.Both);
            _Socket.Close();
            _Socket = null;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool Disposing)
        {
            if (Disposing && !_Disposed)
            {
                Close();
                for (int i = 0; i < _Clients.Count; i++)
                {
                    _Clients[i].Dispose();
                    _Clients[i] = null;
                }
                _Clients = null;
                _Disposed = true;
            }
        }
        #endregion
    }
    #endregion
    #region Asynchronus Client
    public class AsyncClient : IDisposable
    {
        #region Objects
        private bool _Disposed = false;
        private Socket _Socket = null;
        private uint _Recieved = uint.MinValue;
        private uint _Sent = uint.MinValue;
        private RC4 _InCipher = null;
        private RC4 _OutCipher = null;
        private byte[] _Buffer = new byte[65535];
        #endregion
        #region Accessors
        public Socket GetSocket
        {
            get
            {
                return _Socket;
            }
        }
        public uint TotalSent
        {
            get
            {
                return _Sent;
            }
        }
        public uint TotalRecieved
        {
            get
            {
                return _Recieved;
            }
        }
        #endregion
        #region Events / Delegates
        public delegate void ClientDisconnectedHandler(object sender, DisconnectionReason e);
        public delegate void DataRecievedHandler(object sender, byte[] e);
        public event ClientDisconnectedHandler OnClientDisconnected = (o, e) => { };
        public event DataRecievedHandler OnDataRecieved = (o, e) => { };
        #endregion
        #region Constructors
        public AsyncClient(IPAddress Address, ushort Port, string Key)
        {
            try
            {
                _InCipher = new RC4(Constants.Ansi.GetBytes(Key));
                _OutCipher = new RC4(Constants.Ansi.GetBytes(Key));
                _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, false);
                _Socket.Connect(new IPEndPoint(Address, Port));
                _Socket.BeginReceive(_Buffer, 0, _Buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), null);
            }
            catch (SocketException)
            {
                OnClientDisconnected(this, DisconnectionReason.SocketException);
            }
        }
        public AsyncClient(Socket Connection, string Key)
        {
            try
            {
                _InCipher = new RC4(Constants.Ansi.GetBytes(Key));
                _OutCipher = new RC4(Constants.Ansi.GetBytes(Key));
                _Socket = Connection;
                _Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, false);
                _Socket.BeginReceive(_Buffer, 0, _Buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), null);
            }
            catch (SocketException)
            {
                OnClientDisconnected(this, DisconnectionReason.SocketException);
            }
        }
        #endregion
        #region Methods
        private void RecieveCallback(IAsyncResult AR)
        {
            try
            {
                int Numread = _Socket.EndReceive(AR);
                // Client disconnected
                if (Numread == 0)
                    OnClientDisconnected(this, DisconnectionReason.RemoteHostDisconnected);
                if (Numread == 65535)
                {
                    _InCipher.decrypt(_Buffer);
                    OnDataRecieved(this, _Buffer);
                }
                else
                {
                    byte[] buffer = new byte[Numread];
                    Buffer.BlockCopy(_Buffer, 0, buffer, 0, buffer.Length);
                    _InCipher.decrypt(buffer);
                    OnDataRecieved(this, buffer);
                }
                _Recieved += (uint)Numread;
            }
            catch (SocketException)
            {
                OnClientDisconnected(this, DisconnectionReason.SocketException);
            }
            _Socket.BeginReceive(_Buffer, 0, _Buffer.Length,SocketFlags.None,  new AsyncCallback(RecieveCallback), null);
        }
        public void Send(byte[] Data, int Offset, int Length)
        {
            try
            {
                byte[] buffer = Data;
                _OutCipher.encrypt(buffer, Offset, Length);
                _Socket.BeginSend(buffer, Offset, Length,SocketFlags.None, new AsyncCallback(SendCallback), (uint)Length);
            }
            catch (SocketException)
            {
                OnClientDisconnected(this, DisconnectionReason.SocketException);
            }
        }
        public void Send(byte[] Data)
        {
            Send(Data, 0, Data.Length);
        }
        public void Send(Stream Source)
        {
            int Read = 0;
            byte[] buffer = new byte[65535];
            while ((Read = Source.Read(buffer, 0, buffer.Length)) != 0)
                Send(buffer, 0, Read);
        }
        private void SendCallback(IAsyncResult AR)
        {
            try
            {
                _Socket.EndSend(AR);
                _Sent += (uint)AR.AsyncState;
            }
            catch(SocketException)
            {
                OnClientDisconnected(this, DisconnectionReason.SocketException);
            }
        }
        public void Disconnect()
        {
            _Socket.Shutdown(SocketShutdown.Both);
            _Socket.Disconnect(false);
            _Socket.Close();
            _Socket = null;
            OnClientDisconnected(this, DisconnectionReason.Manual);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool Disposing)
        {
            if (Disposing && !_Disposed)
            {
                Disconnect();
                _Buffer = null;
                _Disposed = true;
            }
        }
        #endregion
    }
    #endregion
}
