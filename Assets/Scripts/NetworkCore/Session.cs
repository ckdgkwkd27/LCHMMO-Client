﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace ServerCore
{
    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;
        RecvBuffer _recvBuffer = new RecvBuffer(65535);

        object _lock = new object();
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract void OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);

        void Clear()
        {
            lock(_lock)
            {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }

        public void Start(Socket socket)
        {
            _socket = socket;
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            RegisterRecv();
        }

        public void Send(List<ArraySegment<byte>> sendBufferList)
        {
            if (sendBufferList.Count == 0)
                return;

            lock( _lock)
            {
                foreach(ArraySegment<byte> buffer in sendBufferList)
                    _sendQueue.Enqueue(buffer);

                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Send(ArraySegment<byte> buffer)
        {
            lock( _lock)
            {
                _sendQueue.Enqueue(buffer);
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            Clear();
        }

        public void RegisterSend()
        {
            if (_disconnected == 1)
                return;

            while(_sendQueue.Count > 0 )
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendingList.Add(buff);
            }
            _sendArgs.BufferList = _pendingList;

            try
            {
                bool pending = _socket.SendAsync(_sendArgs);
                if (pending == false)
                    OnSendCompleted(null, _sendArgs);
            }
            catch (Exception ex)
            {
                Debug.Log($"Register Send Failed {ex}");
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock(_lock)
            {
                if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();
                        OnSend(_sendArgs.BytesTransferred);

                        if (_sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch(Exception ex)
                    {
                        UnityEngine.Debug.Log($"OnSendCompleted Failed {ex}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        void RegisterRecv()
        {
            if(_disconnected == 1) 
                return;

            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(_recvArgs);
                if (pending == false) 
                    OnRecvCompleted(null, _recvArgs);
            }
            catch(Exception ex)
            {
                Debug.Log($"RegisterRecv Failed {ex}");
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    if(_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if(processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    if(_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }
                    RegisterRecv();
                }
                catch(Exception ex)
                {
                    Debug.Log($"OnRecvCompleted Failed {ex}");
                }
            }
            else
            {
                Disconnect();
            }
        }
    }
}