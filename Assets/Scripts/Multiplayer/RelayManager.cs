using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public static RelayServerData Serverdata { get { return _serverData; } }
    static RelayServerData _serverData;
    public static string JoinCode { get { return _joinCode; } set { _joinCode = value; } }
    static string _joinCode;

    // Start is called before the first frame update
    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(9);
             _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Kode Relay yang didapat : "+ _joinCode);

            _serverData = new(allocation, "dtls");
            return _joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
        return "0";

    }

    public async void JoinRelay(string RelayCode)
    {
        try
        {
            _joinCode = RelayCode;
            JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(_joinCode);
            _serverData  = new(joinAlloc, "dtls");
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
