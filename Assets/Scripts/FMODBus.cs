using FMOD.Studio;
using ThanhDV.AudioManager.FMOD;

public static class FMODBus
{
    public static Bus MASTER = AudioManager.Instance.FMODReferences.GetBus("Master");
}
