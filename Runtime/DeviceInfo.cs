using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public static class DeviceInfo
{
    private const string PublicIpApiUrl = "https://api.ipify.org";

    /// <summary>
    /// Gets the local IP address of the device.
    /// </summary>
    /// <returns>The local IP address as a string.</returns>
    public static string GetLocalIpAddress()
    {
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return unicastAddress.Address.ToString();
                    }
                }
            }
        }
        return "127.0.0.1"; // Default to localhost if no network interface is found
    }

    /// <summary>
    /// Gets the public IP address of the device.
    /// </summary>
    /// <returns>The public IP address as a string.</returns>
    public static async Task<string> GetPublicIpAddressAsync()
    {
        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(PublicIpApiUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error getting public IP address: {ex.Message}");
                return "Error";
            }
        }
    }

    /// <summary>
    /// Gets the MAC address of the device.
    /// </summary>
    /// <returns>The MAC address as a string.</returns>
    public static string GetMacAddress()
    {
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                return BitConverter.ToString(networkInterface.GetPhysicalAddress().GetAddressBytes());
            }
        }
        return "00-00-00-00-00-00"; // Default to an invalid MAC address if no network interface is found
    }

    /// <summary>
    /// Gets the device's hostname.
    /// </summary>
    /// <returns>The hostname as a string.</returns>
    public static string GetHostName()
    {
        return Dns.GetHostName();
    }

    /// <summary>
    /// Gets the device's DNS addresses.
    /// </summary>
    /// <returns>An array of DNS addresses as strings.</returns>
    public static string[] GetDnsAddresses()
    {
        var dnsAddresses = new List<string>();
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                foreach (var dnsAddress in networkInterface.GetIPProperties().DnsAddresses)
                {
                    dnsAddresses.Add(dnsAddress.ToString());
                }
            }
        }
        return dnsAddresses.ToArray();
    }
}

