using System.Net;
using System.Net.Sockets;

static async Task ProcessNewConnection(TcpClient client)
{
    Console.WriteLine($"Connected client: {client.Client.RemoteEndPoint}=>{client.Client.LocalEndPoint}");

    // Get a stream object for reading and writing
    var stream = client.GetStream();    

    // Buffer for reading data
    var bytes = new byte[256];

    // Bytes to read
    int bytesRead;

    // Loop to receive all the data sent by the client.
    while ((bytesRead = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
    {
        // Send back a response.
        await stream.WriteAsync(bytes, 0, bytesRead);
    }

    Console.WriteLine($"Diconnected client: {client.Client.RemoteEndPoint}=>{client.Client.LocalEndPoint}");
}

TcpListener? server = null;

try
{
    Console.WriteLine("Starting server...");

    // Set the TcpListener on port 12345.
    int port = 12345;

    // Listen for all IP
    var ipEndPoint = new IPEndPoint(IPAddress.Any, port);

    // Create lister
    server = new TcpListener(ipEndPoint);

    // Start listening for client requests.
    server.Start();

    Console.WriteLine($"Server was listed on port {port}");    

    // Enter the listening loop.
    while (true)
    {
        // Perform a non-blocking call to accept requests.
        var client = await server.AcceptTcpClientAsync();

        // Start new task for every connection
        _ = Task.Factory.StartNew(async () => await ProcessNewConnection(client));
    }
}
catch (SocketException exception)
{
    Console.WriteLine($"SocketException: {exception.Message}");
}
finally
{
    // Stop listening for new clients.
    server?.Stop();
}

Console.WriteLine("Server exited");
