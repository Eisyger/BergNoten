namespace BergNoten.Helper;
public static class PermissionHelper
{
    public static async Task<bool> CheckAndRequestStoragePermissions()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        }

        return status == PermissionStatus.Granted;
    }
}