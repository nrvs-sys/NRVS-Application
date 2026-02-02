using System;

/// <summary>
/// Holds a single session id for this app run.
/// </summary>
public static class PresenceSession
{
    private static string _id;

    public static string Id
    {
        get
        {
            if (string.IsNullOrEmpty(_id))
            {
                // one id per app launch
                _id = Guid.NewGuid().ToString("N");
            }

            return _id;
        }
    }
}