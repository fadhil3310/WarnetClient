using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using WebSocketSharp;
using System.Text.Json;
using System.Threading;
using System.Windows;
using WarnetClient.Event;
using ErrorEventArgs = WarnetClient.Event.ErrorEventArgs;

namespace WarnetClient
{

		/// <summary>
		/// A service used to communicate between Client and Server.
		/// This is a singleton class.
		/// </summary>
		internal class ClientService
		{
				private static readonly ClientService instance = new ClientService();
				public static ClientService Instance { get { return instance; } }

				private WebSocket webSocketClient = null;

				public event EventHandler Connecting;
				public event EventHandler<ErrorEventArgs> FailedConnecting;
				public event EventHandler Connected;
				public event EventHandler Disconnected;

				public event EventHandler<AddBillEventArgs> BillAdded;
				public event EventHandler BillRevoked;

				public event EventHandler OnClientError;

				public event EventHandler<DebugEventArgs> OnDebug;

				public bool IsConnected {
						get => webSocketClient == null ? false : webSocketClient.ReadyState == WebSocketState.Open;
				}
				public bool IsConnecting;

				public ClientService()
				{
					
				}

				/// <summary>
				/// Try to find and connect to a server in the same network.
				/// </summary>
				public void Initialize()
				{
						if (IsConnected || IsConnecting)
								return;

						FindServer();
				}

				/**
				 * <summary>
				 * Try to connect to a server directly if the IP address of the server has already been known.
				 * </summary>
				 */
				public void Initialize(IPAddress address)
				{
						if (IsConnected || IsConnecting)
								return;

						Connecting.Emit(this, new EventArgs());
						ConnectToServer(address);
				}

				/// <summary>
				/// Find a server by broadcasting a message telling that
				/// we are finding for a server. The server will then give us
				/// a response with the server IP Address information
				/// that we will use to connect to.
				/// </summary>
				private void FindServer()
				{
						if (IsConnected || IsConnecting)
								return;
						else
								IsConnecting = true;

						// Create UDP Client
						// Used for sending and receiving broadcast message through UDP
						UdpClient listener = new UdpClient();
						IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 5000);

						try
						{
								// Connect the UDP Client to the given IP Endpoint
								listener.Connect(endPoint);

								// Create message to be sent
								var message = new BroadcastMessage
								{
										Type = "FindingServer"
								};

								Connecting.Emit(this, new EventArgs());

								// Convert the message to a JSON string, and then send it
								byte[] messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
								listener.Send(messageBytes, messageBytes.Length);

								// Wait for a response from the server
								// Because listener.Receive blocks thread execution, we should run it on separate thread
								bool tryToFindServer = true;

								var thread = new Thread(async () =>
								{
										while (tryToFindServer)
										{
												try
												{
														var result = await listener.ReceiveAsync();

														BroadcastMessage receivedMessage = JsonSerializer.Deserialize<BroadcastMessage>(Encoding.UTF8.GetString(result.Buffer));

														// Connect to 
														if (receivedMessage.Type == "ServerHandshake")
														{
																tryToFindServer = false;
																FinishedFindingServer(IPAddress.Parse(result.RemoteEndPoint.Address.ToString()));
																break;
														}

												} catch(Exception ex) {
														tryToFindServer = false;

														if (ex is ThreadAbortException)
																return;

														FailedFindingServer(ex.Message);
														break;
												}
										}
								});
								thread.Start();
								
								// Finding server failed if the server didn't give response for 10 seconds
								Task.Run(() =>
								{
										Thread.Sleep(10000);
										if (tryToFindServer)
										{
												tryToFindServer = false;
												thread.Abort();
												FailedFindingServer("Tidak ada respon apa-apa dari server selama 10 detik");
										}
								});
						}
						catch (Exception ex)
						{
								FailedFindingServer(ex.Message + " \n" + ex.StackTrace);
						}

						void FinishedFindingServer(IPAddress serverAddress)
						{
								listener.Close();
								ConnectToServer(serverAddress);
						}
						void FailedFindingServer(string reason)
						{
								listener.Close();
								IsConnecting = false;
								FailedConnecting.Emit(this, new ErrorEventArgs(reason));
						}
				}

				private void ConnectToServer(IPAddress address)
				{
						webSocketClient = new WebSocket($"ws://{address}:8080/warnet");
						webSocketClient.OnError += (sender, e) => OnClientError.Emit(this, new ErrorEventArgs(e.Message));
						webSocketClient.OnMessage += WebSocketClient_OnMessage;

						var tryConnectingToServer = true;
						IsConnecting = true;

						// Connect to the websocket server
						// Run on separate thread because webSocketClient.Connect blocks thread execution
						var thread = new Thread(() =>
						{
								webSocketClient.Connect();

								// After attempting to connect,
								// check if our websocket client succesfully connected to the server
								if (webSocketClient.ReadyState == WebSocketState.Open)
										FinishedConnectingToServer();
								else
										FailedConnectingToServer($"Gagal menghubungkan ke server websocket: {webSocketClient.Url}");
						});
						thread.Start();

						// Finding server failed if the server didn't give response for 15 seconds
						Task.Run(() =>
						{
								Thread.Sleep(10000);
								if (tryConnectingToServer)
								{
										thread.Abort();
										FailedConnectingToServer($"Kehabisan waktu mencoba menghubungkan ke server websocket: {webSocketClient.Url}");
								}
						});

						void FinishedConnectingToServer()
						{
								tryConnectingToServer = false;
								IsConnecting = false;
								Connected.Emit(this, new EventArgs());
						}
						void FailedConnectingToServer(string reason)
						{
								tryConnectingToServer = false;
								IsConnecting = false;
								webSocketClient.Close();
								FailedConnecting.Emit(this, new ErrorEventArgs(reason));
						}
				}

				private void WebSocketClient_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
				{
						string rawMessage = Encoding.UTF8.GetString(e.RawData);
						var message = JsonSerializer.Deserialize<BroadcastMessage>(rawMessage);
						OnDebug.Emit(this, new DebugEventArgs(rawMessage));
						switch (message.Type)
						{
								case "AddBill":
										BillAdded.Emit(this, new AddBillEventArgs(int.Parse(message.Content)));
										break;
								case "RevokeBill":
										BillRevoked.Emit(this, new EventArgs());
										break;
						}
				}

				// <summary>
				// Close all connection to server
				// </summary>
				public void Dispose()
				{
						if (webSocketClient != null)
								webSocketClient.Close();
				}
		}
}
