using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;

public interface IAvrNetworkRunnerCallbacks : INetworkRunnerCallbacks
{
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner){}
    /// <inheritdoc cref="INetworkRunnerCallbacks"/>>
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner){}
}
